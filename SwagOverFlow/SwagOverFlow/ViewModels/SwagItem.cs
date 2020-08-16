using Newtonsoft.Json;
using SwagOverFlow.Utils;
using SwagOverFlow.Iterator;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace SwagOverFlow.ViewModels
{
    #region Interfaces
    public interface ISwagChild<TChild> where TChild : class, ISwagChild<TChild>
    {
        ISwagParent<TChild> Parent { get; set; }
        String Display { get; set; }        //Used for debugging
        Int32 Sequence { get; set; }
    }

    public interface ISwagParent<TChild> : ISwagChild<TChild>, ISwagItemChanged where TChild : class, ISwagChild<TChild>
    {
        ObservableCollection<TChild> Children { get; set; }
    }

    public interface ISwagItemChanged
    {
        event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;
        void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedEventArgs e);
    }
    #endregion Interfaces

    #region SwagItemChangedEventArgs
    public class SwagItemChangedEventArgs : EventArgs
    {
        public SwagItemBase SwagItem { get; set; }
        public PropertyChangedEventArgs PropertyChangedArgs { get; set; }
        public String Message { get; set; }
        
    }
    #endregion SwagItemChangedEventArgs

    public abstract class SwagItemBase : ViewModelBaseExtended
    {
        #region Private/Protected Members
        protected String _display, _alternateId;
        protected Type _valueType = null;
        protected String _valueTypeString;
        Int32 _itemId, _sequence = -1;
        Boolean _isExpanded, _isSelected, _canUndo = true;
        protected Object _objValue;
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
        #region IsSelected
        public Boolean IsSelected
        {
            get { return _isSelected; }
            set { SetValue(ref _isSelected, value); }
        }
        #endregion IsSelected
        #endregion Properties
    }

    public abstract class SwagItem<TParent, TChild> : SwagItemBase, ISwagChild<TChild>
        where TChild : class, ISwagChild<TChild>
        where TParent : class, ISwagParent<TChild>
    {
        #region Private/Protected Members
        protected TParent _parent;
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

        ISwagParent<TChild> ISwagChild<TChild>.Parent
        {
            get { return (ISwagParent<TChild>)_parent; }
            set { SetValue(ref _parent, (TParent)value); }
        }
        #endregion Parent
        #endregion Properties

        #region Initialization
        public SwagItem()
        {
            PropertyChangedExtended += SwagItem_PropertyChangedExtended;
        }

        private void SwagItem_PropertyChangedExtended(object sender, PropertyChangedExtendedEventArgs e)
        {
            Parent?.OnSwagItemChanged(this, e);
        }
        #endregion Initialization
    }

    public abstract class SwagValueItemBase : SwagItemBase
    {
        #region Properties
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
                    else if (!ValueType.IsAssignableFrom(_objValue.GetType()))
                    {
                        //_objValue = JsonConvert.DeserializeObject(_objValue.ToString(), ValueType);
                        _objValue = JsonHelper.ToObject(_objValue.ToString(), ValueType);
                    }
                }
                return _objValue;
            }
            set { SetValue(ref _objValue, value, false); }
        }
        #endregion ObjValue
        #endregion Properties

        #region Methods
        public T GetValue<T>()
        {
            return (T)ObjValue;
        }

        public virtual void SetValue<T>(T val)
        {
            ObjValue = val;
            OnPropertyChanged("Value");
        }
        #endregion Methods
    }


    public abstract class SwagValueItem<TParent, TChild> : SwagValueItemBase, ISwagChild<TChild>
        where TChild : class, ISwagChild<TChild>
        where TParent : class, ISwagParent<TChild>
    {
        #region Private/Protected Members
        protected TParent _parent;
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

        ISwagParent<TChild> ISwagChild<TChild>.Parent 
        {
            get { return (ISwagParent<TChild>)_parent;  } 
            set { SetValue(ref _parent, (TParent)value); }
        }
        #endregion Parent
        #endregion Properties

        #region Initialization
        public SwagValueItem()
        {
            PropertyChangedExtended += SwagItem_PropertyChangedExtended;
        }

        private void SwagItem_PropertyChangedExtended(object sender, PropertyChangedExtendedEventArgs e)
        {
            Parent?.OnSwagItemChanged(this, e);
        }
        #endregion Initialization
    }

    public abstract class SwagValueItem<TParent, TChild, T> : SwagValueItem<TParent, TChild>
        where TChild : class, ISwagChild<TChild>
        where TParent : class, ISwagParent<TChild>
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
        public SwagValueItem() : base()
        {
        }
        #endregion Initialization
    }

    public class SwagValueItem<T> : SwagValueItem<SwagValueItemGroup<T>, SwagValueItem<T>, T>
    {

    }

    public class SwagValueItemGroup<T> : SwagValueItem<T>, ISwagParent<SwagValueItem<T>>
    {
        #region Private/Protected Members
        String _name;
        protected ObservableCollection<SwagValueItem<T>> _children = new ObservableCollection<SwagValueItem<T>>();
        #endregion Private/Protected Members

        #region Events
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

        public virtual void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedEventArgs e)
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
        #region Children
        public ObservableCollection<SwagValueItem<T>> Children
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
        public SwagValueItemGroup() : base()
        {
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagValueItem<T> newItem in e.NewItems)
                {
                    newItem.Parent = this;
                    if (newItem.Sequence <= 0)
                    {
                        newItem.Sequence = this.Children.Count;
                    }
                }
            }
        }
        #endregion Initialization

        #region Iterator
        public SwagItemPreOrderIterator<SwagValueItem<T>> CreateIterator()
        {
            return new SwagItemPreOrderIterator<SwagValueItem<T>>(this);
        }
        #endregion Iterator
    }
}
