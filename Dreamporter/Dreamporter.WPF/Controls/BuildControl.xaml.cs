using Dreamporter.Core;
using SwagOverFlow.Iterator;
using SwagOverFlow.WPF.Controls;
using SwagOverFlow.WPF.UI;
using System;
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

namespace Dreamporter.WPF.Controls
{
    /// <summary>
    /// Interaction logic for BuildControl.xaml
    /// </summary>
    public partial class BuildControl : SwagControlBase
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
                typeof(BuildControl));

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
                typeof(BuildControl));

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
        #region RootBuild
        public static DependencyProperty RootBuildProperty =
            DependencyProperty.Register(
                "RootBuild",
                typeof(GroupBuild),
                typeof(BuildControl));

        public GroupBuild RootBuild
        {
            get { return (GroupBuild)GetValue(RootBuildProperty); }
            set { SetValue(RootBuildProperty, value); }
        }
        #endregion RootBuild
        #region SelectedBuild
        public static DependencyProperty SelectedBuildProperty =
            DependencyProperty.Register(
                "SelectedBuild",
                typeof(Build),
                typeof(BuildControl), 
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault) 
                { 
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, 
                    PropertyChangedCallback = SelectedBuildPropertyChanged 
                });

        private static void SelectedBuildPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Build bldOld = (Build)e.OldValue;
            Build bldNew = (Build)e.NewValue;
            if (bldOld != null && bldNew != null && !(bldOld is GroupBuild && bldNew is InstructionBuild))
            {
                bldNew.TabIndex = bldOld.TabIndex;
            }
        }

        public Build SelectedBuild
        {
            get { return (Build)GetValue(SelectedBuildProperty); }
            set 
            { 
                SetValue(SelectedBuildProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion SelectedBuild
        #region Save
        public static readonly RoutedEvent SaveEvent =
            EventManager.RegisterRoutedEvent(
            "Save",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(BuildControl));

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
                typeof(BuildControl));

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
                typeof(BuildControl),
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
                typeof(BuildControl),
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
                typeof(BuildControl),
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
                typeof(BuildControl),
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
                typeof(BuildControl),
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
                typeof(BuildControl),
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
                typeof(BuildControl),
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
                typeof(BuildControl),
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
                typeof(BuildControl),
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
                typeof(BuildControl),
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
                typeof(BuildControl),
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
        public BuildControl()
        {
            InitializeComponent();
        }
        #endregion Initialization

        #region Events
        private void SwagItemsControl_Add(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;
            GroupBuild grp = (GroupBuild)fe.DataContext;

            Build bld = null;
            String tag = (fe.Tag ?? "").ToString();
            switch (tag)
            {
                case "GRP_BLD":
                    bld = new GroupBuild();
                    break;
                case "BLD_INS":
                    bld = new InstructionBuild();
                    break;
            }

            bld.Name = $"NEW {grp.Children.Count + 1}";
            grp.Children.Add(bld);
        }

        private void SwagItemsControl_Clear(object sender, RoutedEventArgs e)
        {
            BuildControl bc = (BuildControl)e.OriginalSource;
            bc.RootBuild.Children.Clear();
        }

        private void SwagItemsControl_Remove(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;
            Build build = (Build)fe.DataContext;
            build.Parent.Children.Remove(build);
        }

        private void SwagItemsControl_TreeViewItemDropPreview(object sender, RoutedEventArgs e)
        {
            TreeViewItemDropEventArgs tviea = (TreeViewItemDropEventArgs)e;

            if (tviea.TargetItem == tviea.DroppedItem)
            {
                tviea.DragEventArgs.Effects = DragDropEffects.None;
                tviea.DragEventArgs.Handled = true;
            }

            if (!CheckIfValidDrop(tviea))
            {
                return;
            }

            Boolean allowChildMove = false, allowSiblingMove = false;
            switch (tviea.TargetItem.DataContext)
            {
                case GroupBuild grp:
                    allowChildMove = true;
                    allowSiblingMove = true;
                    break;
                case Build build:
                    allowSiblingMove = true;
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
            Build targetBuild = (Build)tviea.TargetItem.DataContext;
            Build droppedBuild = (Build)tviea.DroppedItem.DataContext;

            GroupBuild originalDroppedParent = (GroupBuild)droppedBuild.Parent;
            Boolean sameParent = originalDroppedParent == targetBuild.Parent;
            Int32 originalDroppedSequence = droppedBuild.Sequence,
                currentDroppedSequence = droppedBuild.Sequence,
                targetSequence = targetBuild.Sequence,
                delta = targetSequence - originalDroppedSequence;

            switch (moveType)
            {
                case MoveType.Above:
                case MoveType.Below:
                    if (!sameParent)
                    {
                        originalDroppedParent.Children.Remove(droppedBuild);
                        droppedBuild.Sequence = -1;        //reset sequence
                        targetBuild.Parent.Children.Add(droppedBuild);
                        currentDroppedSequence = droppedBuild.Sequence;
                        delta = targetSequence - currentDroppedSequence;
                    }

                    switch (moveType)
                    {
                        case MoveType.Above:
                            if (delta != 0)
                            {
                                foreach (Build sibling in targetBuild.Parent.Children)
                                {
                                    if (sibling.Sequence >= Math.Min(currentDroppedSequence, targetSequence) && sibling.Sequence < Math.Max(currentDroppedSequence, targetSequence))
                                    {
                                        sibling.Sequence = (sibling.Sequence + (delta > 0 ? -1 : 1));
                                    }
                                }
                                droppedBuild.Sequence = targetSequence + (delta > 0 ? -1 : 0);
                            }
                            break;
                        case MoveType.Below:
                            if (delta != 0)
                            {
                                foreach (Build sibling in targetBuild.Parent.Children)
                                {
                                    if (sibling.Sequence > Math.Min(currentDroppedSequence, targetSequence) && sibling.Sequence <= Math.Max(currentDroppedSequence, targetSequence))
                                    {
                                        sibling.Sequence = (sibling.Sequence + (delta > 0 ? -1 : 1));
                                    }
                                }
                                droppedBuild.Sequence = targetSequence + (delta > 0 ? 0 : 1);
                            }
                            break;
                    }
                    CollectionViewSource.GetDefaultView(targetBuild.Parent.Children).Refresh();
                    break;
                case MoveType.Into:
                    if (tviea.TargetItem.DataContext is GroupBuild grp)
                    {
                        originalDroppedParent.Children.Remove(droppedBuild);
                        droppedBuild.Sequence = -1;        //reset sequence
                        grp.Children.Add(droppedBuild);
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

        private void SwagItemsControl_Copy(object sender, RoutedEventArgs e)
        {
            Build build = null;
            switch (e.OriginalSource)
            {
                case MenuItem mi:
                    build = (Build)mi.DataContext;
                    break;
                case BuildControl ic:
                    build = (Build)ic.RootBuild;
                    break;
            }

            if (build != null)
            {
                SwagItemsControlHelper.SetClipBoardData(build);
            }
        }

        private void SwagItemsControl_Paste(object sender, RoutedEventArgs e)
        {
            Build build = SwagItemsControlHelper.GetClipBoardData<Build>();

            switch (build)
            {
                case GroupBuild grp:
                    SwagItemPreOrderIterator<Build> iterator = new SwagItemPreOrderIterator<Build>(grp);
                    for (Build b = iterator.First(); !iterator.IsDone; b = iterator.Next())
                    {
                        b.BuildId = 0;
                        b.Integration = null;
                        b.IntegrationId = null;
                    }
                    break;
                default:
                    build.BuildId = 0;
                    build.Integration = null;
                    build.IntegrationId = null;
                    break;
            }
            
            if (build != null)
            {
                switch (e.OriginalSource)
                {
                    case MenuItem mi:
                        Build original = (Build)mi.DataContext;
                        GroupBuild parent = original.Parent;
                        build.Sequence = original.Sequence;
                        parent.Children.Remove(original);
                        foreach (Build ins in parent.Children)
                        {
                            if (ins.Sequence >= build.Sequence)
                            {
                                ins.Sequence++;
                            }
                        }
                        parent.Children.Add(build);
                        break;
                    case BuildControl ic:
                        ic.RootBuild = (GroupBuild)build;
                        break;
                }
            }
        }

        private void SwagItemsControl_Export(object sender, RoutedEventArgs e)
        {
            BuildControl ic = (BuildControl)e.OriginalSource;
            Build build = (Build)ic.RootBuild;
            SwagItemsControlHelper.ExportDataToFile<Build>(build);
        }

        private void SwagItemsControl_Import(object sender, RoutedEventArgs e)
        {
            Build build = SwagItemsControlHelper.GetDataFromFile<Build>();

            switch (build)
            {
                case GroupBuild grp:
                    SwagItemPreOrderIterator<Build> iterator = new SwagItemPreOrderIterator<Build>(grp);
                    for (Build b = iterator.First(); !iterator.IsDone; b = iterator.Next())
                    {
                        b.BuildId = 0;
                        b.Integration = null;
                        b.IntegrationId = null;
                    }
                    break;
                default:
                    build.BuildId = 0;
                    build.Integration = null;
                    build.IntegrationId = null;
                    break;
            }

            if (build != null)
            {
                BuildControl ic = (BuildControl)e.OriginalSource;
                ic.RootBuild = (GroupBuild)build;
            }
        }

        private void BooleanExpressionControl_Save(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SaveEvent));
        }

        private void SchemasControl_Save(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SaveEvent));
        }

        private void InstructionControl_Save(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SaveEvent));
        }
        #endregion Events

        #region Methods
        private Boolean CheckIfValidDrop(TreeViewItemDropEventArgs tviea)
        {
            Build targetBuild = (Build)tviea.TargetItem.DataContext;
            Build droppedBuild = (Build)tviea.DroppedItem.DataContext;
            Boolean valid = true;
            MoveType moveType = GetMoveType(tviea);

            if (targetBuild == droppedBuild)    //Don't drop to self
            {
                return false;
            }

            Build tempOpt = targetBuild.Parent;

            while (valid && tempOpt != null)
            {
                if (droppedBuild == tempOpt) //Don't drop within own descendants
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
