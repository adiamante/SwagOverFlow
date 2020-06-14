using SwagOverFlow.ViewModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Data;
using Newtonsoft.Json;
using System.Windows.Input;
using SwagOverFlow.WPF.Commands;
using System;

namespace SwagOverFlow.WPF.ViewModels
{
    public class SwagValueItemGroupWPF<T> : SwagValueItemGroup<T>
    {
        #region Private/Protected Members
        CollectionViewSource _childrenCollectionViewSource;
        ICommand _addDefaultCommand;
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
        #region AddDefaultCommand
        [JsonIgnore]
        [NotMapped]
        public ICommand AddDefaultCommand
        {
            get
            {
                return _addDefaultCommand ?? (_addDefaultCommand =
                    new RelayCommand(() =>
                    {
                        T val = typeof(T).GetConstructor(Type.EmptyTypes) != null ? (T)Activator.CreateInstance(typeof(T)) : default(T);
                        Children.Add(new SwagValueItem<T>() { Value = val });
                    }));
            }
        }
        #endregion AddDefaultCommand
        #endregion Properties

        #region Initialization
        public SwagValueItemGroupWPF() : base()
        {
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagValueItem<T> newItem in e.NewItems)
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
        #endregion Initialization
    }
}
