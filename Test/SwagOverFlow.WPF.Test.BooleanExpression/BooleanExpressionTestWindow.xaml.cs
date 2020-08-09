using MahApps.Metro.IconPacks;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.Controls;
using SwagOverFlow.WPF.UI;
using SwagOverFlow.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SwagOverFlow.WPF.Test.BooleanExpression
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class BooleanExpressionTestWindow : SwagWindow
    {
        #region Expression
        public BooleanContainerExpression Expression
        {
            get { return SwagWindow.GlobalSettings["Test"]["Expression"].GetValue<BooleanContainerExpression>(); }
        }
        #endregion Expression

        #region Options
        public SwagOptionGroup Options
        {
            get { return (SwagOptionGroup)SwagWindow.GlobalSettings["Test"]["Options"].GetValue<SwagOptionGroup>(); }
        }
        #endregion Option

        #region Initialization
        public BooleanExpressionTestWindow()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            #region Prevents Designer Error
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }
            #endregion Prevents Designer Error

            SwagWindow.GlobalSettings.TryAddChildSetting("Test", new SwagSettingGroup() { Icon = PackIconCustomKind.ClipboardTest });
            BooleanContainerExpression cnt = new BooleanContainerExpression() { Display = "Root" };
            SwagSetting<BooleanContainerExpression> ssCnt = new SwagSetting<BooleanContainerExpression>()
            {
                Icon = PackIconMaterialKind.IframeVariable,
                Value = cnt
            };
            SwagWindow.GlobalSettings["Test"].TryAddChildSetting("Expression", ssCnt);
            SwagSetting<SwagOptionGroup> ssOpt = new SwagSetting<SwagOptionGroup>()
            {
                Icon = PackIconCustomKind.Variable,
                Value = new SwagOptionGroup()
            };
            SwagWindow.GlobalSettings["Test"].TryAddChildSetting("Options", ssOpt);

        }
        #endregion Initialization

        #region Events
        private void Evaluate(object sender, RoutedEventArgs e)
        {
            bool result = Expression.Evaluate(Options.Dict);
            txtResult.Text = result.ToString();

            if (result)
            {
                txtResult.Foreground = Brushes.Green;
            }
            else
            {
                txtResult.Foreground = Brushes.Red;
            }
        }

        private void BooleanExpressionControl_Save(object sender, RoutedEventArgs e)
        {
            SwagWindow.GlobalSettings["Test"]["Expression"].SetValue(Expression);
        }

        private void SwagOptionControl_Save(object sender, RoutedEventArgs e)
        {
            SwagWindow.GlobalSettings["Test"]["Options"].SetValue(Options);
        }
        #endregion Events
    }
}
