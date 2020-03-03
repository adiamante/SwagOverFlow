using SwagOverflowWPF.Interface;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace SwagOverflowWPF.ViewModels
{
    public class SwagItemViewModel : ViewModelBase, iSwagItem<SwagItemViewModel>
    {
        #region Private/Protected Members
        String _group, _key, _display;
        Byte[] _data;
        Int32 _id;
        Int32? _parentId;
        protected ObservableCollection<SwagItemViewModel> _children = new ObservableCollection<SwagItemViewModel>();
        SwagItemViewModel _parent;
        CollectionViewSource _childrenCollectionViewSource;
        #endregion Private/Protected Members

        #region Group
        public String Group
        {
            get { return _group; }
            set { SetValue(ref _group, value); }
        }
        #endregion Group
        #region Key
        public String Key
        {
            get { return _key; }
            set { SetValue(ref _key, value); }
        }
        #endregion Key
        #region Id
        public Int32 Id
        {
            get { return _id; }
            set { SetValue(ref _id, value); }
        }
        #endregion Id
        #region ParentId
        public Int32? ParentId
        {
            get { return _parentId; }
            set { SetValue(ref _parentId, value); }
        }
        #endregion ParentId
        #region Display
        public String Display
        {
            get { return _display; }
            set { SetValue(ref _display, value); }
        }
        #endregion Display
        #region Data
        public Byte[] Data
        {
            get { return _data; }
            set { SetValue(ref _data, value); }
        }
        #endregion Data
        #region Parent
        public SwagItemViewModel Parent
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

        public SwagItemViewModel()
        {
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
        }
    }
}
