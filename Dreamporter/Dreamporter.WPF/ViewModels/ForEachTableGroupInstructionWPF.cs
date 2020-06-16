using Dreamporter.Instructions;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Data;

namespace Dreamporter.WPF.ViewModels
{
    public class ForEachTableGroupInstructionWPF : ForEachTableGroupInstruction
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
        public ForEachTableGroupInstructionWPF() : base()
        {
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Instruction instruction in e.NewItems)
                {
                    instruction.PropertyChanged += Instruction_PropertyChanged;
                }
            }

            _childrenCollectionViewSource.View.Refresh();
        }

        private void Instruction_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Sequence")
            {
                _childrenCollectionViewSource.View.Refresh();
            }
        }
        #endregion Initialization
    }
}
