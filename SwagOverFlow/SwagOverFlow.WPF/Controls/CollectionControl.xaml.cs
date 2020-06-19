using SwagOverFlow.Utils;
using SwagOverFlow.WPF.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SwagOverFlow.WPF.Controls
{
    /// <summary>
    /// Interaction logic for CollectionControl.xaml
    /// </summary>
    public partial class CollectionControl : SwagControlBase
    {
        ICommand _addCommand, _removeCommand, _clearCommand;

        #region Collection
        private static readonly DependencyProperty CollectionProperty =
        DependencyProperty.Register("Collection", typeof(ICollection), typeof(CollectionControl));

        public ICollection Collection
        {
            get { return (ICollection)GetValue(CollectionProperty); }
            set
            {
                SetValue(CollectionProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion Collection

        #region ItemTemplate
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(CollectionControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        #endregion ItemTemplate

        #region AddCommand
        public ICommand AddCommand
        {
            get
            {
                return _addCommand ??
                    (_addCommand = new RelayCommand<object>((s) =>
                    {
                        Type t = Collection.GetType();
                        Type[] typeArgs = t.GetGenericArguments();
                        if (typeArgs.Length == 1)
                        {
                            var newObj = Activator.CreateInstance(typeArgs[0]);
                            ReflectionHelper.MethodInfoCollection[Collection.GetType()]["Add"].Invoke(Collection, new object[] { newObj });
                            Refresh();
                        }
                    }));
            }
        }
        #endregion AddCommand

        #region RemoveCommand
        public ICommand RemoveCommand
        {
            get
            {
                return _removeCommand ??
                    (_removeCommand = new RelayCommand<object>((s) =>
                    {
                        MenuItem mi = (MenuItem)s;
                        ReflectionHelper.MethodInfoCollection[Collection.GetType()]["Remove"].Invoke(Collection, new object [] { mi.DataContext });
                        Refresh();
                    }));
            }
        }
        #endregion RemoveCommand

        #region ClearCommand
        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ??
                    (_clearCommand = new RelayCommand<object>((s) =>
                    {
                        ReflectionHelper.MethodInfoCollection[Collection.GetType()]["Clear"].Invoke(Collection, null);
                        Refresh();
                    }));
            }
        }
        #endregion ClearCommand

        public CollectionControl()
        {
            InitializeComponent();
        }

        private void Refresh()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(Collection);
            view.Refresh();
        }
    }
}
