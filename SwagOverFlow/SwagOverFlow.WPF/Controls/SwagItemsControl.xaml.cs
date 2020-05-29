using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.Commands;
using SwagOverFlow.WPF.UI;
using System;
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
        ICommand _saveCommand;
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
                        RaiseEvent(new RoutedEventArgs(SaveEvent, this));
                    }));
            }
        }
        #endregion SaveCommand
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
        #region ShowShowSaveContextMenuItem
        public static DependencyProperty ShowShowSaveContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowShowSaveContextMenuItem",
                typeof(Boolean),
                typeof(SwagItemsControl),
                new PropertyMetadata(false));

        public Boolean ShowShowSaveContextMenuItem
        {
            get { return (Boolean)GetValue(ShowShowSaveContextMenuItemProperty); }
            set
            {
                SetValue(ShowShowSaveContextMenuItemProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowShowSaveContextMenuItem
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

        #endregion Events

    }

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
