using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

    public class SwagSettingViewModel : SwagItemViewModel
    {
        #region Private Members
        Dictionary<String, SwagSettingViewModel> _dict = new Dictionary<string, SwagSettingViewModel>();
        SettingType _settingType;
        Boolean _isExpanded;
        #endregion Private Members

        #region SettingType
        public SettingType SettingType
        {
            get { return _settingType; }
            set { SetValue(ref _settingType, value); }
        }
        #endregion SettingType
        #region IsExpanded
        public Boolean IsExpanded
        {
            get { return _isExpanded; }
            set { SetValue(ref _isExpanded, value); }
        }
        #endregion IsExpanded
        #region HasChildren
        public Boolean HasChildren
        {
            get { return _children.Count > 0; }
        }
        #endregion HasChildren

        #region Initialization
        public SwagSettingViewModel() : base()
        {
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagSettingViewModel newItem in e.NewItems)
                {
                    _dict.Add(newItem.Key, newItem);
                }
            }
            
            if (e.OldItems != null)
            {
                foreach (SwagSettingViewModel oldItem in e.OldItems)
                {
                    _dict.Remove(oldItem.Key);
                }
            }
        }
        #endregion Initialization

        #region Indexer
        public SwagSettingViewModel this[String key]
        {
            get
            {
                if (!_dict.ContainsKey(key))
                {
                    _children.Add(new SwagSettingViewModel<String>() { Key = key, Display = key, SettingType = SettingType.SettingGroup });
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

    public class SwagSettingViewModel<T> : SwagSettingViewModel
    {
        #region Private Members
        T _value;
        T[] _itemsSource;
        #endregion Private Members

        #region ValueType
        public override Type ValueType { get { return typeof(T); } }
        #endregion ValueType

        #region Value
        public override object Value
        {
            get { return (object)_value; }
            set { SetValue<T>(ref _value, (T)value); }
        }
        #endregion Value

        #region GenericValue
        public T GenericValue
        {
            get { return (T)_value; }
            set { SetValue<T>(ref _value, (T)value); }
        }
        #endregion GenericValue

        #region ItemsSource
        public T[] ItemsSource
        {
            get { return _itemsSource; }
            set { SetValue(ref _itemsSource, value); }
        }
        #endregion ItemsSource
    }

    public class SwagSettingCollection : SwagSettingViewModel
    {
        public SwagSettingCollection()
        {
            SettingType = SettingType.SettingGroup;
        }
    }
}

