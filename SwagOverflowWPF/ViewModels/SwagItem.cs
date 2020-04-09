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
    #region Interfaces
    public interface ISwagChild<TParent, TChild>
            where TParent : class, ISwagParent<TParent, TChild>
            where TChild : class, ISwagChild<TParent, TChild>
    {
        TParent Parent { get; set; }
        String Display { get; set; }        //Used for debugging
        Int32 Sequence { get; set; }
    }

    public interface ISwagParent<TParent, TChild> : ISwagChild<TParent, TChild>, ISwagItemChanged
        where TParent : class, ISwagParent<TParent, TChild>
        where TChild : class, ISwagChild<TParent, TChild>
    {
        ObservableCollection<TChild> Children { get; set; }
    }

    public interface ISwagItemChanged
    {
        event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;
        void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedExtendedEventArgs e);
    }
    #endregion Interfaces

    #region SwagItemChangedEventArgs
    public class SwagItemChangedEventArgs : EventArgs
    {
        public SwagItemBase SwagItem { get; set; }
        public PropertyChangedExtendedEventArgs PropertyChangedArgs { get; set; }
        public String Message { get; set; }
        
    }
    #endregion SwagItemChangedEventArgs

    public abstract class SwagItemBase : ViewModelBaseExtended
    {
        #region Private/Protected Members
        String _display, _alternateId;
        protected String _valueTypeString;
        Int32 _itemId, _sequence;
        Boolean _isExpanded, _canUndo = true;
        String _key;
        protected Object _objValue;
        protected Type _valueType = null;
        #endregion Private/Protected Members

        #region Properties
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
        #region CanUndo
        public Boolean CanUndo
        {
            get { return _canUndo; }
            set { SetValue(ref _canUndo, value); }
        }
        #endregion CanUndo
        #region Key
        public String Key
        {
            get { return _key; }
            set { SetValue(ref _key, value); }
        }
        #endregion Key
        #region ValueType
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
        #region ObjValue
        public virtual object ObjValue
        {
            get { return _objValue; }
            set { SetValue(ref _objValue, value); }
        }
        #endregion ObjValue
        #endregion Properties

        #region Methods
        public T GetValue<T>()
        {
            return (T)ObjValue;
        }

        public void SetValue<T>(T val)
        {
            ObjValue = val;
        }
        #endregion Methods
    }

    public abstract class SwagItem<TParent, TChild> : SwagItemBase, ISwagChild<TParent, TChild>
        where TParent : class, ISwagParent<TParent, TChild>
        where TChild : class, ISwagChild<TParent, TChild>
    {
        #region Private/Protected Members
        TParent _parent;
        Int32? _parentId;
        #endregion Private/Protected Members

        #region Properties
        #region ParentId
        public Int32? ParentId
        {
            get { return _parentId; }
            set { SetValue(ref _parentId, value); }
        }
        #endregion ParentId
        #region Parent
        public virtual TParent Parent
        {
            get { return _parent; }
            set { SetValue(ref _parent, value); }
        }
        #endregion Parent
        #endregion Properties

        #region Initialization
        public SwagItem()
        {
            PropertyChangedExtended += SwagIndexedItem_PropertyChangedExtended;
        }

        private void SwagIndexedItem_PropertyChangedExtended(object sender, PropertyChangedExtendedEventArgs e)
        {
            Parent?.OnSwagItemChanged(this, e);
        }
        #endregion Initialization
    }

    public abstract class SwagItem<TParent, TChild, T> : SwagItem<TParent, TChild>
        where TParent : class, ISwagParent<TParent, TChild>
        where TChild : class, ISwagChild<TParent, TChild>
    {
        #region Private/Protected Members
        protected T _value;
        #endregion Private/Protected Members

        #region Properties
        #region Value
        [NotMapped]
        public virtual T Value
        {
            get 
            {
                if (ValueType != null && _objValue != null && ValueType != _objValue.GetType())
                {
                    if (ValueType == typeof(Boolean))
                    {
                        _objValue = Boolean.Parse(_objValue.ToString());
                    }
                    else if (ValueType == typeof(String))
                    {
                        _objValue = _objValue.ToString();
                    }
                    else
                    {
                        _objValue = JsonConvert.DeserializeObject(_objValue.ToString(), ValueType);
                    }
                }

                if (_objValue != null && _value == null)
                {
                    _value = (T)_objValue;
                }

                return _value; 
            }
            set
            {
                SetValue(ref _value, value);
                SetValue(ref _objValue, value);
            }
        }
        #endregion Value
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
        #endregion Properties

        #region Initialization
        public SwagItem() : base()
        {
        }
        #endregion Initialization
    }

    public abstract class SwagItemGroup<TParent, TChild> : SwagItem<TParent, TChild>, ISwagParent<TParent, TChild>
        where TParent : class, ISwagParent<TParent, TChild>
        where TChild : class, ISwagChild<TParent, TChild>
    {
        #region Private/Protected Members
        String _name;
        protected ObservableCollection<TChild> _children = new ObservableCollection<TChild>();
        CollectionViewSource _childrenCollectionViewSource;
        #endregion Private/Protected Members

        #region Events
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

        public virtual void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedExtendedEventArgs e)
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
        #region ChildrenView
        public ICollectionView ChildrenView
        {
            get { return _childrenCollectionViewSource.View; }
        }
        #endregion ChildrenView
        #region Children
        public ObservableCollection<TChild> Children
        {
            get { return _children; }
            set { SetValue(ref _children, value); }
        }
        #endregion Children
        #region HasChildren
        public Boolean HasChildren
        {
            get { return _children.Count > 0; }
        }
        #endregion HasChildren
        #endregion Properties

        #region Initialization
        public SwagItemGroup() : base()
        {
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TChild newItem in e.NewItems)
                {
                    newItem.Parent = this as TParent;
                    if (newItem.Sequence <= 0)
                    {
                        newItem.Sequence = this.Children.Count;
                    }
                }
            }
        }
        #endregion Initialization

        #region Iterator
        public SwagItemPreOrderIterator<TParent, TChild> CreateIterator()
        {
            return new SwagItemPreOrderIterator<TParent, TChild>(this as TParent);
        }
        #endregion Iterator
    }

}
