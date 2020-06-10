﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MahApps.Metro.Controls;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.UI;
using SwagOverFlow.WPF.ViewModels;

namespace SwagOverFlow.WPF.Controls
{
    /// <summary>
    /// Interaction logic for SwagOptionControl.xaml
    /// </summary>
    public partial class SwagOptionControl : SwagControlBase
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
                typeof(SwagOptionControl));

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
                typeof(SwagOptionControl));

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
        #region RootOption
        public static DependencyProperty RootOptionProperty =
            DependencyProperty.Register(
                "RootOption",
                typeof(SwagOptionGroupWPF),
                typeof(SwagOptionControl));

        public SwagOptionGroupWPF RootOption
        {
            get { return (SwagOptionGroupWPF)GetValue(RootOptionProperty); }
            set { SetValue(RootOptionProperty, value); }
        }
        #endregion RootOption
        #region Save
        public static readonly RoutedEvent SaveEvent =
            EventManager.RegisterRoutedEvent(
            "Save",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SwagOptionControl));

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
                typeof(SwagOptionControl));

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
                typeof(SwagOptionControl),
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
                typeof(SwagOptionControl),
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
                typeof(SwagOptionControl),
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
                typeof(SwagOptionControl),
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
                typeof(SwagOptionControl),
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
                typeof(SwagOptionControl),
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
                typeof(SwagOptionControl),
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
                typeof(SwagOptionControl),
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
                typeof(SwagOptionControl),
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
        #endregion Properties

        #region Initialization
        public SwagOptionControl()
        {
            InitializeComponent();
        }
        #endregion Initialization

        #region Events
        private void SwagItemsControl_Add(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;
            SwagOptionGroup optGroup = (SwagOptionGroup)fe.DataContext;

            SwagOption opt = null;
            String tag = (fe.Tag ?? "").ToString();
            switch (tag)
            {
                case "GROUP":
                    opt = new SwagOptionGroupWPF();
                    break;
                case "STRING":
                    opt = new StringOption();
                    break;
                case "DATE":
                    opt = new DateOption();
                    break;
                case "BOOL":
                    opt = new BooleanOption();
                    break;
            }

            optGroup.Children.Add(opt);
        }

        private void SwagItemsControl_Clear(object sender, RoutedEventArgs e)
        {
            SwagOptionControl soc = (SwagOptionControl)e.OriginalSource;
            soc.RootOption.Children.Clear();
        }

        private void SwagItemsControl_Remove(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;
            SwagOption opt = (SwagOption)fe.DataContext;
            opt.Parent.Children.Remove(opt);
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
                case BooleanOperationExpression opExp:
                    allowChildMove = true;
                    allowSiblingMove = true;
                    break;
                case BooleanBooleanVariableExpression bbvExp:
                    allowSiblingMove = true;
                    break;
                case BooleanStringVariableExpression bsvExp:
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
            BooleanExpression targetExpression = (BooleanExpression)tviea.TargetItem.DataContext;
            BooleanExpression droppedExpression = (BooleanExpression)tviea.DroppedItem.DataContext;

            BooleanOperationExpression originalDroppedParent = droppedExpression.Parent;
            Boolean sameParent = originalDroppedParent == targetExpression.Parent;
            Int32 originalDroppedSequence = droppedExpression.Sequence,
                currentDroppedSequence = droppedExpression.Sequence,
                targetSequence = targetExpression.Sequence,
                delta = targetSequence - originalDroppedSequence;

            switch (moveType)
            {
                case MoveType.Above:
                case MoveType.Below:
                    if (!sameParent)
                    {
                        originalDroppedParent.Children.Remove(droppedExpression);
                        droppedExpression.Sequence = -1;        //reset sequence
                        targetExpression.Parent.Children.Add(droppedExpression);
                        currentDroppedSequence = droppedExpression.Sequence;
                        delta = targetSequence - currentDroppedSequence;
                    }

                    switch (moveType)
                    {
                        case MoveType.Above:
                            if (delta != 0)
                            {
                                foreach (BooleanExpression sibling in targetExpression.Parent.Children)
                                {
                                    if (sibling.Sequence >= Math.Min(currentDroppedSequence, targetSequence) && sibling.Sequence < Math.Max(currentDroppedSequence, targetSequence))
                                    {
                                        sibling.Sequence = (sibling.Sequence + (delta > 0 ? -1 : 1));
                                    }
                                }
                                droppedExpression.Sequence = targetSequence + (delta > 0 ? -1 : 0);
                            }
                            break;
                        case MoveType.Below:
                            if (delta != 0)
                            {
                                foreach (BooleanExpression sibling in targetExpression.Parent.Children)
                                {
                                    if (sibling.Sequence > Math.Min(currentDroppedSequence, targetSequence) && sibling.Sequence <= Math.Max(currentDroppedSequence, targetSequence))
                                    {
                                        sibling.Sequence = (sibling.Sequence + (delta > 0 ? -1 : 1));
                                    }
                                }
                                droppedExpression.Sequence = targetSequence + (delta > 0 ? 0 : 1);
                            }
                            break;
                    }
                    break;
                case MoveType.Into:
                    if (tviea.TargetItem.DataContext is BooleanOperationExpression opExp)
                    {
                        originalDroppedParent.Children.Remove(droppedExpression);
                        droppedExpression.Sequence = -1;        //reset sequence
                        opExp.Children.Add(droppedExpression);
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
            SwagOptionControl sic = (SwagOptionControl)e.OriginalSource;
            SwagOption exp = (SwagOption)sic.RootOption;
            SwagItemsControlHelper.SetClipBoardData<SwagOption>(exp);
        }

        private void SwagItemsControl_Paste(object sender, RoutedEventArgs e)
        {
            SwagOption opt = SwagItemsControlHelper.GetClipBoardData<SwagOption>();
            if (opt != null)
            {
                SwagOptionControl soc = (SwagOptionControl)e.OriginalSource;
                soc.RootOption = (SwagOptionGroupWPF)opt;
            }
        }

        private void SwagItemsControl_Export(object sender, RoutedEventArgs e)
        {
            SwagOptionControl sic = (SwagOptionControl)e.OriginalSource;
            SwagOption opt = (SwagOption)sic.RootOption;
            SwagItemsControlHelper.ExportDataToFile<SwagOption>(opt);
        }

        private void SwagItemsControl_Import(object sender, RoutedEventArgs e)
        {
            SwagOption opt = SwagItemsControlHelper.GetDataFromFile<SwagOption>();
            if (opt != null)
            {
                SwagOptionControl soc = (SwagOptionControl)e.OriginalSource;
                soc.RootOption = (SwagOptionGroupWPF)opt;
            }
        }
        #endregion Events

        #region Methods
        private Boolean CheckIfValidDrop(TreeViewItemDropEventArgs tviea)
        {
            BooleanExpression targetExp = (BooleanExpression)tviea.TargetItem.DataContext;
            BooleanExpression droppedExp = (BooleanExpression)tviea.DroppedItem.DataContext;
            Boolean valid = true;
            MoveType moveType = GetMoveType(tviea);

            //if ((targetExp == RootExpression && (moveType == MoveType.Above || moveType == MoveType.Below)) ||  //Don't drop as root sibling
            //    targetExp == droppedExp)        //Don't drop to self
            //{
            //    valid = false;
            //}

            BooleanExpression tempExp = targetExp.Parent;

            while (valid && tempExp != null)
            {
                if (droppedExp == tempExp) //Don't drop within own descendants
                {
                    valid = false;
                    break;
                }
                tempExp = tempExp.Parent;
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