using Microsoft.Win32;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.Commands;
using SwagOverFlow.WPF.UI;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SwagOverFlow.WPF.Controls
{
    /// <summary>
    /// Interaction logic for SwagItemsControl.xaml
    /// </summary>
    public partial class SwagItemsControl : SwagControlBase
    {
        #region Private Members
        Point _startPoint;
        bool _isDragging = false;
        ICommand _searchCommand, _saveCommand, _expandCommand, _collapseCommand, _addCommand, _copyCommand, _pasteCommand,
            _importCommand, _exportCommand, _clearCommand, _itemContextMenuOpenedCommand, _removeCommand;
        #endregion Private Members

        #region Properties
        #region SwagItemsSource
        private static readonly DependencyProperty SwagItemsSourceProperty =
        DependencyProperty.Register("SwagItemsSource", typeof(SwagItemBase), typeof(SwagItemsControl));

        public SwagItemBase SwagItemsSource
        {
            get { return (SwagItemBase)GetValue(SwagItemsSourceProperty); }
            set
            {
                SetValue(SwagItemsSourceProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion SwagItemsSource
        #region CustomDefaultItemTemplate
        public static readonly DependencyProperty CustomDefaultItemTemplateProperty =
            DependencyProperty.Register("CustomDefaultItemTemplate", typeof(SwagTemplate), typeof(SwagItemsControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public SwagTemplate CustomDefaultItemTemplate
        {
            get { return (SwagTemplate)GetValue(CustomDefaultItemTemplateProperty); }
            set { SetValue(CustomDefaultItemTemplateProperty, value); }
        }
        #endregion CustomDefaultItemTemplate
        #region ItemTemplates
        public static readonly DependencyProperty ItemTemplatesProperty =
            DependencyProperty.Register("ItemTemplates", typeof(SwagTemplateCollection), typeof(SwagItemsControl),
            new FrameworkPropertyMetadata(new SwagTemplateCollection(), FrameworkPropertyMetadataOptions.Inherits));

        public SwagTemplateCollection ItemTemplates
        {
            get { return (SwagTemplateCollection)GetValue(ItemTemplatesProperty); }
            set { SetValue(ItemTemplatesProperty, value); }
        }
        #endregion ItemTemplates
        #region CustomDefaultItemContainerStyle
        public static readonly DependencyProperty CustomDefaultItemContainerStyleProperty =
            DependencyProperty.Register("CustomDefaultItemContainerStyle", typeof(SwagStyle), typeof(SwagItemsControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public SwagStyle CustomDefaultItemContainerStyle
        {
            get { return (SwagStyle)GetValue(CustomDefaultItemContainerStyleProperty); }
            set { SetValue(CustomDefaultItemContainerStyleProperty, value); }
        }
        #endregion CustomDefaultItemContainerStyle
        #region ItemContainerStyles
        public static readonly DependencyProperty ItemContainerStylesProperty =
            DependencyProperty.Register("ItemContainerStyles", typeof(SwagStyleCollection), typeof(SwagItemsControl),
            new FrameworkPropertyMetadata(new SwagStyleCollection(), FrameworkPropertyMetadataOptions.Inherits));

        public SwagStyleCollection ItemContainerStyles
        {
            get { return (SwagStyleCollection)GetValue(ItemContainerStylesProperty); }
            set { SetValue(ItemContainerStylesProperty, value); }
        }
        #endregion ItemContainerStyles
        #region AllowMove
        public static DependencyProperty AllowMoveProperty =
            DependencyProperty.Register(
                "AllowMove",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(new PropertyChangedCallback(AllowMovePropertyChanged)));

        private static void AllowMovePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            SwagItemsControl sictl = (SwagItemsControl)source;
            sictl.ControlTreeView.AllowDrop = (Boolean)e.NewValue;
        }

        public Boolean AllowMove
        {
            get { return (Boolean)GetValue(AllowMoveProperty); }
            set { SetValue(AllowMoveProperty, value); }
        }
        #endregion AllowMove
        #region TreeViewItemDropPreview
        public static readonly RoutedEvent TreeViewItemDropPreviewEvent =
            EventManager.RegisterRoutedEvent(
            "TreeViewItemDropPreview",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SwagItemsControl));

        public event RoutedEventHandler TreeViewItemDropPreview
        {
            add { AddHandler(TreeViewItemDropPreviewEvent, value); }
            remove { RemoveHandler(TreeViewItemDropPreviewEvent, value); }
        }
        #endregion TreeViewItemDropPreview
        #region TreeViewItemLeavePreview
        public static readonly RoutedEvent TreeViewItemLeavePreviewEvent =
            EventManager.RegisterRoutedEvent(
            "TreeViewItemLeavePreview",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SwagItemsControl));

        public event RoutedEventHandler TreeViewItemLeavePreview
        {
            add { AddHandler(TreeViewItemLeavePreviewEvent, value); }
            remove { RemoveHandler(TreeViewItemLeavePreviewEvent, value); }
        }
        #endregion TreeViewItemDropPreview
        #region TreeViewItemDrop
        public static readonly RoutedEvent TreeViewItemDropEvent =
            EventManager.RegisterRoutedEvent(
            "TreeViewItemDrop",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SwagItemsControl));

        public event RoutedEventHandler TreeViewItemDrop
        {
            add { AddHandler(TreeViewItemDropEvent, value); }
            remove { RemoveHandler(TreeViewItemDropEvent, value); }
        }
        #endregion TreeViewItemDrop
        #region Search
        public static readonly RoutedEvent SearchEvent =
            EventManager.RegisterRoutedEvent(
            "Search",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SwagItemsControl));

        public event RoutedEventHandler Search
        {
            add { AddHandler(SearchEvent, value); }
            remove { RemoveHandler(SearchEvent, value); }
        }
        #endregion Search
        #region SearchCommand
        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand ??
                    (_searchCommand = new RelayCommand<object>((s) =>
                    {
                        RaiseEvent(new RoutedEventArgs(SearchEvent, s ?? this));
                    }));
            }
        }
        #endregion SearchCommand
        #region Save
        public static readonly RoutedEvent SaveEvent =
            EventManager.RegisterRoutedEvent(
            "Save",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SwagItemsControl));

        public event RoutedEventHandler Save
        {
            add { AddHandler(SaveEvent, value); }
            remove { RemoveHandler(SaveEvent, value); }
        }
        #endregion Save
        #region SaveCommand
        public ICommand SaveCommand
        {
            get 
            {
                return _saveCommand ??
                    (_saveCommand = new RelayCommand<object>((s) =>
                    {
                        RaiseEvent(new RoutedEventArgs(SaveEvent, s ?? this));
                    }));
            }
        }
        #endregion SaveCommand
        #region Expand
        public static readonly RoutedEvent ExpandEvent =
            EventManager.RegisterRoutedEvent(
            "Expand",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SwagItemsControl));

        public event RoutedEventHandler Expand
        {
            add { AddHandler(ExpandEvent, value); }
            remove { RemoveHandler(ExpandEvent, value); }
        }
        #endregion Expand
        #region ExpandCommand
        public ICommand ExpandCommand
        {
            get
            {
                return _expandCommand ??
                    (_expandCommand = new RelayCommand<object>((s) =>
                    {
                        RaiseEvent(new RoutedEventArgs(ExpandEvent, s ?? this));
                    }));
            }
        }
        #endregion ExpandCommand
        #region Collapse
        public static readonly RoutedEvent CollapseEvent =
            EventManager.RegisterRoutedEvent(
            "Collapse",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SwagItemsControl));

        public event RoutedEventHandler Collapse
        {
            add { AddHandler(CollapseEvent, value); }
            remove { RemoveHandler(CollapseEvent, value); }
        }
        #endregion Collapse
        #region CollapseCommand
        public ICommand CollapseCommand
        {
            get
            {
                return _collapseCommand ??
                    (_collapseCommand = new RelayCommand<object>((s) =>
                    {
                        RaiseEvent(new RoutedEventArgs(CollapseEvent, s ?? this));
                    }));
            }
        }
        #endregion CollapseCommand
        #region Add
        public static readonly RoutedEvent AddEvent =
            EventManager.RegisterRoutedEvent(
            "Add",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SwagItemsControl));

        public event RoutedEventHandler Add
        {
            add { AddHandler(AddEvent, value); }
            remove { RemoveHandler(AddEvent, value); }
        }
        #endregion Add
        #region AddCommand
        public ICommand AddCommand
        {
            get
            {
                return _addCommand ??
                    (_addCommand = new RelayCommand<object>((s) =>
                    {
                        RaiseEvent(new RoutedEventArgs(AddEvent, s ?? this));
                    }));
            }
        }
        #endregion AddCommand
        #region Copy
        public static readonly RoutedEvent CopyEvent =
            EventManager.RegisterRoutedEvent(
            "Copy",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SwagItemsControl));

        public event RoutedEventHandler Copy
        {
            add { AddHandler(CopyEvent, value); }
            remove { RemoveHandler(CopyEvent, value); }
        }
        #endregion Copy
        #region CopyCommand
        public ICommand CopyCommand
        {
            get
            {
                return _copyCommand ??
                    (_copyCommand = new RelayCommand<object>((s) =>
                    {
                        RaiseEvent(new RoutedEventArgs(CopyEvent, s ?? this));
                    }));
            }
        }
        #endregion CopyCommand
        #region Paste
        public static readonly RoutedEvent PasteEvent =
            EventManager.RegisterRoutedEvent(
            "Paste",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SwagItemsControl));

        public event RoutedEventHandler Paste
        {
            add { AddHandler(PasteEvent, value); }
            remove { RemoveHandler(PasteEvent, value); }
        }
        #endregion Paste
        #region PasteCommand
        public ICommand PasteCommand
        {
            get
            {
                return _pasteCommand ??
                    (_pasteCommand = new RelayCommand<object>((s) =>
                    {
                        RaiseEvent(new RoutedEventArgs(PasteEvent, s ?? this));
                    }));
            }
        }
        #endregion PasteCommand
        #region Export
        public static readonly RoutedEvent ExportEvent =
            EventManager.RegisterRoutedEvent(
            "Export",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SwagItemsControl));

        public event RoutedEventHandler Export
        {
            add { AddHandler(ExportEvent, value); }
            remove { RemoveHandler(ExportEvent, value); }
        }
        #endregion Export
        #region ExportCommand
        public ICommand ExportCommand
        {
            get
            {
                return _exportCommand ??
                    (_exportCommand = new RelayCommand<object>((s) =>
                    {
                        RaiseEvent(new RoutedEventArgs(ExportEvent, s ?? this));
                    }));
            }
        }
        #endregion ExportCommand
        #region Import
        public static readonly RoutedEvent ImportEvent =
            EventManager.RegisterRoutedEvent(
            "Import",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SwagItemsControl));

        public event RoutedEventHandler Import
        {
            add { AddHandler(ImportEvent, value); }
            remove { RemoveHandler(ImportEvent, value); }
        }
        #endregion Import
        #region ImportCommand
        public ICommand ImportCommand
        {
            get
            {
                return _importCommand ??
                    (_importCommand = new RelayCommand<object>((s) =>
                    {
                        RaiseEvent(new RoutedEventArgs(ImportEvent, s ?? this));
                    }));
            }
        }
        #endregion ImportCommand
        #region Clear
        public static readonly RoutedEvent ClearEvent =
            EventManager.RegisterRoutedEvent(
            "Clear",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SwagItemsControl));

        public event RoutedEventHandler Clear
        {
            add { AddHandler(ClearEvent, value); }
            remove { RemoveHandler(ClearEvent, value); }
        }
        #endregion Clear
        #region ClearCommand
        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ??
                    (_clearCommand = new RelayCommand<object>((s) =>
                    {
                        RaiseEvent(new RoutedEventArgs(ClearEvent, s ?? this));
                    }));
            }
        }
        #endregion ClearCommand
        #region ItemContextMenuOpened
        public static readonly RoutedEvent ItemContextMenuOpenedEvent =
            EventManager.RegisterRoutedEvent(
            "ItemContextMenuOpened",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SwagItemsControl));

        public event RoutedEventHandler ItemContextMenuOpened
        {
            add { AddHandler(ItemContextMenuOpenedEvent, value); }
            remove { RemoveHandler(ItemContextMenuOpenedEvent, value); }
        }
        #endregion ItemContextMenuOpened
        #region UseDefaultItemContextMenuOpened
        public static DependencyProperty UseDefaultItemContextMenuOpenedProperty =
            DependencyProperty.Register(
                "UseDefaultItemContextMenuOpened",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(false));

        public Boolean UseDefaultItemContextMenuOpened
        {
            get { return (Boolean)GetValue(UseDefaultItemContextMenuOpenedProperty); }
            set { SetValue(UseDefaultItemContextMenuOpenedProperty, value); }
        }
        #endregion UseDefaultItemContextMenuOpened
        #region ItemContextMenuOpenedCommand
        public ICommand ItemContextMenuOpenedCommand
        {
            get
            {
                return _itemContextMenuOpenedCommand ??
                    (_itemContextMenuOpenedCommand = new RelayCommand<object>((s) =>
                    {
                        if (UseDefaultItemContextMenuOpened)
                        {
                            FrameworkElement fe = (FrameworkElement)s ?? this;
                            if (fe.DataContext is SwagItemBase item)
                            {
                                item.IsSelected = true;
                            }
                        }
                        else
                        {
                            RaiseEvent(new RoutedEventArgs(ItemContextMenuOpenedEvent, s ?? this));
                        }
                    }));
            }
        }
        #endregion ItemContextMenuOpenedCommand
        #region Remove
        public static readonly RoutedEvent RemoveEvent =
            EventManager.RegisterRoutedEvent(
            "Remove",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SwagItemsControl));

        public event RoutedEventHandler Remove
        {
            add { AddHandler(RemoveEvent, value); }
            remove { RemoveHandler(RemoveEvent, value); }
        }
        #endregion Remove
        #region RemoveCommand
        public ICommand RemoveCommand
        {
            get
            {
                return _removeCommand ??
                    (_removeCommand = new RelayCommand<object>((s) =>
                    {
                        RaiseEvent(new RoutedEventArgs(RemoveEvent, s ?? this));
                    }));
            }
        }
        #endregion RemoveCommand
        #region ShowSaveButton
        public static DependencyProperty ShowSaveButtonProperty =
            DependencyProperty.Register(
                "ShowSaveButton",
                typeof(Boolean),
                typeof(SwagItemsControl),
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
        #region ShowSaveContextMenuItem
        public static DependencyProperty ShowSaveContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowSaveContextMenuItem",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(true));

        public Boolean ShowSaveContextMenuItem
        {
            get { return (Boolean)GetValue(ShowSaveContextMenuItemProperty); }
            set
            {
                SetValue(ShowSaveContextMenuItemProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowSaveContextMenuItem
        #region ShowSearchContextMenuItem
        public static DependencyProperty ShowSearchContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowSearchContextMenuItem",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(false));

        public Boolean ShowSearchContextMenuItem
        {
            get { return (Boolean)GetValue(ShowSearchContextMenuItemProperty); }
            set
            {
                SetValue(ShowSearchContextMenuItemProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowSearchContextMenuItem
        #region ShowLevelContextMenuItem
        public static DependencyProperty ShowLevelContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowLevelContextMenuItem",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(false));

        public Boolean ShowLevelContextMenuItem
        {
            get { return (Boolean)GetValue(ShowLevelContextMenuItemProperty); }
            set
            {
                SetValue(ShowLevelContextMenuItemProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowLevelContextMenuItem
        #region ShowCopyContextMenuItem
        public static DependencyProperty ShowCopyContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowCopyContextMenuItem",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(false));

        public Boolean ShowCopyContextMenuItem
        {
            get { return (Boolean)GetValue(ShowCopyContextMenuItemProperty); }
            set
            {
                SetValue(ShowCopyContextMenuItemProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowCopyContextMenuItem
        #region ShowAddContextMenuItem
        public static DependencyProperty ShowAddContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowAddContextMenuItem",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(false));

        public Boolean ShowAddContextMenuItem
        {
            get { return (Boolean)GetValue(ShowAddContextMenuItemProperty); }
            set
            {
                SetValue(ShowAddContextMenuItemProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowAddContextMenuItem
        #region ShowPasteContextMenuItem
        public static DependencyProperty ShowPasteContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowPasteContextMenuItem",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(false));

        public Boolean ShowPasteContextMenuItem
        {
            get { return (Boolean)GetValue(ShowPasteContextMenuItemProperty); }
            set
            {
                SetValue(ShowPasteContextMenuItemProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowPasteContextMenuItem
        #region ShowExportContextMenuItem
        public static DependencyProperty ShowExportContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowExportContextMenuItem",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(false));

        public Boolean ShowExportContextMenuItem
        {
            get { return (Boolean)GetValue(ShowExportContextMenuItemProperty); }
            set
            {
                SetValue(ShowExportContextMenuItemProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowExportContextMenuItem
        #region ShowImportContextMenuItem
        public static DependencyProperty ShowImportContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowImportContextMenuItem",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(false));

        public Boolean ShowImportContextMenuItem
        {
            get { return (Boolean)GetValue(ShowImportContextMenuItemProperty); }
            set
            {
                SetValue(ShowImportContextMenuItemProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowImportContextMenuItem
        #region ShowItemSaveContextMenuItem
        public static DependencyProperty ShowItemSaveContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowItemSaveContextMenuItem",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(false));

        public Boolean ShowItemSaveContextMenuItem
        {
            get { return (Boolean)GetValue(ShowItemSaveContextMenuItemProperty); }
            set
            {
                SetValue(ShowItemSaveContextMenuItemProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowItemSaveContextMenuItem
        #region ShowItemLevelContextMenuItem
        public static DependencyProperty ShowItemLevelContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowItemLevelContextMenuItem",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(false));

        public Boolean ShowItemLevelContextMenuItem
        {
            get { return (Boolean)GetValue(ShowItemLevelContextMenuItemProperty); }
            set
            {
                SetValue(ShowItemLevelContextMenuItemProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowItemLevelContextMenuItem
        #region ShowItemAddContextMenuItem
        public static DependencyProperty ShowItemAddContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowItemAddContextMenuItem",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(false));

        public Boolean ShowItemAddContextMenuItem
        {
            get { return (Boolean)GetValue(ShowItemAddContextMenuItemProperty); }
            set
            {
                SetValue(ShowItemAddContextMenuItemProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowItemAddContextMenuItem
        #region ShowItemCopyContextMenuItem
        public static DependencyProperty ShowItemCopyContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowItemCopyContextMenuItem",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(false));

        public Boolean ShowItemCopyContextMenuItem
        {
            get { return (Boolean)GetValue(ShowItemCopyContextMenuItemProperty); }
            set
            {
                SetValue(ShowItemCopyContextMenuItemProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowItemCopyContextMenuItem
        #region ShowItemPasteContextMenuItem
        public static DependencyProperty ShowItemPasteContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowItemPasteContextMenuItem",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(false));

        public Boolean ShowItemPasteContextMenuItem
        {
            get { return (Boolean)GetValue(ShowItemPasteContextMenuItemProperty); }
            set
            {
                SetValue(ShowItemPasteContextMenuItemProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowItemPasteContextMenuItem
        #region ShowItemExportContextMenuItem
        public static DependencyProperty ShowItemExportContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowItemExportContextMenuItem",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(false));

        public Boolean ShowItemExportContextMenuItem
        {
            get { return (Boolean)GetValue(ShowItemExportContextMenuItemProperty); }
            set
            {
                SetValue(ShowItemExportContextMenuItemProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowItemExportContextMenuItem
        #region ShowItemImportContextMenuItem
        public static DependencyProperty ShowItemImportContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowItemImportContextMenuItem",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(false));

        public Boolean ShowItemImportContextMenuItem
        {
            get { return (Boolean)GetValue(ShowItemImportContextMenuItemProperty); }
            set
            {
                SetValue(ShowItemImportContextMenuItemProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowItemImportContextMenuItem
        #region ShowItemRemoveContextMenuItem
        public static DependencyProperty ShowItemRemoveContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowItemRemoveContextMenuItem",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(false));

        public Boolean ShowItemRemoveContextMenuItem
        {
            get { return (Boolean)GetValue(ShowItemRemoveContextMenuItemProperty); }
            set
            {
                SetValue(ShowItemRemoveContextMenuItemProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowItemRemoveContextMenuItem
        #region SaveVerticalAlignment
        public static DependencyProperty SaveVerticalAlignmentProperty =
            DependencyProperty.Register(
                "SaveVerticalAlignment",
                typeof(VerticalAlignment),
                typeof(SwagItemsControl));

        public VerticalAlignment SaveVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(SaveVerticalAlignmentProperty); }
            set 
            { 
                SetValue(SaveVerticalAlignmentProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion SaveVerticalAlignment
        #region SaveHorizontalAlignment
        public static DependencyProperty SaveHorizontalAlignmentProperty =
            DependencyProperty.Register(
                "SaveHorizontalAlignment",
                typeof(HorizontalAlignment),
                typeof(SwagItemsControl));

        public HorizontalAlignment SaveHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(SaveHorizontalAlignmentProperty); }
            set 
            { 
                SetValue(SaveHorizontalAlignmentProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion SaveHorizontalAlignment
        #endregion Properties

        #region Initialization
        public SwagItemsControl()
        {
            InitializeComponent();
        }
        #endregion Initialization

        #region Events
        //https://stackoverflow.com/questions/1026179/drag-drop-in-treeview
        void ControlTreeView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (AllowMove &&
               (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed && !_isDragging))
            {
                Point position = e.GetPosition(null);
                if (Math.Abs(position.X - _startPoint.X) >
                        SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _startPoint.Y) >
                        SystemParameters.MinimumVerticalDragDistance)
                {
                    StartDrag(e);
                }
            }
        }

        void ControlTreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (AllowMove)
            {
                _startPoint = e.GetPosition(null);
            }
        }

        private void StartDrag(MouseEventArgs e)
        {
            _isDragging = true;
            object temp = ControlTreeView.SelectedItem;
            DataObject data = null;

            TreeViewItem tvi = ControlTreeView
                       .ItemContainerGenerator
                       .ContainerFromItemRecursive(ControlTreeView.SelectedItem);

            if (tvi != null)
            {
                data = new DataObject("TreeViewItemDrop", tvi);

                if (data != null)
                {
                    DragDrop.DoDragDrop(ControlTreeView, data, DragDropEffects.Move);
                }
            }
            
            _isDragging = false;
        }

        private void ControlTreeView_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (AllowMove)
            {
                TreeViewItem targetItem = ((DependencyObject)e.OriginalSource).TryFindParent<TreeViewItem>();
                TreeViewItem droppedItem = (TreeViewItem)e.Data.GetData("TreeViewItemDrop");

                if (targetItem != null && droppedItem != null)
                {
                    RoutedEventArgs ea = new TreeViewItemDropEventArgs(SwagItemsControl.TreeViewItemDropPreviewEvent, e, targetItem, droppedItem);
                    RaiseEvent(ea);
                }
            }
        }

        private void ControlTreeView_Drop(object sender, DragEventArgs e)
        {
            if (AllowMove)
            {
                TreeViewItem targetItem = ((DependencyObject)e.OriginalSource).TryFindParent<TreeViewItem>();
                TreeViewItem droppedItem = (TreeViewItem)e.Data.GetData("TreeViewItemDrop");

                if (targetItem != null && droppedItem != null)
                {
                    RoutedEventArgs ea = new TreeViewItemDropEventArgs(SwagItemsControl.TreeViewItemDropEvent, e, targetItem, droppedItem);
                    RaiseEvent(ea);
                }
            }
        }

        private void ControlTreeView_PreviewDragLeave(object sender, DragEventArgs e)
        {
            if (AllowMove)
            {
                TreeViewItem targetItem = ((DependencyObject)e.OriginalSource).TryFindParent<TreeViewItem>();
                if (targetItem != null)
                {
                    RoutedEventArgs ea = new TreeViewItemDropEventArgs(SwagItemsControl.TreeViewItemLeavePreviewEvent, e, targetItem, null);
                    RaiseEvent(ea);
                }
            }
        }

        private void ItemContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)sender;
            if (fe.DataContext is SwagItemBase item)
            {
                item.IsSelected = true;
            }
        }

        #endregion Events

    }

    #region SwagItemsControlHelper
    public static class SwagItemsControlHelper
    {
        public static void SetClipBoardData<T>(T item)
        {
            String json = JsonHelper.ToJsonString(item);
            Clipboard.SetData(typeof(T).Name, json);
        }

        public static T GetClipBoardData<T>()
        {
            if (Clipboard.ContainsData(typeof(T).Name))
            {
                String json = Clipboard.GetData(typeof(T).Name).ToString();
                T item = JsonHelper.ToObject<T>(json);
                return item;
            }
            return default(T);
        }

        public static void ExportDataToFile<T>(T item)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            sfd.FileName = typeof(T).Name;
            sfd.Filter = "JSON files (*.json)|*.json";
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (sfd.ShowDialog() ?? false)
                {
                    String json = JsonHelper.ToJsonString(item);
                    File.WriteAllText(sfd.FileName, json);
                }
            }));
        }

        public static T GetDataFromFile<T>()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            ofd.FileName = typeof(T).Name;
            ofd.Filter = "JSON files (*.json)|*.json";
            T item = default(T);
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (ofd.ShowDialog() ?? false)
                {
                    String json = File.ReadAllText(ofd.FileName);
                    item = JsonHelper.ToObject<T>(json);
                }
            }));

            return item;
        }
    }
    #endregion SwagItemsControlHelper

    #region TreeViewItemDropEventArgs
    public class TreeViewItemDropEventArgs : RoutedEventArgs
    {
        private readonly TreeViewItem _targetItem, _droppedItem;
        private readonly DragEventArgs _dragEventArgs;

        public TreeViewItem TargetItem
        {
            get { return _targetItem; }
        }

        public TreeViewItem DroppedItem
        {
            get { return _droppedItem; }
        }

        public DragEventArgs DragEventArgs
        {
            get { return _dragEventArgs; }
        }

        public TreeViewItemDropEventArgs(RoutedEvent routedEvent, DragEventArgs dragEventArgs, TreeViewItem targetItem, TreeViewItem droppedItem) : base(routedEvent)
        {
            _dragEventArgs = dragEventArgs;
            _targetItem = targetItem;
            _droppedItem = droppedItem;
        }
    }
    #endregion TreeViewItemDropEventArgs
}
