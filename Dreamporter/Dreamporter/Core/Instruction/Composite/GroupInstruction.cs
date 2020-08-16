using Dreamporter.Core;
using Newtonsoft.Json;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamporter.Core
{
    //https://stackoverflow.com/questions/14383736/how-to-serialize-deserialize-a-custom-collection-with-additional-properties-usin
    //So JsonConvert does not see this as an array
    [JsonObject]    
    public class GroupInstruction : Instruction, ISwagParent<Instruction>, ICollection<Instruction>
    {
        #region Private/Protected Members
        Boolean _isInitialized = false;
        Boolean _isConcurrent = false;
        protected ObservableCollection<Instruction> _children = new ObservableCollection<Instruction>();
        #endregion Private/Protected Members

        #region Properties
        #region IsInitialized
        [NotMapped]
        [JsonIgnore]
        public Boolean IsInitialized
        {
            get { return _isInitialized; }
            set { SetValue(ref _isInitialized, value); }
        }
        #endregion IsInitialized
        #region Type
        public override Type Type { get { return typeof(GroupInstruction); } }
        #endregion Type
        #region Children
        public ObservableCollection<Instruction> Children
        {
            get { return _children; }
            set { SetValue(ref _children, value); }
        }
        #endregion Children
        #region IsConcurrent
        public Boolean IsConcurrent
        {
            get { return _isConcurrent; }
            set { SetValue(ref _isConcurrent, value); }
        }

        public int Count => ((ICollection<Instruction>)_children).Count;

        public bool IsReadOnly => ((ICollection<Instruction>)_children).IsReadOnly;
        #endregion IsConcurrent
        #endregion Properties

        #region SwagItemChanged
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

        public void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedEventArgs e)
        {
            Instruction ix = (Instruction)swagItem;

            switch (e)
            {
                case PropertyChangedExtendedEventArgs exArgs:
                    SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs()
                    {
                        SwagItem = swagItem,
                        PropertyChangedArgs = e,
                        Message = $"{ix.Path}({exArgs.PropertyName})\n\t{exArgs.OldValue} => {exArgs.NewValue}"
                    });
                    break;
                case CollectionPropertyChangedEventArgs colArgs:
                    SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs()
                    {
                        SwagItem = swagItem,
                        PropertyChangedArgs = e,
                        Message = $"{ix.Path}({colArgs.PropertyName})\n\t[OLD] => {colArgs.OldItems}\n\t[NEW] {colArgs.NewItems}"
                    });
                    break;
            }

            Parent?.OnSwagItemChanged(swagItem, e);
        }
        #endregion SwagItemChanged

        #region Initialization
        public GroupInstruction() : base()
        {
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Instruction item in e.NewItems)
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
                foreach (Instruction item in e.OldItems)
                {
                    foreach (Instruction sibling in Children)
                    {
                        if (item.Sequence < sibling.Sequence)
                        {
                            sibling.Sequence--;
                        }
                    }
                }
            }

            OnSwagItemChanged(this, new CollectionPropertyChangedEventArgs(nameof(Children), this, e.Action, e.OldItems, e.NewItems));
            OnPropertyChanged("HasChildren");
        }
        #endregion Initialization

        #region Events
        #endregion Events

        #region ICollection
        public void Add(Instruction item)
        {
            ((ICollection<Instruction>)_children).Add(item);
        }

        public void Clear()
        {
            ((ICollection<Instruction>)_children).Clear();
        }

        public bool Contains(Instruction item)
        {
            return ((ICollection<Instruction>)_children).Contains(item);
        }

        public void CopyTo(Instruction[] array, int arrayIndex)
        {
            ((ICollection<Instruction>)_children).CopyTo(array, arrayIndex);
        }

        public bool Remove(Instruction item)
        {
            return ((ICollection<Instruction>)_children).Remove(item);
        }

        public IEnumerator<Instruction> GetEnumerator()
        {
            return ((IEnumerable<Instruction>)_children).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_children).GetEnumerator();
        }
        #endregion ICollection

        #region Methods
        public override void RunHandler(RunContext context, RunParams rp)
        {
            if (IsConcurrent)
            {
                ConcurrentOrderedTasks tasks = new ConcurrentOrderedTasks();
                foreach (Instruction child in Children.OrderBy(c => c.Sequence))
                {
                    Task task = new Task(() => child.Run(context, rp));
                    tasks.Append(task);
                }
                tasks.Execute();
            }
            else
            {
                foreach (Instruction child in Children.OrderBy(c => c.Sequence))
                {
                    child.Run(context, rp);
                }
            }
        }
        #endregion Methods
    }
}
