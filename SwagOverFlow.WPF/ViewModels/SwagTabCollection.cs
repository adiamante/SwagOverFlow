using Newtonsoft.Json;
using SwagOverflow.Iterator;
using SwagOverflow.ViewModels;
using SwagOverFlow.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Data;

namespace SwagOverflow.WPF.ViewModels
{
    public class SwagTabItem : SwagIndexedItem<SwagTabCollection, SwagTabItem>
    {
        #region Private/Protected Members
        Enum _icon;
        INotifyPropertyChanged _viewModel;
        protected String _iconString, _iconTypeString;
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

    public class SwagTabCollection : SwagTabItem, ISwagParent<SwagTabItem>
    {
        #region Private/Protected Members
        Boolean _showChildText = false, _isInitialized = false;
        Int32 _selectedIndex = -1;
        protected ObservableCollection<SwagTabItem> _children = new ObservableCollection<SwagTabItem>();
        Dictionary<String, SwagTabItem> _dict = new Dictionary<string, SwagTabItem>();
        CollectionViewSource _childrenCollectionViewSource;
        #endregion Private/Protected Members

        #region Events
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

        public virtual void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedExtendedEventArgs e)
        {
            SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs() { SwagItem = swagItem, PropertyChangedArgs = e });
            Parent?.OnSwagItemChanged(swagItem, e);
        }
        #endregion Events

        #region Properties
        #region ChildrenView
        [JsonIgnore]
        public ICollectionView ChildrenView
        {
            get { return _childrenCollectionViewSource.View; }
        }
        #endregion ChildrenView
        #region Children
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
        public SwagTabCollection()
        {
            _children.CollectionChanged += _children_CollectionChanged;
            this.PropertyChanged += SwagTabCollection_PropertyChanged;
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
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
