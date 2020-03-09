using Newtonsoft.Json;
using SwagOverflowWPF.Data;
using SwagOverflowWPF.Repository;
using SwagOverflowWPF.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


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

    public class SwagSettingViewModel : SwagIndexedItemViewModel
    {
        #region Private/Protected Members
        SettingType _settingType;
        Enum _icon;
        protected Object _itemsSource;
        protected String _iconString, _iconTypeString, _itemsSourceTypeString;
        #endregion Private/Protected Members

        #region Properties
        #region Icon
        [NotMapped]
        public Enum Icon
        {
            get { return _icon; }
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
            get { return _itemsSource; }
            set { SetValue(ref _itemsSource, value); }
        }
        #endregion ItemsSource
        #region ItemsSourceType
        [NotMapped]
        public virtual Type ItemsSourceType { get { return null; } set { } }
        #endregion ItemsSourceType
        #region ItemsSourceTypeString
        public virtual String ItemsSourceTypeString
        {
            get { return _itemsSourceTypeString; }
            set { SetValue(ref _itemsSourceTypeString, value); }
        }
        #endregion ItemsSourceTypeString
        #endregion Properties

        #region Initialization
        public SwagSettingViewModel() : base()
        {

        }
        #endregion Initialization
    }

    public class SwagSettingViewModel<T> : SwagSettingViewModel
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
        #region ItemsSourceType
        [NotMapped]
        public override Type ItemsSourceType { get { return typeof(T[]); } set { } }
        #endregion ItemsSourceType
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
        public SwagSettingViewModel() : base()
        {

        }

        public SwagSettingViewModel(SwagSettingViewModel swagSetting) : base()
        {
            PropertyCopy.Copy(swagSetting, this);
        }
        #endregion Initialization
    }

    public class SwagSettingGroupViewModel : SwagIndexedGroupViewModel<SwagSettingViewModel>
    {
        #region Initialization
        public SwagSettingGroupViewModel() : base()
        {
            IndexedRootGeneric.SettingType = SettingType.SettingGroup;
        }
        #endregion Initialization
    }
}

