﻿using Dreamporter.Instructions;
using Dreamporter.WPF.ViewModels;
using SwagOverFlow.WPF.Controls;
using SwagOverFlow.WPF.UI;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Dreamporter.WPF.Controls
{
    /// <summary>
    /// Interaction logic for InstructionControl.xaml
    /// </summary>
    public partial class InstructionControl : SwagControlBase
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
                typeof(InstructionControl));

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
                typeof(InstructionControl));

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
        #region RootInstruction
        public static DependencyProperty RootInstructionProperty =
            DependencyProperty.Register(
                "RootInstruction",
                typeof(GroupInstructionWPF),
                typeof(InstructionControl));

        public GroupInstructionWPF RootInstruction
        {
            get { return (GroupInstructionWPF)GetValue(RootInstructionProperty); }
            set { SetValue(RootInstructionProperty, value); }
        }
        #endregion RootInstruction
        #region SelectedInstruction
        public static DependencyProperty SelectedInstructionProperty =
            DependencyProperty.Register(
                "SelectedInstruction",
                typeof(Instruction),
                typeof(InstructionControl));

        public Instruction SelectedInstruction
        {
            get { return (Instruction)GetValue(SelectedInstructionProperty); }
            set { SetValue(SelectedInstructionProperty, value); }
        }
        #endregion SelectedInstruction
        #region Save
        public static readonly RoutedEvent SaveEvent =
            EventManager.RegisterRoutedEvent(
            "Save",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(InstructionControl));

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
                typeof(InstructionControl));

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
                typeof(InstructionControl),
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
                typeof(InstructionControl),
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
                typeof(InstructionControl),
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
                typeof(InstructionControl),
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
                typeof(InstructionControl),
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
                typeof(InstructionControl),
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
                typeof(InstructionControl),
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
                typeof(InstructionControl),
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
                typeof(InstructionControl),
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
                typeof(InstructionControl),
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
                typeof(InstructionControl),
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
        public InstructionControl()
        {
            InitializeComponent();
        }
        #endregion Initialization

        #region Events
        private void SwagItemsControl_Add(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;
            GroupInstruction insGrp = (GroupInstruction)fe.DataContext;

            Instruction ins = null;
            String tag = (fe.Tag ?? "").ToString();
            switch (tag)
            {
                case "GRP_INS":
                    ins = new GroupInstructionWPF();
                    break;
                case "GRP_TBL":
                    ins = new ForEachTableGroupInstructionWPF();
                    break;
                case "LF_QRY":
                    ins = new QueryInstruction();
                    break;
                case "LF_SQL_SP":
                    ins = new SqlExecSPInstruction();
                    break;
                case "LF_WR_WRI":
                    ins = new WebRequestInstruction();
                    break;
                case "LF_WR_WRGRP":
                    ins = new ForEachTableWebRequestInstruction();
                    break;
            }

            ins.Display = "NEW";
            insGrp.Children.Add(ins);
        }

        private void SwagItemsControl_Clear(object sender, RoutedEventArgs e)
        {
            InstructionControl ic = (InstructionControl)e.OriginalSource;
            ic.RootInstruction.Children.Clear();
        }

        private void SwagItemsControl_Remove(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;
            Instruction instruction = (Instruction)fe.DataContext;
            instruction.Parent.Children.Remove(instruction);
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
                case GroupInstruction grp:
                    allowChildMove = true;
                    allowSiblingMove = true;
                    break;
                case Instruction instruction:
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
            Instruction targetInstruction = (Instruction)tviea.TargetItem.DataContext;
            Instruction droppedInstruction = (Instruction)tviea.DroppedItem.DataContext;

            GroupInstruction originalDroppedParent = (GroupInstruction)droppedInstruction.Parent;
            Boolean sameParent = originalDroppedParent == targetInstruction.Parent;
            Int32 originalDroppedSequence = droppedInstruction.Sequence,
                currentDroppedSequence = droppedInstruction.Sequence,
                targetSequence = targetInstruction.Sequence,
                delta = targetSequence - originalDroppedSequence;

            switch (moveType)
            {
                case MoveType.Above:
                case MoveType.Below:
                    if (!sameParent)
                    {
                        originalDroppedParent.Children.Remove(droppedInstruction);
                        droppedInstruction.Sequence = -1;        //reset sequence
                        targetInstruction.Parent.Children.Add(droppedInstruction);
                        currentDroppedSequence = droppedInstruction.Sequence;
                        delta = targetSequence - currentDroppedSequence;
                    }

                    switch (moveType)
                    {
                        case MoveType.Above:
                            if (delta != 0)
                            {
                                foreach (Instruction sibling in targetInstruction.Parent.Children)
                                {
                                    if (sibling.Sequence >= Math.Min(currentDroppedSequence, targetSequence) && sibling.Sequence < Math.Max(currentDroppedSequence, targetSequence))
                                    {
                                        sibling.Sequence = (sibling.Sequence + (delta > 0 ? -1 : 1));
                                    }
                                }
                                droppedInstruction.Sequence = targetSequence + (delta > 0 ? -1 : 0);
                            }
                            break;
                        case MoveType.Below:
                            if (delta != 0)
                            {
                                foreach (Instruction sibling in targetInstruction.Parent.Children)
                                {
                                    if (sibling.Sequence > Math.Min(currentDroppedSequence, targetSequence) && sibling.Sequence <= Math.Max(currentDroppedSequence, targetSequence))
                                    {
                                        sibling.Sequence = (sibling.Sequence + (delta > 0 ? -1 : 1));
                                    }
                                }
                                droppedInstruction.Sequence = targetSequence + (delta > 0 ? 0 : 1);
                            }
                            break;
                    }
                    break;
                case MoveType.Into:
                    if (tviea.TargetItem.DataContext is GroupInstruction grp)
                    {
                        originalDroppedParent.Children.Remove(droppedInstruction);
                        droppedInstruction.Sequence = -1;        //reset sequence
                        grp.Children.Add(droppedInstruction);
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
            Instruction instruction = null;
            switch (e.OriginalSource)
            {
                case MenuItem mi:
                    instruction = (Instruction)mi.DataContext;
                    break;
                case InstructionControl ic:
                    instruction = (Instruction)ic.RootInstruction;
                    break;
            }

            if (instruction != null)
            {
                SwagItemsControlHelper.SetClipBoardData<Instruction>(instruction);
            }
        }

        private void SwagItemsControl_Paste(object sender, RoutedEventArgs e)
        {
            Instruction instruction = SwagItemsControlHelper.GetClipBoardData<Instruction>();
            if (instruction != null)
            {
                switch (e.OriginalSource)
                {
                    case MenuItem mi:
                        Instruction original = (Instruction)mi.DataContext;
                        GroupInstruction parent = original.Parent;
                        instruction.Sequence = original.Sequence;
                        parent.Children.Remove(original);
                        foreach (Instruction ins in parent.Children)
                        {
                            if (ins.Sequence >= instruction.Sequence)
                            {
                                ins.Sequence++;
                            }
                        }
                        parent.Children.Add(instruction);
                        break;
                    case InstructionControl ic:
                        ic.RootInstruction = (GroupInstructionWPF)instruction;
                        break;
                }
            }
        }

        private void SwagItemsControl_Export(object sender, RoutedEventArgs e)
        {
            InstructionControl ic = (InstructionControl)e.OriginalSource;
            Instruction instruction = (Instruction)ic.RootInstruction;
            SwagItemsControlHelper.ExportDataToFile<Instruction>(instruction);
        }

        private void SwagItemsControl_Import(object sender, RoutedEventArgs e)
        {
            Instruction instruction = SwagItemsControlHelper.GetDataFromFile<Instruction>();
            if (instruction != null)
            {
                InstructionControl ic = (InstructionControl)e.OriginalSource;
                ic.RootInstruction = (GroupInstructionWPF)instruction;
            }
        }
        #endregion Events

        #region Methods
        private Boolean CheckIfValidDrop(TreeViewItemDropEventArgs tviea)
        {
            Instruction targetInstruction = (Instruction)tviea.TargetItem.DataContext;
            Instruction droppedInstruction = (Instruction)tviea.DroppedItem.DataContext;
            Boolean valid = true;
            MoveType moveType = GetMoveType(tviea);

            Instruction tempOpt = targetInstruction.Parent;

            while (valid && tempOpt != null)
            {
                if (droppedInstruction == tempOpt) //Don't drop within own descendants
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