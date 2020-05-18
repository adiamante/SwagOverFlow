using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwagOverFlow.ViewModels
{
    #region Interfaces
    public interface ISwagIndexedChild<TChild> : ISwagChild<TChild> where TChild : class, ISwagIndexedChild<TChild>
    {
        String Key { get; set; }
    }
    #endregion Interfaces

    public abstract class SwagIndexedItem<TParent, TChild> : SwagItemBase, ISwagIndexedChild<TChild>
        where TChild : class, ISwagIndexedChild<TChild>
        where TParent : class, ISwagParent<TChild>
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

        ISwagParent<TChild> ISwagChild<TChild>.Parent
        {
            get { return (ISwagParent<TChild>)_parent; }
            set { SetValue(ref _parent, (TParent)value); }
        }
        #endregion Parent
        #region Path
        public String Path
        {
            get
            {
                SwagIndexedItem<TParent, TChild> tempNode = this;
                String path = "";
                while (tempNode != null)
                {
                    path = $"{tempNode.Display ?? tempNode.Key}/{path}";
                    tempNode = tempNode.Parent as SwagIndexedItem<TParent, TChild>;
                }
                return path.Trim('/');
            }
        }
        #endregion Path
        #region Indexer
        //This is here for transparency
        public virtual TChild this[String key]
        {
            get
            {
                throw new InvalidOperationException("SwagIndexedItem type does not support indexing");
            }
            set
            {
                throw new InvalidOperationException("SwagIndexedItem type does not support indexing");
            }
        }
        #endregion Indexer
        #endregion Properties

        #region Initialization
        public SwagIndexedItem()
        {
            PropertyChangedExtended += SwagIndexedItem_PropertyChangedExtended;
        }

        private void SwagIndexedItem_PropertyChangedExtended(object sender, PropertyChangedExtendedEventArgs e)
        {
            Parent?.OnSwagItemChanged(this, e);
        }
        #endregion Initialization
    }

    public abstract class SwagIndexedItem<TParent, TChild, T> : SwagIndexedItem<TParent, TChild>
        where TChild : class, ISwagIndexedChild<TChild>
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
        #endregion Properties

        #region Initialization
        public SwagIndexedItem() : base()
        {
        }
        #endregion Initialization
    }

    #region SwagIndexedItemGroup (OLD)
    //public abstract class SwagIndexedItemGroup<TChild> : SwagIndexedItem<SwagIndexedItemGroup<TChild>, TChild>, ISwagParent<TChild>
    //    where TChild : class, ISwagIndexedChild<TChild>
    //{
    //    #region Private/Protected Members
    //    String _name;
    //    protected ObservableCollection<TChild> _children = new ObservableCollection<TChild>();
    //    Dictionary<String, TChild> _dict = new Dictionary<string, TChild>();
    //    CollectionViewSource _childrenCollectionViewSource;
    //    #endregion Private/Protected Members

    //    #region Events
    //    public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

    //    public virtual void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedExtendedEventArgs e)
    //    {
    //        SwagIndexedItem<SwagIndexedItemGroup<TChild>, TChild> swagSetting = (SwagIndexedItem<SwagIndexedItemGroup<TChild>, TChild>)swagItem;
    //        SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs() { SwagItem = swagItem, PropertyChangedArgs = e, Message = $"{swagSetting.Path}({e.OldValue}) => {e.NewValue}" });
    //        Parent?.OnSwagItemChanged(swagItem, e);
    //    }
    //    #endregion Events

    //    #region Properties
    //    #region Name
    //    public String Name
    //    {
    //        get { return _name; }
    //        set { SetValue(ref _name, value); }
    //    }
    //    #endregion Name
    //    #region ChildrenView
    //    public ICollectionView ChildrenView
    //    {
    //        get { return _childrenCollectionViewSource.View; }
    //    }
    //    #endregion ChildrenView
    //    #region Children
    //    public ObservableCollection<TChild> Children
    //    {
    //        get { return _children; }
    //        set { SetValue(ref _children, value); }
    //    }
    //    #endregion Children
    //    #region HasChildren
    //    public Boolean HasChildren
    //    {
    //        get { return _children.Count > 0; }
    //    }
    //    #endregion HasChildren
    //    #region Indexer
    //    public override TChild this[String key]
    //    {
    //        get 
    //        {
    //            if (!_dict.ContainsKey(key))
    //            {
    //                TChild child = (TChild)Activator.CreateInstance(this.GetType());
    //                child.Key = child.Display = key;
    //                _children.Add(child);
    //            }
    //            return _dict[key]; 
    //        }
    //        set
    //        {
    //            if (!_dict.ContainsKey(key))
    //            {
    //                value.Display = value.Key = key;
    //                _children.Add(value);
    //            }
    //            _dict[key] = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //    #endregion Indexer
    //    #endregion Properties

    //    #region Initialization
    //    public SwagIndexedItemGroup() : base()
    //    {
    //        _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
    //        _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
    //        _children.CollectionChanged += _children_CollectionChanged;
    //    }

    //    private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //    {
    //        if (e.NewItems != null)
    //        {
    //            foreach (TChild newItem in e.NewItems)
    //            {
    //                newItem.Parent = this;
    //                if (newItem.Sequence <= 0)
    //                {
    //                    newItem.Sequence = this.Children.Count;
    //                }
    //                _dict.Add(newItem.Key, newItem);
    //            }
    //        }

    //        if (e.OldItems != null)
    //        {
    //            foreach (TChild oldItems in e.OldItems)
    //            {
    //                _dict.Remove(oldItems.Key);
    //            }
    //        }

    //        _childrenCollectionViewSource.View.Refresh();
    //    }
    //    #endregion Initialization

    //    #region Iterator
    //    public SwagItemPreOrderIterator<TChild> CreateIterator()
    //    {
    //        return new SwagItemPreOrderIterator<TChild>(this);
    //    }
    //    #endregion Iterator
    //}
    #endregion SwagIndexedItemGroup (OLD)
}
