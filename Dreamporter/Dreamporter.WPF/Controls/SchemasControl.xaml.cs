using Dreamporter.Core;
using SwagOverFlow.WPF.Controls;
using SwagOverFlow.WPF.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Dreamporter.WPF.Controls
{
    /// <summary>
    /// Interaction logic for SchemasControl.xaml
    /// </summary>
    public partial class SchemasControl : SwagControlBase
    {
        #region Schemas
        public static DependencyProperty SchemasProperty =
            DependencyProperty.Register(
                "Schemas",
                typeof(ICollection<Schema>),
                typeof(SchemasControl));

        public ICollection<Schema> Schemas
        {
            get { return (ICollection<Schema>)GetValue(SchemasProperty); }
            set { SetValue(SchemasProperty, value); }
        }
        #endregion Schemas
        #region Save
        public static readonly RoutedEvent SaveEvent =
            EventManager.RegisterRoutedEvent(
            "Save",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SchemasControl));

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
                typeof(SchemasControl),
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
        public SchemasControl()
        {
            InitializeComponent();
        }
        #endregion Initialization

        #region Events
        private void SwagItemsControl_Add(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;

            switch (fe.DataContext)
            {
                case Schema schema:
                    schema.Tables.Add(new SchemaTable());
                    Refresh(schema.Tables);
                    
                    break;
                case SchemaTable table:
                    table.Columns.Add(new SchemaColumn());
                    Refresh(table.Columns);
                    break;
                case SchemaColumn column:
                    TreeViewItem tviCol = SwagItemsControl.ContainerFromItemRecursive(column);
                    TreeViewItem tviParentTable = tviCol.TryFindParent<TreeViewItem>();
                    SchemaTable tblParent = (SchemaTable)tviParentTable.DataContext;
                    tblParent.Columns.Add(new SchemaColumn());
                    Refresh(tblParent.Columns);
                    break;
                default:
                    Schemas.Add(new Schema());
                    Refresh(Schemas);
                    break;
            }
        }

        private void SwagItemsControl_Remove(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;

            switch (fe.DataContext)
            {
                case Schema schema:
                    Schemas.Remove(schema);
                    Refresh(Schemas);
                    break;
                case SchemaTable table:
                    TreeViewItem tviTable = SwagItemsControl.ContainerFromItemRecursive(table);
                    TreeViewItem tviParentSchema = tviTable.TryFindParent<TreeViewItem>();
                    Schema schParent = (Schema)tviParentSchema.DataContext;
                    schParent.Tables.Remove(table);
                    Refresh(schParent.Tables);
                    break;
                case SchemaColumn column:
                    TreeViewItem tviCol = SwagItemsControl.ContainerFromItemRecursive(column);
                    TreeViewItem tviParentTable = tviCol.TryFindParent<TreeViewItem>();
                    SchemaTable tblParent = (SchemaTable)tviParentTable.DataContext;
                    tblParent.Columns.Remove(column);
                    Refresh(tblParent.Columns);
                    break;
            }
        }

        private void SwagItemsControl_Save(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SaveEvent));
        }
        #endregion Events

        #region Method
        private void Refresh(Object col)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(col);
            view.Refresh();
        }
        #endregion Method


    }
}
