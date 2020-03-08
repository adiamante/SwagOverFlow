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
using System.Text;
using System.Windows.Data;

namespace SwagOverflowWPF.ViewModels
{
    public class SwagItemViewModel : ViewModelBase, ISwagHeirarchy<SwagItemViewModel>
    {
        #region Private/Protected Members
        String _display, _alternateId;
        protected String _valueTypeString;
        Int32 _itemId, _groupId, _sequence;
        Int32? _parentId, _groupRootId;
        Boolean _isExpanded;
        protected ObservableCollection<SwagItemViewModel> _children = new ObservableCollection<SwagItemViewModel>();
        SwagItemViewModel _parent;
        CollectionViewSource _childrenCollectionViewSource;
        SwagGroupViewModel _group, _groupRoot;
        protected Object _value;
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
            get { return (object)_value; }
            set { SetValue(ref _value, value); }
        }
        #endregion Value
        #region ValueType
        [NotMapped]
        public virtual Type ValueType { get { return null; } set { } }
        #endregion ValueType
        #region ValueTypeString
        public virtual String ValueTypeString
        {
            get { return _valueTypeString; }
            set { SetValue(ref _valueTypeString, value); }
        }
        #endregion ValueTypeString
        #region Parent
        public virtual SwagItemViewModel Parent
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
        public ObservableCollection<SwagItemViewModel> Children
        {
            get { return _children; }
            set { SetValue(ref _children, value); }
        }
        #endregion Children
        #region Group
        public SwagGroupViewModel Group
        {
            get { return _group; }
            set { SetValue(ref _group, value); }
        }
        #endregion Group
        #region GroupRoot
        public SwagGroupViewModel GroupRoot
        {
            get { return _groupRoot; }
            set { SetValue(ref _groupRoot, value); }
        }
        #endregion GroupRoot
        #endregion Properties

        #region Initialization
        public SwagItemViewModel()
        {
            PropertyChanged += SwagItemViewModel_PropertyChanged;
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void SwagItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Group != null && e.PropertyName != "Group")
            {
                Group.OnSwagItemChanged(this);
            }
        }

        private void _children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagItemViewModel child in e.NewItems)
                {
                    child.Parent = this;
                    child.Group = this.Group;
                }
            }
        }
        #endregion Initialization
    }

    public class SwagItemViewModel<T> : SwagItemViewModel
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
        public SwagItemViewModel() : base()
        {

        }

        #endregion Initialization
    }

    public class SwagIndexedItemViewModel : SwagItemViewModel
    {
        #region Private Members
        Dictionary<String, SwagIndexedItemViewModel> _dict = new Dictionary<string, SwagIndexedItemViewModel>();
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
        public SwagIndexedItemViewModel this[String key]
        {
            get
            {
                if (!_dict.ContainsKey(key))
                {
                    SwagIndexedItemViewModel child = (SwagIndexedItemViewModel)Activator.CreateInstance(this.GetType());
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
        #endregion Properties

        #region Initialization
        public SwagIndexedItemViewModel() : base()
        {
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagIndexedItemViewModel newItem in e.NewItems)
                {
                    _dict.Add(newItem.Key, newItem);
                }
            }

            if (e.OldItems != null)
            {
                foreach (SwagIndexedItemViewModel oldItem in e.OldItems)
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

    public class SwagIndexedItemViewModel<T> : SwagIndexedItemViewModel
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
        public SwagIndexedItemViewModel() : base()
        {
            
        }
        #endregion Initialization
    }

    public class SwagItemChangedEventArgs : EventArgs
    {
        public SwagItemViewModel SwagItem { get; set; }
    }

    public class SwagGroupViewModel : ViewModelBase
    {
        #region Private/Protected Members
        String _name, _display, _alternateId;
        Int32 _groupId;
        Int32? _rootId;
        protected SwagItemViewModel _root;
        protected ObservableCollection<SwagItemViewModel> _descendants = new ObservableCollection<SwagItemViewModel>();
        #endregion Private/Protected Members

        #region Events
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

        public virtual void OnSwagItemChanged(SwagItemViewModel swagItem)
        {
            SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs() { SwagItem = swagItem });
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
        public SwagItemViewModel Root
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
        public virtual ObservableCollection<SwagItemViewModel> Descendants
        {
            get { return _descendants; }
            set { SetValue(ref _descendants, value); }
        }
        #endregion Descendants
        #endregion Properties

        #region Initialization
        public SwagGroupViewModel() : base()
        {
            
        }
        #endregion Initialization

        #region Iterator
        public SwagItemPreOrderIterator<SwagItemViewModel> CreateIterator()
        {
            return new SwagItemPreOrderIterator<SwagItemViewModel>(Root);
        }
        #endregion Iterator
    }

    public class SwagIndexedGroupViewModel : SwagGroupViewModel
    {
        #region Private/Protected Members
        protected ObservableCollection<SwagIndexedItemViewModel> _indexedDescendants = new ObservableCollection<SwagIndexedItemViewModel>();
        #endregion Private/Protected Members

        #region Properties
        #region Root
        [NotMapped]
        public SwagIndexedItemViewModel IndexedRoot
        {
            get { return (SwagIndexedItemViewModel)_root; }
            set
            {
                _root.Group = this;
                SetValue(ref _root, value);
            }
        }
        #endregion Root
        #region Descendants
        [NotMapped]
        public virtual ObservableCollection<SwagIndexedItemViewModel> IndexedDescendants
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
        public SwagIndexedGroupViewModel() : base()
        {

        }
        #endregion Initialization

        #region Indexer
        public SwagIndexedItemViewModel this[String key]
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

    public class SwagGroupViewModel<T> : SwagGroupViewModel where T : SwagItemViewModel, ISwagHeirarchy<SwagItemViewModel>, new()
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
        public SwagGroupViewModel() : base()
        {
            RootGeneric = new T();
            RootGeneric.Group = this;
        }
        #endregion Initialization
    }

    public class SwagIndexedGroupViewModel<T> : SwagIndexedGroupViewModel where T : SwagIndexedItemViewModel, new()
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
        public SwagIndexedGroupViewModel() : base()
        {
            IndexedRootGeneric = new T();
            IndexedRootGeneric.Group = this;
        }
        #endregion Initialization

    }
}
