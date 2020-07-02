using SwagOverFlow.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Dreamporter.Core
{
    public class GroupBuild : BaseBuild, ISwagParent<BaseBuild>
    {
        #region Private/Protected Members
        protected ObservableCollection<BaseBuild> _children = new ObservableCollection<BaseBuild>();
        #endregion Private/Protected Members

        #region Properties
        #region Type
        public override Type Type { get { return typeof(GroupBuild); } }
        #endregion Type
        #region Children
        public ObservableCollection<BaseBuild> Children
        {
            get { return _children; }
            set { SetValue(ref _children, value); }
        }
        #endregion Children
        #endregion Properties

        #region SwagItemChanged
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

        public void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedExtendedEventArgs e)
        {
            SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs() { SwagItem = swagItem, PropertyChangedArgs = e });
            Parent?.OnSwagItemChanged(swagItem, e);
        }
        #endregion SwagItemChanged

        #region Initialization
        public GroupBuild() : base()
        {
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (BaseBuild item in e.NewItems)
                {
                    item.Parent = this;
                    if (item.Sequence < 0)
                    {
                        item.Sequence = this.Children.Count - 1;
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (BaseBuild item in e.OldItems)
                {
                    foreach (BaseBuild sibling in Children)
                    {
                        if (item.Sequence < sibling.Sequence)
                        {
                            sibling.Sequence--;
                        }
                    }
                }
            }

            OnPropertyChanged("HasChildren");

            //TODO: This probably get's called twice when adding so need to check on that sometime
            OnSwagItemChanged(this, new PropertyChangedExtendedEventArgs("Children", Children, null, null));
        }
        #endregion Initialization

        #region Events
        #endregion Events

        #region Methods
        #endregion Methods
    }

    public class CoreGroupBuild : GroupBuild { }
    public class ProfileGroupBuild : GroupBuild { }
}
