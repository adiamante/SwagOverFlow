using Dreamporter.Core;
using SwagOverFlow.WPF.Controls;
using SwagOverFlow.WPF.UI;
using System;
using System.Collections.Generic;
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

namespace Dreamporter.WPF.Controls
{
    /// <summary>
    /// Interaction logic for OptionsTreeControl.xaml
    /// </summary>
    public partial class OptionsTreeControl : SwagControlBase
    {
        #region MoveType
        enum MoveType
        {
            None,
            Above,
            Below,
            Into
        }
        #endregion MoveType

        #region Properties
        #region AllowMove
        public static DependencyProperty AllowMoveProperty =
            DependencyProperty.Register(
                "AllowMove",
                typeof(Boolean),
                typeof(OptionsTreeControl));

        public Boolean AllowMove
        {
            get { return (Boolean)GetValue(AllowMoveProperty); }
            set { SetValue(AllowMoveProperty, value); }
        }
        #endregion AllowMove
        #region ShowSequence
        public static DependencyProperty ShowSequenceProperty =
            DependencyProperty.Register(
                "ShowSequence",
                typeof(Boolean),
                typeof(OptionsTreeControl));

        public Boolean ShowSequence
        {
            get { return (Boolean)GetValue(ShowSequenceProperty); }
            set
            {
                SetValue(ShowSequenceProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowSequence
        #region RootOptions
        public static DependencyProperty RootOptionsProperty =
            DependencyProperty.Register(
                "RootOptions",
                typeof(OptionsNode),
                typeof(OptionsTreeControl));

        public OptionsNode RootOptions
        {
            get { return (OptionsNode)GetValue(RootOptionsProperty); }
            set { SetValue(RootOptionsProperty, value); }
        }
        #endregion RootOptions
        #region SelectedOptionsNode
        public static DependencyProperty SelectedOptionsProperty =
            DependencyProperty.Register(
                "SelectedOptions",
                typeof(OptionsNode),
                typeof(OptionsTreeControl));

        public OptionsNode SelectedOptions
        {
            get { return (OptionsNode)GetValue(SelectedOptionsProperty); }
            set { SetValue(SelectedOptionsProperty, value); }
        }
        #endregion SelectedOptionsNode
        #region Save
        public static readonly RoutedEvent SaveEvent =
            EventManager.RegisterRoutedEvent(
            "Save",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(OptionsTreeControl));

        public event RoutedEventHandler Save
        {
            add { AddHandler(SaveEvent, value); }
            remove { RemoveHandler(SaveEvent, value); }
        }
        #endregion Save
        #region SaveCommand
        public static DependencyProperty SaveCommandProperty =
            DependencyProperty.Register(
                "SaveCommand",
                typeof(ICommand),
                typeof(OptionsTreeControl));

        public ICommand SaveCommand
        {
            get { return (ICommand)GetValue(SaveCommandProperty); }
            set { SetValue(SaveCommandProperty, value); }
        }
        #endregion SaveCommand
        #region ShowSaveButton
        public static DependencyProperty ShowSaveButtonProperty =
            DependencyProperty.Register(
                "ShowSaveButton",
                typeof(Boolean),
                typeof(OptionsTreeControl),
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
                typeof(OptionsTreeControl),
                new PropertyMetadata(false));

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
        #region ShowAddContextMenuItem
        public static DependencyProperty ShowAddContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowAddContextMenuItem",
                typeof(Boolean),
                typeof(OptionsTreeControl),
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
        #region ShowCopyContextMenuItem
        public static DependencyProperty ShowCopyContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowCopyContextMenuItem",
                typeof(Boolean),
                typeof(OptionsTreeControl),
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
        #region ShowPasteContextMenuItem
        public static DependencyProperty ShowPasteContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowPasteContextMenuItem",
                typeof(Boolean),
                typeof(OptionsTreeControl),
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
                typeof(OptionsTreeControl),
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
                typeof(OptionsTreeControl),
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
        #region ShowItemAddContextMenuItem
        public static DependencyProperty ShowItemAddContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowItemAddContextMenuItem",
                typeof(Boolean),
                typeof(OptionsTreeControl),
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
        #region ShowItemRemoveContextMenuItem
        public static DependencyProperty ShowItemRemoveContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowItemRemoveContextMenuItem",
                typeof(Boolean),
                typeof(OptionsTreeControl),
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
        #region ShowItemCopyContextMenuItem
        public static DependencyProperty ShowItemCopyContextMenuItemProperty =
            DependencyProperty.Register(
                "ShowItemCopyContextMenuItem",
                typeof(Boolean),
                typeof(OptionsTreeControl),
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
                typeof(OptionsTreeControl),
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
        #endregion Properties

        #region Initialization
        public OptionsTreeControl()
        {
            InitializeComponent();
        }
        #endregion Initialization

        #region Events
        private void SwagItemsControl_Add(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;
            OptionsNode node = (OptionsNode)fe.DataContext;
            OptionsNode newChild = new OptionsNode() { Display = "NEW" };
            node.Children.Add(newChild);
            CollectionViewSource.GetDefaultView(node.Children).Refresh();
        }

        private void SwagItemsControl_Clear(object sender, RoutedEventArgs e)
        {
            OptionsTreeControl otc = (OptionsTreeControl)e.OriginalSource;
            otc.RootOptions.Clear();
            CollectionViewSource.GetDefaultView(otc.RootOptions).Refresh();
        }

        private void SwagItemsControl_Remove(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;
            OptionsNode node = (OptionsNode)fe.DataContext;
            node.Parent.Children.Remove(node);
        }

        private void SwagItemsControl_TreeViewItemDropPreview(object sender, RoutedEventArgs e)
        {
            TreeViewItemDropEventArgs tviea = (TreeViewItemDropEventArgs)e;

            if (!CheckIfValidDrop(tviea))
            {
                return;
            }

            Boolean allowChildMove = false, allowSiblingMove = false;
            switch (tviea.TargetItem.DataContext)
            {
                case OptionsNode node:
                    allowSiblingMove = true;
                    allowChildMove = true;
                    break;
            }

            tviea.TargetItem.BorderBrush = (Brush)this.FindResource("MahApps.Brushes.Text");
            MoveType moveType = GetMoveType(tviea);

            switch (moveType)
            {
                case MoveType.Above:
                    if (allowSiblingMove)
                    {
                        tviea.TargetItem.BorderThickness = new Thickness(0, 1, 0, 0);
                    }
                    break;
                case MoveType.Below:
                    if (allowSiblingMove)
                    {
                        tviea.TargetItem.BorderThickness = new Thickness(0, 0, 0, 1);
                    }
                    break;
                case MoveType.Into:
                    if (allowChildMove)
                    {
                        tviea.TargetItem.BorderThickness = new Thickness(1, 0, 0, 0);
                    }
                    else
                    {
                        tviea.DragEventArgs.Effects = DragDropEffects.None;
                        tviea.DragEventArgs.Handled = true;
                    }
                    break;
            }
        }

        private void SwagItemsControl_TreeViewItemDrop(object sender, RoutedEventArgs e)
        {
            TreeViewItemDropEventArgs tviea = (TreeViewItemDropEventArgs)e;
            tviea.TargetItem.BorderThickness = new Thickness(0, 0, 0, 0);
            if (!CheckIfValidDrop(tviea))       //Drops are still possible because TreeViewItemDropPreview can be skipped
            {
                return;
            }

            MoveType moveType = GetMoveType(tviea);
            OptionsNode targetOptionsNode = (OptionsNode)tviea.TargetItem.DataContext;
            OptionsNode droppedOptionsNode = (OptionsNode)tviea.DroppedItem.DataContext;

            OptionsNode originalDroppedParent = (OptionsNode)droppedOptionsNode.Parent;
            Boolean sameParent = originalDroppedParent == targetOptionsNode.Parent;
            Int32 originalDroppedSequence = droppedOptionsNode.Sequence,
                currentDroppedSequence = droppedOptionsNode.Sequence,
                targetSequence = targetOptionsNode.Sequence,
                delta = targetSequence - originalDroppedSequence;

            switch (moveType)
            {
                case MoveType.Above:
                case MoveType.Below:
                    if (!sameParent)
                    {
                        originalDroppedParent.Children.Remove(droppedOptionsNode);
                        droppedOptionsNode.Sequence = -1;        //reset sequence
                        targetOptionsNode.Parent.Children.Add(droppedOptionsNode);
                        currentDroppedSequence = droppedOptionsNode.Sequence;
                        delta = targetSequence - currentDroppedSequence;
                    }

                    switch (moveType)
                    {
                        case MoveType.Above:
                            if (delta != 0)
                            {
                                foreach (OptionsNode sibling in targetOptionsNode.Parent.Children)
                                {
                                    if (sibling.Sequence >= Math.Min(currentDroppedSequence, targetSequence) && sibling.Sequence < Math.Max(currentDroppedSequence, targetSequence))
                                    {
                                        sibling.Sequence = (sibling.Sequence + (delta > 0 ? -1 : 1));
                                    }
                                }
                                droppedOptionsNode.Sequence = targetSequence + (delta > 0 ? -1 : 0);
                            }
                            break;
                        case MoveType.Below:
                            if (delta != 0)
                            {
                                foreach (OptionsNode sibling in targetOptionsNode.Parent.Children)
                                {
                                    if (sibling.Sequence > Math.Min(currentDroppedSequence, targetSequence) && sibling.Sequence <= Math.Max(currentDroppedSequence, targetSequence))
                                    {
                                        sibling.Sequence = (sibling.Sequence + (delta > 0 ? -1 : 1));
                                    }
                                }
                                droppedOptionsNode.Sequence = targetSequence + (delta > 0 ? 0 : 1);
                            }
                            break;
                    }
                    break;
                case MoveType.Into:
                    if (tviea.TargetItem.DataContext is OptionsNode grp)
                    {
                        originalDroppedParent.Children.Remove(droppedOptionsNode);
                        droppedOptionsNode.Sequence = -1;        //reset sequence
                        grp.Children.Add(droppedOptionsNode);
                    }
                    break;
            }
        }

        private void SwagItemsControl_TreeViewItemLeavePreview(object sender, RoutedEventArgs e)
        {
            TreeViewItemDropEventArgs tviea = (TreeViewItemDropEventArgs)e;
            tviea.TargetItem.BorderThickness = new Thickness(0, 0, 0, 0);
        }

        private void SwagItemsControl_Save(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SaveEvent));
        }

        private void OptionsSave_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SaveEvent));
        }

        private void SwagItemsControl_Copy(object sender, RoutedEventArgs e)
        {
            OptionsNode node = null;
            switch (e.OriginalSource)
            {
                case MenuItem mi:
                    node = (OptionsNode)mi.DataContext;
                    break;
                case OptionsTreeControl ic:
                    node = (OptionsNode)ic.RootOptions;
                    break;
            }

            if (node != null)
            {
                SwagItemsControlHelper.SetClipBoardData<OptionsNode>(node);
            }
        }

        private void SwagItemsControl_Paste(object sender, RoutedEventArgs e)
        {
            OptionsNode node = SwagItemsControlHelper.GetClipBoardData<OptionsNode>();
            node.Init();
            if (node != null)
            {
                switch (e.OriginalSource)
                {
                    case MenuItem mi:
                        OptionsNode original = (OptionsNode)mi.DataContext;
                        OptionsNode parent = original.Parent;
                        node.Sequence = original.Sequence;
                        parent.Children.Remove(original);
                        foreach (OptionsNode ins in parent.Children)
                        {
                            if (ins.Sequence >= node.Sequence)
                            {
                                ins.Sequence++;
                            }
                        }
                        parent.Children.Add(node);
                        break;
                    case OptionsTreeControl ic:
                        ic.RootOptions = (OptionsNode)node;
                        break;
                }
            }
        }

        private void SwagItemsControl_Export(object sender, RoutedEventArgs e)
        {
            OptionsTreeControl ic = (OptionsTreeControl)e.OriginalSource;
            OptionsNode node = (OptionsNode)ic.RootOptions;
            SwagItemsControlHelper.ExportDataToFile<OptionsNode>(node);
        }

        private void SwagItemsControl_Import(object sender, RoutedEventArgs e)
        {
            OptionsNode node = SwagItemsControlHelper.GetDataFromFile<OptionsNode>();
            if (node != null)
            {
                OptionsTreeControl ic = (OptionsTreeControl)e.OriginalSource;
                ic.RootOptions = (OptionsNode)node;
            }
        }
        #endregion Events

        #region Methods
        private Boolean CheckIfValidDrop(TreeViewItemDropEventArgs tviea)
        {
            OptionsNode targetOptionsNode = (OptionsNode)tviea.TargetItem.DataContext;
            OptionsNode droppedOptionsNode = (OptionsNode)tviea.DroppedItem.DataContext;
            Boolean valid = true;
            MoveType moveType = GetMoveType(tviea);

            OptionsNode tempOpt = targetOptionsNode.Parent;

            while (valid && tempOpt != null)
            {
                if (droppedOptionsNode == tempOpt) //Don't drop within own descendants
                {
                    valid = false;
                    break;
                }
                tempOpt = tempOpt.Parent;
            }

            if (!valid)
            {
                tviea.DragEventArgs.Effects = DragDropEffects.None;
                tviea.DragEventArgs.Handled = true;
            }

            return valid;
        }

        private MoveType GetMoveType(TreeViewItemDropEventArgs tviea)
        {
            MoveType moveType = MoveType.None;

            FrameworkElement header = tviea.TargetItem.FindVisualChild<FrameworkElement>("PART_Header");
            Point p = tviea.DragEventArgs.GetPosition(header);
            Double quarter = header.ActualHeight / 4;

            if (p.Y + 2 < quarter * 1)
            {
                moveType = MoveType.Above;
            }
            else if (p.Y - 2 > quarter * 3)
            {
                moveType = MoveType.Below;
            }
            else
            {
                moveType = MoveType.Into;
            }

            return moveType;
        }
        #endregion Methods

    }
}
