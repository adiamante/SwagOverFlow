using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverflowWPF.Utilities;
using System;
using System.ComponentModel.DataAnnotations.Schema;


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

    public class SwagSetting : SwagIndexedItem
    {
        #region Private/Protected Members
        SettingType _settingType;
        Enum _icon;
        protected Object _itemsSource;
        protected String _iconString, _iconTypeString, _itemsSourceTypeString;
        #endregion Private/Protected Members

        #region Properties
        #region Icon
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
        #region ItemsSource
        public virtual Object ItemsSource
        {
            get 
            {
                if (_itemsSource != null && _itemsSource is JArray && !String.IsNullOrEmpty(_itemsSourceTypeString))
                {
                    Type itemsSourceType = JsonConvert.DeserializeObject<Type>(_itemsSourceTypeString);
                    _itemsSource = JsonConvert.DeserializeObject(_itemsSource.ToString(), itemsSourceType);
                }
                return _itemsSource; 
            }
            set { SetValue(ref _itemsSource, value); }
        }
        #endregion ItemsSource
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
        T[] _genericItemsSource;
        #endregion Private/Protected Members

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
        #region ItemsSourceTypeString
        public override String ItemsSourceTypeString
        {
            get { return JsonHelper.ToJsonString(typeof(T[])); }
            set { SetValue(ref _itemsSourceTypeString, value); }
        }
        #endregion ItemsSourceTypeString
        #region GenericItemsSource
        public T[] GenericItemsSource
        {
            get { return _genericItemsSource; }
            set 
            {
                ItemsSource = value;
                SetValue(ref _genericItemsSource, value); 
            }
        }
        #endregion GenericItemsSource
        #endregion Properties

        #region Initialization
        public SwagSetting() : base()
        {

        }

        public SwagSetting(SwagSetting swagSetting) : base()
        {
            PropertyCopy.Copy(swagSetting, this);
        }
        #endregion Initialization
    }

    public class SwagSettingGroup : SwagIndexedGroup<SwagSetting>
    {
        #region Initialization
        public SwagSettingGroup() : base()
        {
            IndexedRootGeneric.SettingType = SettingType.SettingGroup;
        }
        #endregion Initialization
    }

    public class SwagSettingString : SwagSetting<String>
    {

    }

    public class SwagSettingBoolean : SwagSetting<Boolean>
    {

    }

}

