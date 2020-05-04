using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverflowWPF.Collections;
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
    #region SettingType
    public enum SettingType
    {
        Normal,
        DropDown,
        ReadOnly,
        Hidden,
        EnumFlag,
        FolderPath,
        FilePath,
        SettingGroup,
        SwagListView,
        SwagTreeView
    }
    #endregion SettingType

    public class SwagSetting : SwagIndexedItem<SwagSettingGroup, SwagSetting>
    {
        #region Private/Protected Members
        SettingType _settingType;
        Enum _icon;
        protected String _iconString, _iconTypeString, _itemsSourceTypeString;
        protected Object _objItemsSource;
        #endregion Private/Protected Members

        #region Properties
        #region Icon
        [JsonIgnore]
        public Enum Icon
        {
            get 
            {
                if (_icon == null && !String.IsNullOrEmpty(_iconTypeString) && !String.IsNullOrEmpty(_iconTypeString))
                {
                    Type iconType = JsonConvert.DeserializeObject<Type>(_iconTypeString);
                    _icon = (Enum)Enum.Parse(iconType, _iconString);
                }
                return _icon; 
            }
            set { SetValue(ref _icon, value); }
        }
        #endregion Icon
        #region IconString
        public String IconString
        {
            get 
            {
                if (_icon != null)
                {
                    return _icon.ToString();
                }
                return _iconString; 
            }
            set { SetValue(ref _iconString, value); }
        }
        #endregion IconString
        #region IconTypeString
        public String IconTypeString
        {
            get 
            {
                if (_icon != null)
                {
                    return JsonHelper.ToJsonString(_icon.GetType());
                }
                return _iconTypeString;
            }
            set { SetValue(ref _iconTypeString, value); }
        }
        #endregion IconTypeString
        #region SettingType
        public SettingType SettingType
        {
            get { return _settingType; }
            set { SetValue(ref _settingType, value); }
        }
        #endregion SettingType
        #region ObjItemsSource
        public virtual Object ObjItemsSource
        {
            get { return _objItemsSource; }
            set { SetValue(ref _objItemsSource, value); }
        }
        #endregion ObjItemsSource
        #region ItemsSourceTypeString
        public virtual String ItemsSourceTypeString
        {
            get { return _itemsSourceTypeString; }
            set { SetValue(ref _itemsSourceTypeString, value); }
        }
        #endregion ItemsSourceTypeString
        #endregion Properties

        #region Initialization
        public SwagSetting() : base()
        {

        }
        #endregion Initialization
    }

    public class SwagSetting<T> : SwagSetting
    {
        #region Private/Protected Members
        protected T _value;
        protected T[] _itemsSource;
        #endregion Private/Protected Members

        #region Properties
        #region Value
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

                if (_objValue != null && (_value == null || !_value.Equals(_objValue)))
                {
                    _value = (T)_objValue;
                }

                return _value;
            }
            set
            {
                _objValue = value;
                OnPropertyChanged("ObjValue");      //Normal (if this was extended SwagCommandManager detects double)
                SetValue(ref _value, value);        //Extended
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
        #region ItemsSource
        public T[] ItemsSource
        {
            get 
            {
                if (_objItemsSource != null && _objItemsSource is JArray && !String.IsNullOrEmpty(_itemsSourceTypeString))
                {
                    Type itemsSourceType = JsonConvert.DeserializeObject<Type>(_itemsSourceTypeString);
                    _objItemsSource = JsonConvert.DeserializeObject(_objItemsSource.ToString(), itemsSourceType);
                    _itemsSource = (T[])_objItemsSource;
                }
                return _itemsSource; 
            }
            set 
            { 
                SetValue(ref _itemsSource, value);
                SetValue(ref _objItemsSource, value, "ObjItemsSource");
            }
        }
        #endregion ItemsSource
        #region ItemsSourceTypeString
        public override String ItemsSourceTypeString
        {
            get { return JsonHelper.ToJsonString(typeof(T[])); ; }
            set { SetValue(ref _itemsSourceTypeString, value); }
        }
        #endregion ItemsSourceTypeString
        #endregion Properties

        #region Initialization
        public SwagSetting() : base()
        {
        }
        #endregion Initialization
    }

    public class SwagSettingGroup : SwagSetting, ISwagParent<SwagSettingGroup, SwagSetting>
    {
        #region Private/Protected Members
        String _name;
        CollectionViewSource _childrenCollectionViewSource;
        protected ObservableCollection<SwagSetting> _children = new ObservableCollection<SwagSetting>();
        Dictionary<String, SwagSetting> _dict = new Dictionary<string, SwagSetting>();
        #endregion Private/Protected Members

        #region Events
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

        public virtual void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedExtendedEventArgs e)
        {
            SwagSetting swagSetting = (SwagSetting)swagItem;
            SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs() { SwagItem = swagItem, PropertyChangedArgs = e, Message = $"{swagSetting.Path}({e.OldValue}) => {e.NewValue}" });
            Parent?.OnSwagItemChanged(swagItem, e);
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
        public ObservableCollection<SwagSetting> Children
        {
            get { return _children; }
            set { SetValue(ref _children, value); }
        }
        #endregion Children
        #region ChildrenView
        [JsonIgnore]
        public ICollectionView ChildrenView
        {
            get { return _childrenCollectionViewSource.View; }
        }
        #endregion ChildrenView
        #region HasChildren
        public Boolean HasChildren
        {
            get { return _children.Count > 0; }
        }
        #endregion HasChildren
        #region Indexer
        public override SwagSetting this[String key]
        {
            get
            {
                if (!_dict.ContainsKey(key))
                {
                    SwagSettingGroup child = (SwagSettingGroup)Activator.CreateInstance(this.GetType());
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
        public SwagSettingGroup() : base()
        {
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagSetting newItem in e.NewItems)
                {
                    newItem.Parent = this;
                    if (newItem.Sequence <= 0)
                    {
                        newItem.Sequence = this.Children.Count;
                    }
                    if (_dict.ContainsKey(newItem.Key))
                    {
                        _dict[newItem.Key] = newItem;
                    }
                    else
                    {
                        _dict.Add(newItem.Key, newItem);
                    }
                    
                }
            }

            if (e.OldItems != null)
            {
                foreach (SwagSetting oldItems in e.OldItems)
                {
                    _dict.Remove(oldItems.Key);
                }
            }

            _childrenCollectionViewSource.View.Refresh();
        }
        #endregion Initialization

        #region Iterator
        public SwagItemPreOrderIterator<SwagSettingGroup, SwagSetting> CreateIterator()
        {
            return new SwagItemPreOrderIterator<SwagSettingGroup, SwagSetting>(this);
        }
        #endregion Iterator
    }

    public class SwagSettingString : SwagSetting<String>
    {

    }

    public class SwagSettingBoolean : SwagSetting<Boolean>
    {

    }

}

