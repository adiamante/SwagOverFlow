using Dreamporter.Core;
using SwagOverFlow.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Dreamporter.WPF.Controls
{
    /// <summary>
    /// Interaction logic for DataStoresControl.xaml
    /// </summary>
    public partial class DataStoresControl : SwagControlBase
    {
        #region DataStores
        public static DependencyProperty DataStoresProperty =
            DependencyProperty.Register(
                "DataStores",
                typeof(ICollection<DataStore>),
                typeof(DataStoresControl));

        public ICollection<DataStore> DataStores
        {
            get { return (ICollection<DataStore>)GetValue(DataStoresProperty); }
            set { SetValue(DataStoresProperty, value); }
        }
        #endregion DataStores

        #region Initialization
        public DataStoresControl()
        {
            InitializeComponent();
        }
        #endregion Initialization

        #region Events
        private void SwagItemsControl_Add(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;

            String tag = (fe.Tag ?? "").ToString();
            switch (tag)
            {
                case "SQL":
                    DataStores.Add(new SqlConnectionStore());
                    break;
            }
        }
        #endregion Events
    }
}
