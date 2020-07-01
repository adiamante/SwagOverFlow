using Dreamporter.Core;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamporter.Core
{
    public class GroupInstruction : Instruction, ISwagParent<Instruction>
    {
        #region Private/Protected Members
        Boolean _isConcurrent = false;
        protected ObservableCollection<Instruction> _children = new ObservableCollection<Instruction>();
        #endregion Private/Protected Members

        #region Properties
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
        #endregion IsConcurrent
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

            OnPropertyChanged("HasChildren");

            //TODO: This probably get's called twice when adding so need to check on that sometime
            OnSwagItemChanged(this, new PropertyChangedExtendedEventArgs("Children", Children, null, null));
        }
        #endregion Initialization

        #region Events
        #endregion Events

        #region Methods
        public override void Execute(RunContext context, Dictionary<String, String> parameters)
        {
            if (IsConcurrent)
            {
                ConcurrentOrderedTasks tasks = new ConcurrentOrderedTasks();
                foreach (Instruction child in Children.OrderBy(c => c.Sequence))
                {
                    Task task = new Task(() => child.Run(context, parameters));
                    tasks.Append(task);
                }
                tasks.Execute();
            }
            else
            {
                foreach (Instruction child in Children.OrderBy(c => c.Sequence))
                {
                    child.Run(context, parameters);
                }
            }
        }
        #endregion Methods
    }
}
