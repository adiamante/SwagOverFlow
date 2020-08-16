using ControlzEx.Standard;
using Dreamporter.Core;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Clients;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.Commands;
using SwagOverFlow.WPF.Controls;
using SwagOverFlow.WPF.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Dreamporter.WPF.Controls
{
    /// <summary>
    /// Interaction logic for InstructionControl.xaml
    /// </summary>
    public partial class InstructionControl : SwagControlBase
    {
        Int32 _currentTabIndex = 0;

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
        #region InstructionCollection
        public static DependencyProperty InstructionCollectionProperty =
            DependencyProperty.Register(
                "InstructionCollection",
                typeof(ICollection<Instruction>),
                typeof(InstructionControl),
                new FrameworkPropertyMetadata(null, InstructionCollectionProperty_Changed));

        private static void InstructionCollectionProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InstructionControl ic = (InstructionControl)d;

            if (ic.InstructionCollection is GroupInstruction grpIns && !grpIns.IsInitialized)
            {
                grpIns.SwagItemChanged += InstructionCollection_SwagItemChanged;
                grpIns.IsInitialized = true;
            }
        }

        private static void InstructionCollection_SwagItemChanged(object sender, SwagItemChangedEventArgs e)
        {
            if (!SwagWindow.CommandManager.IsFrozen)
            {
                Boolean canUndo = true;

                switch (e.PropertyChangedArgs)
                {
                    case PropertyChangedExtendedEventArgs exArgs:
                        #region General
                        switch (exArgs.Object)
                        {
                            case Instruction ix:
                                switch (exArgs.PropertyName)
                                {
                                    case "Parent":
                                    case "IsSelected":
                                    case "IsExpanded":
                                        canUndo = false;
                                        break;
                                    case "Sequence":
                                        if ((Int32)exArgs.OldValue == -1)
                                        {
                                            canUndo = false;
                                        }
                                        break;
                                }
                                break;
                        }
                        #endregion General
                        #region SwagPropertyChangedCommand
                        if (canUndo)
                        {
                            SwagPropertyChangedCommand cmd = new SwagPropertyChangedCommand(
                                exArgs.PropertyName,
                                exArgs.Object,
                                exArgs.OldValue,
                                exArgs.NewValue);
                            cmd.Display = e.Message;

                            SwagWindow.CommandManager.AddCommand(cmd);
                        }
                        #endregion SwagPropertyChangedCommand
                        break;
                    case CollectionPropertyChangedEventArgs colArgs:
                        #region SwagCollectionPropertyChangedCommand
                        if (canUndo)
                        {
                            SwagCollectionPropertyChangedCommand cmd = new SwagCollectionPropertyChangedCommand(
                                colArgs.PropertyName,
                                colArgs.Object,
                                colArgs.OldItems,
                                colArgs.NewItems);
                            cmd.Display = e.Message;

                            SwagWindow.CommandManager.AddCommand(cmd);
                        }
                        #endregion SwagCollectionPropertyChangedCommand
                        break;
                }
            }
        }

        public ICollection<Instruction> InstructionCollection
        {
            get { return (ICollection<Instruction>)GetValue(InstructionCollectionProperty); }
            set { SetValue(InstructionCollectionProperty, value); }
        }
        #endregion InstructionCollection
        #region SelectedInstruction
        public static DependencyProperty SelectedInstructionProperty =
            DependencyProperty.Register(
                "SelectedInstruction",
                typeof(Instruction),
                typeof(InstructionControl), 
                new FrameworkPropertyMetadata(null, SelectedInstruction_PropertyChanged));

        private static void SelectedInstruction_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InstructionControl ic = (InstructionControl)d;
            Instruction insOld = (Instruction)e.OldValue;
            Instruction insNew = (Instruction)e.NewValue;
            if (insOld != null && insNew != null && insOld.GetType().Name == insNew.GetType().Name)
            {
                insNew.TabIndex = insOld.TabIndex;
            }

            if (insOld != null && insNew == null)
            {
                ic._currentTabIndex = insOld.TabIndex;
            }

            if (insOld == null && insNew != null)
            {
                insNew.TabIndex = ic._currentTabIndex;
            }
        }

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
            ICollection<Instruction> instructions = (ICollection<Instruction>)fe.DataContext;

            Instruction ins = null;
            String tag = (fe.Tag ?? "").ToString();
            switch (tag)
            {
                case "GRP_INS":
                    ins = new GroupInstruction();
                    break;
                case "GRP_TBL":
                    ins = new ForEachTableGroupInstruction();
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
                case "LF_UT_SRP":
                    ins = new SetRunParamsInstruction();
                    break;
            }

            ins.Name = $"NEW {instructions.Count + 1}";
            instructions.Add(ins);
            CollectionViewSource.GetDefaultView(instructions).Refresh();
        }

        private void SwagItemsControl_Clear(object sender, RoutedEventArgs e)
        {
            InstructionControl ic = (InstructionControl)e.OriginalSource;
            ic.InstructionCollection.Clear();
            CollectionViewSource.GetDefaultView(ic.InstructionCollection).Refresh();
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
                                if (targetInstruction.Parent == null)
                                {
                                    CollectionViewSource.GetDefaultView(InstructionCollection).Refresh();
                                }
                                else
                                {
                                    CollectionViewSource.GetDefaultView(targetInstruction.Parent.Children).Refresh();
                                }
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
                                if (targetInstruction.Parent == null)
                                {
                                    CollectionViewSource.GetDefaultView(InstructionCollection).Refresh();
                                }
                                else
                                {
                                    CollectionViewSource.GetDefaultView(targetInstruction.Parent.Children).Refresh();
                                }
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
                    instruction = (Instruction)ic.InstructionCollection;
                    break;
            }

            if (instruction != null)
            {
                String originalStatus = instruction.Status;
                instruction.Status = new JObject(
                    new JProperty("Path",  instruction.Path)
                    ).ToString();
                SwagItemsControlHelper.SetClipBoardData<Instruction>(instruction);
                instruction.Status = originalStatus;
            }
        }

        private void SwagItemsControl_Paste(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;
            Instruction instruction = SwagItemsControlHelper.GetClipBoardData<Instruction>();

            if (instruction != null)
            {
                JObject jStatus = JObject.Parse(instruction.Status);
                instruction.Status = "";
                switch (fe.Tag)
                {
                    case "TEMPLATE":
                        TemplateInstruction templateInstruction = new TemplateInstruction();
                        templateInstruction.Name = instruction.Name;
                        GroupInstruction groupInstruction = new GroupInstruction();
                        groupInstruction.Children.Add(instruction);
                        templateInstruction.Template = groupInstruction;
                        templateInstruction.TemplateKey = jStatus["Path"].ToString();
                        instruction = templateInstruction;
                        break;
                }

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
                        CollectionViewSource.GetDefaultView(parent.Children).Refresh();
                        break;
                    case InstructionControl ic:
                        ic.InstructionCollection = (GroupInstruction)instruction;
                        break;
                }
            }
        }

        private void SwagItemsControl_Export(object sender, RoutedEventArgs e)
        {
            InstructionControl ic = (InstructionControl)e.OriginalSource;
            Instruction instruction = (Instruction)ic.InstructionCollection;
            SwagItemsControlHelper.ExportDataToFile<Instruction>(instruction);
        }

        private void SwagItemsControl_Import(object sender, RoutedEventArgs e)
        {
            Instruction instruction = SwagItemsControlHelper.GetDataFromFile<Instruction>();
            if (instruction != null)
            {
                InstructionControl ic = (InstructionControl)e.OriginalSource;
                ic.InstructionCollection = (GroupInstruction)instruction;
            }
        }

        private void SwagOptionControl_Save(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SaveEvent));
        }

        private void BooleanExpressionControl_Save(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SaveEvent));
        }

        private void SchemasControl_Save(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SaveEvent));
        }

        private void SqlParams_Import(object sender, RoutedEventArgs e)
        {
            SwagItemsControl sic = (SwagItemsControl)sender;
            String txt = Clipboard.GetText();
            try
            {
                List<SqlParam> sqlParams = JsonHelper.ToObject<List<SqlParam>>(txt);
                sic.SwagItemsSource = sqlParams;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
