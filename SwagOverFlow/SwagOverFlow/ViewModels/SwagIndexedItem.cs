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
        String _key;
        #endregion Private/Protected Members

        #region Properties
        #region Key
        public String Key
        {
            get { return _key; }
            set { SetValue(ref _key, value); }
        }
        #endregion Key
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

    public abstract class SwagIndexedValueItem<TParent, TChild> : SwagValueItemBase, ISwagIndexedChild<TChild>
    where TChild : class, ISwagIndexedChild<TChild>
    where TParent : class, ISwagParent<TChild>
    {
        #region Private/Protected Members
        TParent _parent;
        Int32? _parentId;
        String _key;
        #endregion Private/Protected Members

        #region Properties
        #region Key
        public String Key
        {
            get { return _key; }
            set { SetValue(ref _key, value); }
        }
        #endregion Key
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
                SwagIndexedValueItem<TParent, TChild> tempNode = this;
                String path = "";
                while (tempNode != null)
                {
                    path = $"{tempNode.Display ?? tempNode.Key}/{path}";
                    tempNode = tempNode.Parent as SwagIndexedValueItem<TParent, TChild>;
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
        public SwagIndexedValueItem()
        {
            PropertyChangedExtended += SwagIndexedItem_PropertyChangedExtended;
        }

        private void SwagIndexedItem_PropertyChangedExtended(object sender, PropertyChangedExtendedEventArgs e)
        {
            Parent?.OnSwagItemChanged(this, e);
        }
        #endregion Initialization
    }

    public abstract class SwagIndexedValueItem<TParent, TChild, T> : SwagIndexedValueItem<TParent, TChild>
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
        public SwagIndexedValueItem() : base()
        {
        }
        #endregion Initialization
    }

    
}
