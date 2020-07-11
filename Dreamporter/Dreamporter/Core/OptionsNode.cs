using Newtonsoft.Json;
using SwagOverFlow.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace Dreamporter.Core
{
    [JsonObject]
    public class OptionsNode : SwagItemBase, ICollection<OptionsNode>
    {
        #region Private Members
        String _name;
        SwagOptionGroup _options = new SwagOptionGroup();
        OptionsNode _parent = null;
        ObservableCollection<OptionsNode> _children;
        #endregion Private Members

        #region Name
        public String Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }
        #endregion Name
        #region Options
        public SwagOptionGroup Options
        {
            get { return _options; }
            set { SetValue(ref _options, value); }
        }
        #endregion Options
        #region Parent
        public OptionsNode Parent
        {
            get { return _parent; }
            set { SetValue(ref _parent, value); }
        }
        #endregion Parent
        #region Children
        public ObservableCollection<OptionsNode> Children
        {
            get 
            {
                if (_children == null)
                {
                    _children = new ObservableCollection<OptionsNode>();
                    _children.CollectionChanged += _children_CollectionChanged;
                }
                return _children; 
            }
            set 
            { 
                SetValue(ref _children, value);
                if (_children != null)
                {
                    _children.CollectionChanged += _children_CollectionChanged;
                }
            }
        }

        public int Count => ((ICollection<OptionsNode>)_children).Count;

        public bool IsReadOnly
        {
            get
            {
                if (_children == null)
                {
                    return true;
                }
                return ((ICollection<OptionsNode>)_children).IsReadOnly;
            }
        }
        #endregion Children

        #region Initialization
        public OptionsNode()
        {
            if (_children != null)
            {
                _children.CollectionChanged += _children_CollectionChanged;
            }
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (OptionsNode item in e.NewItems)
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
                foreach (OptionsNode item in e.OldItems)
                {
                    foreach (OptionsNode sibling in Children)
                    {
                        if (item.Sequence < sibling.Sequence)
                        {
                            sibling.Sequence--;
                        }
                    }
                }
            }
        }

        public void Init()
        {
            foreach (OptionsNode item in Children)
            {
                item.Parent = this;
                item.Init();
            }
        }
        #endregion Initialization

        #region ICollection
        public void Add(OptionsNode item)
        {
            ((ICollection<OptionsNode>)_children).Add(item);
        }

        public void Clear()
        {
            ((ICollection<OptionsNode>)_children).Clear();
        }

        public bool Contains(OptionsNode item)
        {
            return ((ICollection<OptionsNode>)_children).Contains(item);
        }

        public void CopyTo(OptionsNode[] array, int arrayIndex)
        {
            ((ICollection<OptionsNode>)_children).CopyTo(array, arrayIndex);
        }

        public bool Remove(OptionsNode item)
        {
            return ((ICollection<OptionsNode>)_children).Remove(item);
        }

        public IEnumerator<OptionsNode> GetEnumerator()
        {
            return ((IEnumerable<OptionsNode>)_children).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Children).GetEnumerator();
        }
        #endregion ICollection
    }
}
