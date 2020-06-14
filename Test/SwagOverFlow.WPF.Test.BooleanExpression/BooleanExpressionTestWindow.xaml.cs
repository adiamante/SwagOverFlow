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
        public BooleanOrExpressionWPF Expression
        {
            get 
            {
                if (!((SwagSettingGroup)SwagWindow.GlobalSettings["Test"]).ContainsKey("Expression"))
                {
                    BooleanOrExpressionWPF exp = new BooleanOrExpressionWPF() { Display = "Root" };
                    SwagSetting<BooleanOrExpressionWPF> ssExp = new SwagSetting<BooleanOrExpressionWPF>()
                    {
                        Icon = PackIconMaterialKind.IframeVariable,
                        Value = exp
                    };

                    ssExp.IconString = ssExp.IconString;
                    ssExp.IconTypeString = ssExp.IconTypeString;
                    ssExp.ValueTypeString = ssExp.ValueTypeString;
                    ssExp.ObjValue = ssExp.ObjValue;
                    SwagWindow.GlobalSettings["Test"]["Expression"] = ssExp;
                    ((SwagWindowSettingGroup)SwagWindow.GlobalSettings).Save();
                }

                return SwagWindow.GlobalSettings["Test"]["Expression"].GetValue<BooleanOrExpressionWPF>(); 
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

                    //SwagSetting<SwagItemGroupWPF<KeyValuePairViewModel<String, String>>> ssContext =
                    //    new SwagSetting<SwagItemGroupWPF<KeyValuePairViewModel<String, String>>>()
                    //    {
                    //        Icon = PackIconCustomKind.Variable,
                    //        Value = new SwagItemGroupWPF<KeyValuePairViewModel<String, String>>()
                    //    };

                    //ssContext.IconString = ssContext.IconString;
                    //ssContext.IconTypeString = ssContext.IconTypeString;
                    //ssContext.ValueTypeString = ssContext.ValueTypeString;
                    //ssContext.ObjValue = ssContext.ObjValue;
                    //SwagWindow.GlobalSettings["Test"]["Context"] = ssContext;
                    ((SwagWindowSettingGroup)SwagWindow.GlobalSettings).Save();
                }

                return (SwagOptionGroupWPF)SwagWindow.GlobalSettings["Test"]["Options"].GetValue<SwagOptionGroup>();
            }
        }

        #endregion Option

        #region Context
        public SwagValueItemGroupWPF<KeyValuePairViewModel<String, String>> Context
        {
            get { return SwagWindow.GlobalSettings["Test"]["Context"].GetValue<SwagValueItemGroupWPF<KeyValuePairViewModel<String, String>>>(); }
        }
        #endregion Context

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

            #region Options
            
            #endregion Options
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
        #endregion Events

        private void BooleanExpressionControl_Save(object sender, RoutedEventArgs e)
        {
            SwagWindow.GlobalSettings["Test"]["Expression"].SetValue(Expression);
        }

        private void SwagOptionControl_Save(object sender, RoutedEventArgs e)
        {
            SwagWindow.GlobalSettings["Test"]["Options"].SetValue(Options);
        }
    }
}
