using SwagOverflow.ViewModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Data;
using Newtonsoft.Json;

namespace SwagOverflowWPF.ViewModels
{
    public class SwagItemGroupWPF<T> : SwagItemGroup<T>
    {
        #region Private/Protected Members
        CollectionViewSource _childrenCollectionViewSource;
        #endregion Private/Protected Members

        #region ChildrenView
        [JsonIgnore]
        [NotMapped]
        public ICollectionView ChildrenView
        {
            get { return _childrenCollectionViewSource.View; }
        }
        #endregion ChildrenView

        public SwagItemGroupWPF() : base()
        {
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagItem<T> newItem in e.NewItems)
                {
                    //Will probably hard to troubleshoot. It's the price of transparency
                    if (typeof(T).IsSubclassOf(typeof(ViewModelBaseExtended)))
                    {
                        ViewModelBaseExtended vm = newItem.Value as ViewModelBaseExtended;
                        vm.PropertyChangedExtended += (s, e) =>
                        {
                            OnSwagItemChanged(newItem, e);
                        };

                        newItem.PropertyChangedExtended += (s, e) =>
                        {
                            if (e.PropertyName == "ObjValue" || e.PropertyName == "Value")
                            {
                                OnSwagItemChanged(newItem, e);
                            };
                        };
                    }
                }
            }

            _childrenCollectionViewSource.View.Refresh();
        }
    }
}
