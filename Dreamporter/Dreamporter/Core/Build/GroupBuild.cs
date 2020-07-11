using Newtonsoft.Json;
using SwagOverFlow.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Dreamporter.Core
{
    [JsonObject]
    public class GroupBuild : Build, ISwagParent<Build>, ICollection<Build>
    {
        #region Private/Protected Members
        protected ObservableCollection<Build> _children = new ObservableCollection<Build>();
        #endregion Private/Protected Members

        #region Properties
        #region Type
        public override Type Type { get { return typeof(GroupBuild); } }
        #endregion Type
        #region Children
        public ObservableCollection<Build> Children
        {
            get { return _children; }
            set { SetValue(ref _children, value); }
        }

        public int Count => ((ICollection<Build>)_children).Count;

        public bool IsReadOnly => ((ICollection<Build>)_children).IsReadOnly;
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
                foreach (Build item in e.NewItems)
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
                foreach (Build item in e.OldItems)
                {
                    foreach (Build sibling in Children)
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

        #region ICollection
        public void Add(Build item)
        {
            ((ICollection<Build>)_children).Add(item);
        }

        public void Clear()
        {
            ((ICollection<Build>)_children).Clear();
        }

        public bool Contains(Build item)
        {
            return ((ICollection<Build>)_children).Contains(item);
        }

        public void CopyTo(Build[] array, int arrayIndex)
        {
            ((ICollection<Build>)_children).CopyTo(array, arrayIndex);
        }

        public bool Remove(Build item)
        {
            return ((ICollection<Build>)_children).Remove(item);
        }

        public IEnumerator<Build> GetEnumerator()
        {
            return ((IEnumerable<Build>)_children).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_children).GetEnumerator();
        }
        #endregion ICollection

        #region Methods
        #endregion Methods
    }
}
