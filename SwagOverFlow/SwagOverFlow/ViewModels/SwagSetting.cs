using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Iterator;
using SwagOverFlow.ViewModels;
using SwagOverFlow.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwagOverFlow.ViewModels
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

    public class SwagSetting : SwagIndexedValueItem<SwagSettingGroup, SwagSetting>
    {
        #region Private/Protected Members
        protected JObject _data = new JObject();
        SettingType _settingType;
        Enum _icon, _icon2;
        #endregion Private/Protected Members

        #region Properties
        #region Data
        public JObject Data
        {
            get { return _data; }
            set { SetValue(ref _data, value); }
        }
        #endregion Data
        #region Icon
        [JsonIgnore]
        public Enum Icon
        {
            get 
            {
                if (_icon == null)
                {
                    _icon = IconHelper.ToEnum(_data);
                }
                return _icon;
            }
            set 
            { 
                SetValue(ref _icon, value);
                JObject iconData = IconHelper.ToObject(_icon);
                Data.Merge(iconData);
            }
        }
        #endregion Icon
        #region Icon2
        [JsonIgnore]
        public Enum Icon2
        {
            get
            {
                if (_icon2 == null)
                {
                    _icon2 = IconHelper.ToEnum(_data, "Icon2");
                }
                return _icon2;
            }
            set
            {
                SetValue(ref _icon2, value);
                JObject iconData = IconHelper.ToObject(_icon2, "Icon2");
                Data.Merge(iconData);
            }
        }
        #endregion Icon2
        #region SettingType
        public SettingType SettingType
        {
            get { return _settingType; }
            set { SetValue(ref _settingType, value); }
        }
        #endregion SettingType
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
                if (_itemsSource == null)
                {
                    Type itemsSourceType = JsonConvert.DeserializeObject<Type>(_data["ItemsSource"]["Type"].ToString());
                    _itemsSource = (T[])JsonConvert.DeserializeObject(_data["ItemsSource"]["Items"].ToString(), itemsSourceType);
                }
                return _itemsSource; 
            }
            set 
            { 
                SetValue(ref _itemsSource, value);
                JObject jData = new JObject();
                JObject jItemsSource = new JObject();
                jItemsSource["Type"] = JsonHelper.ToJsonString(typeof(T[]));
                jItemsSource["Items"] = JArray.Parse(JsonHelper.ToJsonString(_itemsSource));
                jData["ItemsSource"] = jItemsSource;
                _data.Merge(jData);
            }
        }
        #endregion ItemsSource
        #endregion Properties

        #region Initialization
        public SwagSetting() : base()
        {
        }
        #endregion Initialization
    }

    public class SwagSettingGroup : SwagSetting, ISwagParent<SwagSetting>
    {
        #region Private/Protected Members
        String _name;
        protected ObservableCollection<SwagSetting> _children = new ObservableCollection<SwagSetting>();
        protected Dictionary<String, SwagSetting> _dict = new Dictionary<string, SwagSetting>();
        #endregion Private/Protected Members

        #region Events
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

        public virtual void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedEventArgs e)
        {
            SwagSetting swagSetting = (SwagSetting)swagItem;
            PropertyChangedExtendedEventArgs exArgs = (PropertyChangedExtendedEventArgs)e;
            SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs()
            {
                SwagItem = swagItem,
                PropertyChangedArgs = e,
                Message = $"{swagSetting.Path}({exArgs.PropertyName})\n\t{exArgs.OldValue} => {exArgs.NewValue}"
            });
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
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagSetting newItem in e.NewItems)
                {
                    newItem.Parent = this;
                    if (newItem.Sequence < 0)
                    {
                        newItem.Sequence = this.Children.Count - 1;
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
        }
        #endregion Initialization

        #region Iterator
        public SwagItemPreOrderIterator<SwagSetting> CreateIterator()
        {
            return new SwagItemPreOrderIterator<SwagSetting>(this);
        }
        #endregion Iterator

        #region Methods
        public Boolean ContainsKey(String key)
        {
            return _dict.ContainsKey(key);
        }
        #endregion Methods
    }

    public class SwagSettingInt : SwagSetting<Int32>
    {

    }

    public class SwagSettingString : SwagSetting<String>
    {

    }

    public class SwagSettingBoolean : SwagSetting<Boolean>
    {

    }

    public static class SwagSettingExtensions
    {
        public static void TryAddChildSetting(this SwagSetting setting, String key, SwagSetting child)
        {
            if (setting is SwagSettingGroup grp)
            {
                if (!grp.ContainsKey(key))
                {
                    grp[key] = child;
                    child.ValueTypeString = child.ValueTypeString;
                    child.ObjValue = child.ObjValue;
                    child.Data = child.Data;
                }
            }
            else
            {
                throw new InvalidOperationException($"Cannot add child to type {setting.GetType()}");
            }
        }
    }

}

