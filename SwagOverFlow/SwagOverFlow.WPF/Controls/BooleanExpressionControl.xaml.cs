using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MahApps.Metro.Controls;
using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.UI;
using SwagOverFlow.WPF.ViewModels;

namespace SwagOverFlow.WPF.Controls
{
    /// <summary>
    /// Interaction logic for BooleanExpressionControl.xaml
    /// </summary>
    public partial class BooleanExpressionControl : SwagControlBase
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

        #region AllowMove
        public static DependencyProperty AllowMoveProperty =
            DependencyProperty.Register(
                "AllowMove",
                typeof(Boolean),
                typeof(BooleanExpressionControl));

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
                typeof(BooleanExpressionControl));

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

        #region Expression
        public static DependencyProperty ExpressionProperty =
            DependencyProperty.Register(
                "Expression",
                typeof(BooleanExpression),
                typeof(BooleanExpressionControl));

        public BooleanExpression Expression
        {
            get { return (BooleanExpression)GetValue(ExpressionProperty); }
            set { SetValue(ExpressionProperty, value); }
        }
        #endregion Expression

        #region Save
        public static readonly RoutedEvent SaveEvent =
            EventManager.RegisterRoutedEvent(
            "Save",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(BooleanExpressionControl));

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
                typeof(BooleanExpressionControl),
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
                typeof(BooleanExpressionControl),
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

        #region SaveCommand
        public static DependencyProperty SaveCommandProperty =
            DependencyProperty.Register(
                "SaveCommand",
                typeof(ICommand),
                typeof(BooleanExpressionControl));

        public ICommand SaveCommand
        {
            get { return (ICommand)GetValue(SaveCommandProperty); }
            set { SetValue(SaveCommandProperty, value); }
        }
        #endregion SaveCommand

        #region Initialization
        public BooleanExpressionControl()
        {
            InitializeComponent();
        }
        #endregion Initialization

        #region Events
        private void AddExpression(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)sender;
            BooleanOperationExpression exp = (BooleanOperationExpression)fe.DataContext;

            BooleanExpression newExp = null;
            String tag = (fe.Tag ?? "").ToString();
            switch (tag)
            {
                case "OR":
                    newExp = new BooleanOrExpressionWPF();
                    break;
                case "AND":
                    newExp = new BooleanAndExpressionWPF();
                    break;
                case "VAR_BOOL":
                    newExp = new BooleanBooleanVariableExpression();
                    break;
                case "VAR_STR":
                    newExp = new BooleanStringVariableExpression();
                    break;
            }

            exp.Children.Add(newExp);
        }

        private void RemoveRootExpression(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)sender;
            BooleanOperationExpression exp = (BooleanOperationExpression)fe.DataContext;
            exp.Children.Clear();
        }

        private void RemoveExpression(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)sender;
            BooleanExpression exp = (BooleanExpression)fe.DataContext;
            exp.Parent.Children.Remove(exp);
        }

        private void ExpressionContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)sender;
            BooleanExpression exp = (BooleanExpression)fe.DataContext;
            exp.IsSelected = true;
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
        #endregion Events

        #region Methods
        private Boolean CheckIfValidDrop(TreeViewItemDropEventArgs tviea)
        {
            BooleanExpression targetExp = (BooleanExpression)tviea.TargetItem.DataContext;
            BooleanExpression droppedExp = (BooleanExpression)tviea.DroppedItem.DataContext;
            Boolean valid = true;

            if (targetExp == droppedExp)    //Don't drop to self
            {
                valid = false;
            }

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
