using Newtonsoft.Json;
using SwagOverFlow.Iterator;
using SwagOverFlow.ViewModels;
using SwagOverFlow.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json.Linq;

namespace SwagOverFlow.ViewModels
{
    public class SwagTabItem : SwagIndexedItem<SwagTabGroup, SwagTabItem>
    {
        #region Private/Protected Members
        JObject _data = new JObject();
        Enum _icon, _icon2;
        INotifyPropertyChanged _viewModel;
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
        #region ShowText
        public bool ShowText
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.ShowChildText;
                }
                return true;
            }
        }
        #endregion ShowText
        #region ViewModel
        [NotMapped]
        [JsonIgnore]
        public INotifyPropertyChanged ViewModel
        {
            get { return _viewModel; }
            set { SetValue(ref _viewModel, value); }
        }
        #endregion ViewModel
        #endregion Properties

        #region Methods
        public void InvokePropertyChanged(String property)
        {
            OnPropertyChanged(property);
        }
        #endregion Methods
    }

    public class SwagTabGroup : SwagTabItem, ISwagParent<SwagTabItem>
    {
        #region Private/Protected Members
        Boolean _showChildText = false, _isInitialized = false;
        Int32 _selectedIndex = -1;
        protected ObservableCollection<SwagTabItem> _children = new ObservableCollection<SwagTabItem>();
        Dictionary<String, SwagTabItem> _dict = new Dictionary<string, SwagTabItem>();
        #endregion Private/Protected Members

        #region Events
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

        public virtual void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedEventArgs e)
        {
            SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs() { SwagItem = swagItem, PropertyChangedArgs = e });
            Parent?.OnSwagItemChanged(swagItem, e);
        }
        #endregion Events

        #region Properties
        #region Children
        [NotMapped]
        public ObservableCollection<SwagTabItem> Children
        {
            get { return _children; }
            set { SetValue(ref _children, value); }
        }
        #endregion Children
        #region Dictionary
        [JsonIgnore]
        [NotMapped]
        public Dictionary<String, SwagTabItem> Dictionary
        {
            get { return _dict; }
        }
        #endregion Dictionary
        #region HasChildren
        [JsonIgnore]
        [NotMapped]
        public Boolean HasChildren
        {
            get { return _children.Count > 0; }
        }
        #endregion HasChildren
        #region Indexer
        public override SwagTabItem this[String key]
        {
            get
            {
                if (!_dict.ContainsKey(key))
                {
                    SwagTabItem child = (SwagTabItem)Activator.CreateInstance(this.GetType());
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
        #region IsInitialized
        public Boolean IsInitialized
        {
            get { return _isInitialized; }
            set { SetValue(ref _isInitialized, value); }
        }
        #endregion IsInitialized
        #region ShowChildText
        public Boolean ShowChildText
        {
            get { return _showChildText; }
            set { SetValue(ref _showChildText, value); }
        }
        #endregion ShowChildText
        #region SelectedIndex
        public Int32 SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_isInitialized)
                {
                    SetValue(ref _selectedIndex, value);
                }
                else
                {
                    SetValue(ref _selectedIndex, -1);
                }
            }
        }
        #endregion SelectedIndex
        #endregion Properties

        #region Initialization
        public SwagTabGroup()
        {
            _children.CollectionChanged += _children_CollectionChanged;
            this.PropertyChanged += SwagTabCollection_PropertyChanged;
        }

        private void SwagTabCollection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShowChildText")
            {
                if (ShowChildText && SelectedIndex != -1)
                {
                    SelectedIndex = -1;
                    ShowChildText = false;
                }
            }

            foreach (SwagTabItem swagTabItem in _children)
            {
                swagTabItem.InvokePropertyChanged("ShowText");
            }
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagTabItem newItem in e.NewItems)
                {
                    newItem.Parent = this;
                    if (newItem.Sequence <= 0)
                    {
                        newItem.Sequence = this.Children.Count;
                    }
                    _dict.Add(newItem.Key, newItem);
                }
            }

            if (e.OldItems != null)
            {
                foreach (SwagTabItem oldItems in e.OldItems)
                {
                    _dict.Remove(oldItems.Key);
                }
            }
        }
        #endregion Initialization

        #region Iterator
        public SwagItemPreOrderIterator<SwagTabItem> CreateIterator()
        {
            return new SwagItemPreOrderIterator<SwagTabItem>(this);
        }
        #endregion Iterator
    }
}
