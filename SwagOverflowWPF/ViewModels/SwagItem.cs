using Newtonsoft.Json;
using SwagOverflowWPF.Interface;
using SwagOverflowWPF.Iterator;
using SwagOverflowWPF.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Data;

namespace SwagOverflowWPF.ViewModels
{
    public class SwagItem : ViewModelBaseExtended, ISwagHeirarchy<SwagItem>
    {
        #region Private/Protected Members
        String _display, _alternateId;
        protected String _valueTypeString;
        Int32 _itemId, _groupId, _sequence;
        Int32? _parentId, _groupRootId;
        Boolean _isExpanded;
        protected ObservableCollection<SwagItem> _children = new ObservableCollection<SwagItem>();
        SwagItem _parent;
        CollectionViewSource _childrenCollectionViewSource;
        SwagGroup _group, _groupRoot;
        protected Object _value;
        protected Type _valueType = null;
        #endregion Private/Protected Members

        #region Properties
        #region GroupId
        public Int32 GroupId
        {
            get { return _groupId; }
            set { SetValue(ref _groupId, value); }
        }
        #endregion GroupId
        #region ItemId
        public Int32 ItemId
        {
            get { return _itemId; }
            set { SetValue(ref _itemId, value); }
        }
        #endregion ItemId
        #region AlternateId
        public String AlternateId
        {
            get { return _alternateId; }
            set { SetValue(ref _alternateId, value); }
        }
        #endregion AlternateId
        #region GroupRootId
        public Int32? GroupRootId
        {
            get { return _groupRootId; }
            set { SetValue(ref _groupRootId, value); }
        }
        #endregion GroupRootId
        #region ParentId
        public Int32? ParentId
        {
            get { return _parentId; }
            set { SetValue(ref _parentId, value); }
        }
        #endregion ParentId
        #region Sequence
        public Int32 Sequence
        {
            get { return _sequence; }
            set { SetValue(ref _sequence, value); }
        }
        #endregion Sequence
        #region Display
        public String Display
        {
            get { return _display; }
            set { SetValue(ref _display, value); }
        }
        #endregion Display
        #region IsExpanded
        public Boolean IsExpanded
        {
            get { return _isExpanded; }
            set { SetValue(ref _isExpanded, value); }
        }
        #endregion IsExpanded
        #region Value
        public virtual object Value
        {
            get 
            {
                if (ValueType != null && _value != null && ValueType != _value.GetType())
                {
                    if (ValueType == typeof(Boolean))
                    {
                        _value = Boolean.Parse(_value.ToString());
                    }
                    else if (ValueType == typeof(String))
                    {
                        _value = _value.ToString();
                    }
                    else
                    {
                        _value = JsonConvert.DeserializeObject(_value.ToString(), ValueType);
                    }
                }
                return (object)_value; 
            }
            set { SetValue(ref _value, value); }
        }
        #endregion Value
        #region ValueType
        [NotMapped]
        public virtual Type ValueType 
        { 
            get 
            {
                if (_valueType == null && !String.IsNullOrEmpty(_valueTypeString))
                {
                    _valueType = JsonConvert.DeserializeObject<Type>(_valueTypeString);
                }
                return _valueType; 
            } 
            set { }
        }
        #endregion ValueType
        #region ValueTypeString
        public virtual String ValueTypeString
        {
            get { return _valueTypeString; }
            set { SetValue(ref _valueTypeString, value); }
        }
        #endregion ValueTypeString
        #region Parent
        public virtual SwagItem Parent
        {
            get { return _parent; }
            set { SetValue(ref _parent, value); }
        }
        #endregion Parent
        #region ChildrenView
        public ICollectionView ChildrenView
        {
            get { return _childrenCollectionViewSource.View; }
        }
        #endregion ChildrenView
        #region Children
        public ObservableCollection<SwagItem> Children
        {
            get { return _children; }
            set { SetValue(ref _children, value); }
        }
        #endregion Children
        #region Group
        public SwagGroup Group
        {
            get { return _group; }
            set { SetValue(ref _group, value); }
        }
        #endregion Group
        #region GroupRoot
        public SwagGroup GroupRoot
        {
            get { return _groupRoot; }
            set { SetValue(ref _groupRoot, value); }
        }
        #endregion GroupRoot
        #endregion Properties

        #region Initialization
        public SwagItem()
        {
            PropertyChangedExtended += SwagItemViewModel_PropertyChangedExtended;
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void SwagItemViewModel_PropertyChangedExtended(object sender, PropertyChangedExtendedEventArgs e)
        {
            Group?.OnSwagItemChanged(this, e);
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagItem child in e.NewItems)
                {
                    child.Parent = this;
                    child.Group = this.Group;
                    if (child.Sequence <= 0)
                    {
                        child.Sequence = this.Children.Count;
                    }
                }
            }
        }
        #endregion Initialization
    }

    public class SwagItem<T> : SwagItem
    {
        #region Properties
        #region ValueType
        public override Type ValueType { get { return typeof(T); } set { } }
        #endregion ValueType
        #region ValueTypeString
        public override String ValueTypeString
        {
            get { return JsonHelper.ToJsonString(typeof(T)); }
            set { SetValue(ref _valueTypeString, value); }
        }
        #endregion ValueTypeString
        #region GenericValue
        [NotMapped]
        public T GenericValue
        {
            get 
            { 
                if (_value is T)
                {
                    return (T)_value;
                }
                return default(T);
            }
            set 
            {
                T _temp = default(T);
                SetValue<T>(ref _temp, (T)value);
                _value = _temp;
            }
        }
        #endregion GenericValue
        #endregion Properties

        #region Initialization
        public SwagItem() : base()
        {

        }

        #endregion Initialization
    }

    public class SwagIndexedItem : SwagItem
    {
        #region Private Members
        Dictionary<String, SwagIndexedItem> _dict = new Dictionary<string, SwagIndexedItem>();
        String _key;
        #endregion Private Members

        #region Properties        
        #region Key
        public String Key
        {
            get { return _key; }
            set { SetValue(ref _key, value); }
        }
        #endregion Key
        #region HasChildren
        public Boolean HasChildren
        {
            get { return _children.Count > 0; }
        }
        #endregion HasChildren
        #region Indexer
        public SwagIndexedItem this[String key]
        {
            get
            {
                if (!_dict.ContainsKey(key))
                {
                    SwagIndexedItem child = (SwagIndexedItem)Activator.CreateInstance(this.GetType());
                    child.Key = child.Display = key;
                    _children.Add(child);
                }
                return _dict[key];
            }
            set
            {
                if (!_dict.ContainsKey(key))
                {
                    value.Display = value.Key = key;
                    _children.Add(value);
                }
                _dict[key] = value;
                OnPropertyChanged();
            }
        }
        #endregion Indexer
        #region Path
        public String Path
        {
            get 
            {
                SwagItem tempNode = this;
                String path = "";
                while (tempNode != null)
                {
                    path = $"{tempNode.Display}/{path}";
                    tempNode = tempNode.Parent;
                }
                return path.Trim('/'); 
            }
        }
        #endregion Path
        #endregion Properties

        #region Initialization
        public SwagIndexedItem() : base()
        {
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagIndexedItem newItem in e.NewItems)
                {
                    _dict.Add(newItem.Key, newItem);
                }
            }

            if (e.OldItems != null)
            {
                foreach (SwagIndexedItem oldItem in e.OldItems)
                {
                    _dict.Remove(oldItem.Key);
                }
            }
        }

        #endregion Initialization

        #region Methods
        public T GetValue<T>()
        {
            return (T)Value;
        }

        public void SetValue<T>(T val)
        {
            Value = val;
        }
        #endregion Methods
    }

    public class SwagIndexedItem<T> : SwagIndexedItem
    {
        #region Properties
        #region ValueType
        public override Type ValueType { get { return typeof(T); } set { } }
        #endregion ValueType
        #region ValueTypeString
        public override String ValueTypeString
        {
            get { return JsonHelper.ToJsonString(typeof(T)); }
            set { SetValue(ref _valueTypeString, value); }
        }
        #endregion ValueTypeString
        #region GenericValue
        [NotMapped]
        public T GenericValue
        {
            get { return (T)_value; }
            set 
            {
                T _temp = default(T);
                SetValue<T>(ref _temp, (T)value);
                _value = _temp;
            }
        }
        #endregion GenericValue
        #endregion Properties

        #region Initialization
        public SwagIndexedItem() : base()
        {
            
        }
        #endregion Initialization
    }

    public class SwagItemChangedEventArgs : EventArgs
    {
        public SwagItem SwagItem { get; set; }
        public PropertyChangedExtendedEventArgs PropertyChangedArgs { get; set; }
    }

    public class SwagGroup : ViewModelBaseExtended
    {
        #region Private/Protected Members
        String _name, _display, _alternateId;
        Int32 _groupId;
        Int32? _rootId;
        protected SwagItem _root;
        protected ObservableCollection<SwagItem> _descendants = new ObservableCollection<SwagItem>();
        #endregion Private/Protected Members

        #region Events
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

        public virtual void OnSwagItemChanged(SwagItem swagItem, PropertyChangedExtendedEventArgs e)
        {
            SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs() { SwagItem = swagItem, PropertyChangedArgs = e });
        }
        #endregion Events

        #region Properties
        #region Name
        public String Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }
        #endregion Name
        #region Display
        public String Display
        {
            get { return _display; }
            set { SetValue(ref _display, value); }
        }
        #endregion Display
        #region GroupId
        public Int32 GroupId
        {
            get { return _groupId; }
            set { SetValue(ref _groupId, value); }
        }
        #endregion GroupId
        #region AlternateId
        public String AlternateId
        {
            get { return _alternateId; }
            set { SetValue(ref _alternateId, value); }
        }
        #endregion AlternateId
        #region RootId
        public Int32? RootId
        {
            get { return _rootId; }
            set { SetValue(ref _rootId, value); }
        }
        #endregion RootId
        #region Root
        [NotMapped]
        public SwagItem Root
        {
            get { return _root; }
            set 
            {
                _root.Group = this;
                SetValue(ref _root, value); 
            }
        }
        #endregion Root
        #region Descendants
        public virtual ObservableCollection<SwagItem> Descendants
        {
            get { return _descendants; }
            set { SetValue(ref _descendants, value); }
        }
        #endregion Descendants
        #endregion Properties

        #region Initialization
        public SwagGroup() : base()
        {
            
        }
        #endregion Initialization

        #region Iterator
        public SwagItemPreOrderIterator<SwagItem> CreateIterator()
        {
            return new SwagItemPreOrderIterator<SwagItem>(Root);
        }
        #endregion Iterator
    }

    public class SwagIndexedGroup : SwagGroup
    {
        #region Private/Protected Members
        protected ObservableCollection<SwagIndexedItem> _indexedDescendants = new ObservableCollection<SwagIndexedItem>();
        #endregion Private/Protected Members

        #region Properties
        #region Root
        [NotMapped]
        public SwagIndexedItem IndexedRoot
        {
            get { return (SwagIndexedItem)_root; }
            set
            {
                _root.Group = this;
                SetValue(ref _root, value);
            }
        }
        #endregion Root
        #region Descendants
        [NotMapped]
        public virtual ObservableCollection<SwagIndexedItem> IndexedDescendants
        {
            get { return _indexedDescendants; }
            set 
            { 
                SetValue(ref _indexedDescendants, value);
            }
        }
        #endregion Descendants
        #endregion Properties

        #region Initialization
        public SwagIndexedGroup() : base()
        {

        }
        #endregion Initialization

        #region Indexer
        public SwagIndexedItem this[String key]
        {
            get { return IndexedRoot[key]; }
            set
            {
                IndexedRoot[key] = value;
                OnPropertyChanged();
            }
        }
        #endregion Indexer

    }

    public class SwagGroup<T> : SwagGroup where T : SwagItem, ISwagHeirarchy<SwagItem>, new()
    {
        #region RootGeneric
        [NotMapped]
        public T RootGeneric
        {
            get { return (T)_root; }
            set { SetValue(ref _root, value); }
        }
        #endregion RootGeneric

        #region Initialization
        public SwagGroup() : base()
        {
            RootGeneric = new T();
            RootGeneric.Group = this;
        }
        #endregion Initialization
    }

    public class SwagIndexedGroup<T> : SwagIndexedGroup where T : SwagIndexedItem, new()
    {
        #region RootGeneric
        [NotMapped]
        public T IndexedRootGeneric
        {
            get { return (T)_root; }
            set { SetValue(ref _root, value); }
        }
        #endregion RootGeneric

        #region Initialization
        public SwagIndexedGroup() : base()
        {
            IndexedRootGeneric = new T();
            IndexedRootGeneric.Group = this;
        }
        #endregion Initialization

    }
}
