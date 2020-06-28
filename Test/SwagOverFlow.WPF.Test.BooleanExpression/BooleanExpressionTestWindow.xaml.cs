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
        public BooleanContainerExpressionWPF Expression
        {
            get 
            {
                if (!((SwagSettingGroup)SwagWindow.GlobalSettings["Test"]).ContainsKey("Expression"))
                {
                    BooleanContainerExpressionWPF cnt = new BooleanContainerExpressionWPF() { Display = "Root" };
                    SwagSetting<BooleanContainerExpressionWPF> ssCnt = new SwagSetting<BooleanContainerExpressionWPF>()
                    {
                        Icon = PackIconMaterialKind.IframeVariable,
                        Value = cnt
                    };

                    ssCnt.IconString = ssCnt.IconString;
                    ssCnt.IconTypeString = ssCnt.IconTypeString;
                    ssCnt.ValueTypeString = ssCnt.ValueTypeString;
                    ssCnt.ObjValue = ssCnt.ObjValue;
                    SwagWindow.GlobalSettings["Test"]["Expression"] = ssCnt;
                    ((SwagWindowSettingGroup)SwagWindow.GlobalSettings).Save();
                }

                return SwagWindow.GlobalSettings["Test"]["Expression"].GetValue<BooleanContainerExpressionWPF>(); 
            }
        }
        #endregion Expression

        #region Options
        public SwagOptionGroupWPF Options
        {
            get
            {
                if (!((SwagSettingGroup)SwagWindow.GlobalSettings["Test"]).ContainsKey("Options"))
                {
                    SwagSetting<SwagOptionGroup> ssOpt = new SwagSetting<SwagOptionGroup>()
                    {
                        Icon = PackIconCustomKind.Variable,
                        Value = new SwagOptionGroupWPF()
                    };

                    ssOpt.IconString = ssOpt.IconString;
                    ssOpt.IconTypeString = ssOpt.IconTypeString;
                    ssOpt.ValueTypeString = ssOpt.ValueTypeString;
                    ssOpt.ObjValue = ssOpt.ObjValue;
                    SwagWindow.GlobalSettings["Test"]["Options"] = ssOpt;
                    ((SwagWindowSettingGroup)SwagWindow.GlobalSettings).Save();
                }

                return (SwagOptionGroupWPF)SwagWindow.GlobalSettings["Test"]["Options"].GetValue<SwagOptionGroup>();
            }
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

            #region Test
            if (!((SwagSettingGroup)SwagWindow.GlobalSettings).ContainsKey("Test"))
            {
                SwagSettingWPFGroup swagDataSetting = new SwagSettingWPFGroup() { Icon = PackIconCustomKind.ClipboardTest };
                SwagWindow.GlobalSettings["Test"] = swagDataSetting;
                swagDataSetting.IconString = swagDataSetting.IconString;
                swagDataSetting.IconTypeString = swagDataSetting.IconTypeString;
                ((SwagWindowSettingGroup)SwagWindow.GlobalSettings).Save();
            }
            #endregion Test
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
