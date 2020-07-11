using Dreamporter.Core;
using SwagOverFlow.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Dreamporter.WPF.Controls
{
    /// <summary>
    /// Interaction logic for DataContextsControl.xaml
    /// </summary>
    public partial class DataContextsControl : SwagControlBase
    {
        #region DataContexts
        public static DependencyProperty DataContextsProperty =
            DependencyProperty.Register(
                "DataContexts",
                typeof(ICollection<DataContext>),
                typeof(DataContextsControl));

        public ICollection<DataContext> DataContexts
        {
            get { return (ICollection<DataContext>)GetValue(DataContextsProperty); }
            set { SetValue(DataContextsProperty, value); }
        }
        #endregion DataContexts
        #region Save
        public static readonly RoutedEvent SaveEvent =
            EventManager.RegisterRoutedEvent(
            "Save",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(DataContextsControl));

        public event RoutedEventHandler Save
        {
            add { AddHandler(SaveEvent, value); }
            remove { RemoveHandler(SaveEvent, value); }
        }
        #endregion Save
        #region ShowSaveButton
        public static DependencyProperty ShowSaveButtonProperty =
            DependencyProperty.Register(
                "ShowSaveButton",
                typeof(Boolean),
                typeof(DataContextsControl),
                new PropertyMetadata(false));

        public Boolean ShowSaveButton
        {
            get { return (Boolean)GetValue(ShowSaveButtonProperty); }
            set
            {
                SetValue(ShowSaveButtonProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowSaveButton

        #region Initialization
        public DataContextsControl()
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
                    DataContexts.Add(new SqlConnectionDataContext());
                    break;
            }

            CollectionViewSource.GetDefaultView(DataContexts).Refresh();
        }

        private void SwagItemsControl_Save(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SaveEvent));
        }
        #endregion Events
    }
}
