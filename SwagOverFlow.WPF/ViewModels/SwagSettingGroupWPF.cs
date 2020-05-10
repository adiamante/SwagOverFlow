using Newtonsoft.Json;
using SwagOverFlow.Iterator;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Data;

namespace SwagOverFlow.WPF.ViewModels
{
    public class SwagSettingWPFGroup : SwagSettingGroup
    {
        #region Private/Protected Members
        CollectionViewSource _childrenCollectionViewSource;
        #endregion Private/Protected Members

        #region Properties
        #region ChildrenView
        [JsonIgnore]
        [NotMapped]
        public ICollectionView ChildrenView
        {
            get { return _childrenCollectionViewSource.View; }
        }
        #endregion ChildrenView
        #endregion Properties

        #region Initialization
        public SwagSettingWPFGroup() : base()
        {
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
            _children.CollectionChanged += _children_CollectionChanged;
        }

        public SwagSettingWPFGroup(SwagSettingGroup swagSettingGroup) : this()
        {
            PropertyCopy.Copy(swagSettingGroup, this);
            foreach (SwagSetting child in Children)
            {
                if (!_dict.ContainsKey(child.Key))
                {
                    _dict.Add(child.Key, child);
                }
            }
            _childrenCollectionViewSource.Source = _children;
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _childrenCollectionViewSource.View.Refresh();
        }
        #endregion Initialization
    }
}

