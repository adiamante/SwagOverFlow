using SwagOverflowWPF.Interface;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Data;

namespace SwagOverflowWPF.ViewModels
{
    public class SwagItemViewModel : ViewModelBase, ISwagItem<SwagItemViewModel>
    {
        #region Private/Protected Members
        String _display, _parentKey, _alternateId;
        Byte[] _data;
        Int32 _itemId, _groupId, _sequence;
        Int32? _parentId, _groupRootId;
        Boolean _isExpanded;
        protected ObservableCollection<SwagItemViewModel> _children = new ObservableCollection<SwagItemViewModel>();
        SwagItemViewModel _parent;
        CollectionViewSource _childrenCollectionViewSource;
        SwagGroupViewModel _group, _groupRoot;
        #endregion Private/Protected Members

        #region Properties
        #region GroupId
        public Int32 GroupId
        {
            get { return _groupId; }
            set { SetValue(ref _groupId, value); }
        }
        #endregion GroupId
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
        #region GroupRootId
        public Int32? GroupRootId
        {
            get { return _groupRootId; }
            set { SetValue(ref _groupRootId, value); }
        }
        #endregion GroupRootId
        #region ParentId
        public Int32? ParentId
        {
            get { return _parentId; }
            set { SetValue(ref _parentId, value); }
        }
        #endregion ParentId
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
        #region Value
        public virtual Object Value { get { return null; } set { } }
        #endregion Value
        #region ValueType
        public virtual Type ValueType { get { return null; } set { } }
        #endregion ValueType
        #region Data
        public virtual Byte[] Data
        {
            get { return _data; }
            set { SetValue(ref _data, value); }
        }
        #endregion Data
        #region Parent
        public virtual SwagItemViewModel Parent
        {
            get { return _parent; }
            set { SetValue(ref _parent, value); }
        }
        #endregion Parent
        #region ChildrenView
        public ICollectionView ChildrenView
        {
            get { return _childrenCollectionViewSource.View; }
        }
        #endregion ChildrenView
        #region Children
        public ObservableCollection<SwagItemViewModel> Children
        {
            get { return _children; }
            set { SetValue(ref _children, value); }
        }
        #endregion Children
        #region Group
        public virtual SwagGroupViewModel Group
        {
            get { return _group; }
            set { SetValue(ref _group, value); }
        }
        #endregion Group
        #region GroupRoot
        public SwagGroupViewModel GroupRoot
        {
            get { return _groupRoot; }
            set { SetValue(ref _groupRoot, value); }
        }
        #endregion GroupRoot
        #endregion Properties

        #region Initialization
        public SwagItemViewModel()
        {
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagItemViewModel child in e.NewItems)
                {
                    child.Parent = this;
                    child.Group = this.Group;
                }
            }
        }
        #endregion Initialization
    }

    public class SwagGroupViewModel : ViewModelBase
    {
        #region Private/Protected Members
        String _name, _display, _alternateId;
        Int32 _groupId;
        Int32? _rootId;
        protected SwagItemViewModel _root;
        protected ObservableCollection<SwagItemViewModel> _descendants = new ObservableCollection<SwagItemViewModel>();
        #endregion Private/Protected Members

        #region Properties
        #region Name
        public String Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }
        #endregion Name
        #region Display
        public String Display
        {
            get { return _display; }
            set { SetValue(ref _display, value); }
        }
        #endregion Display
        #region GroupId
        public Int32 GroupId
        {
            get { return _groupId; }
            set { SetValue(ref _groupId, value); }
        }
        #endregion GroupId
        #region AlternateId
        public String AlternateId
        {
            get { return _alternateId; }
            set { SetValue(ref _alternateId, value); }
        }
        #endregion AlternateId
        #region RootId
        public Int32? RootId
        {
            get { return _rootId; }
            set { SetValue(ref _rootId, value); }
        }
        #endregion RootId
        #region Root
        public SwagItemViewModel Root
        {
            get { return _root; }
            set 
            {
                _root.Group = this;
                SetValue(ref _root, value); 
            }
        }
        #endregion Root
        #region Descendants
        public virtual ObservableCollection<SwagItemViewModel> Descendants
        {
            get { return _descendants; }
            set { SetValue(ref _descendants, value); }
        }
        #endregion Descendants
        #endregion Properties

        #region Initialization
        public SwagGroupViewModel() : base()
        {
            
        }
        #endregion Initialization
    }

    public class SwagGroupViewModel<T> : SwagGroupViewModel where T : SwagItemViewModel, new()
    {
        #region RootGeneric
        [NotMapped]
        public T RootGeneric
        {
            get { return (T)_root; }
            set { SetValue(ref _root, value); }
        }
        #endregion RootGeneric

        #region Initialization
        public SwagGroupViewModel() : base()
        {
            RootGeneric = new T();
            RootGeneric.Group = this;
        }
        #endregion Initialization

    }
}
