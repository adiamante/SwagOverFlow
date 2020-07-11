using Dreamporter.Core;
using Dreamporter.WPF.Services;
using MahApps.Metro.IconPacks;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.Controls;
using SwagOverFlow.WPF.Services;
using SwagOverFlow.WPF.UI;
using SwagOverFlow.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dreamporter.WPF.Test.Instruction
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class InstructionTestWindow : SwagWindow
    {
        #region Instruction
        public GroupInstruction Instruction
        {
            get
            {
                if (!((SwagOverFlow.ViewModels.SwagSettingGroup)SwagWindow.GlobalSettings["Test"]).ContainsKey("Instruction"))
                {
                    GroupInstruction grp = new GroupInstruction() { Display = "Root" };

                    SwagSetting<GroupInstruction> ssGrp = new SwagSetting<GroupInstruction>()
                    {
                        Icon = PackIconRPGAwesomeKind.LightningBolt,
                        Value = grp
                    };

                    ssGrp.IconString = ssGrp.IconString;
                    ssGrp.IconTypeString = ssGrp.IconTypeString;
                    ssGrp.ValueTypeString = ssGrp.ValueTypeString;
                    ssGrp.ObjValue = ssGrp.ObjValue;
                    SwagWindow.GlobalSettings["Test"]["Instruction"] = ssGrp;
                    //((SwagWindowSettingGroup)SwagWindow.GlobalSettings).Save();
                }

                return SwagWindow.GlobalSettings["Test"]["Instruction"].GetValue<GroupInstruction>();
            }
        }
        #endregion Instruction

        #region Schemas
        public List<Schema> Schemas
        {
            get
            {
                if (!((SwagOverFlow.ViewModels.SwagSettingGroup)SwagWindow.GlobalSettings["Test"]).ContainsKey("Schemas"))
                {
                    SwagSetting<List<Schema>> ssSchemas = new SwagSetting<List<Schema>>()
                    {
                        Icon = PackIconCustomKind.Dataset,
                        Value = new List<Schema>()
                    };

                    ssSchemas.IconString = ssSchemas.IconString;
                    ssSchemas.IconTypeString = ssSchemas.IconTypeString;
                    ssSchemas.ValueTypeString = ssSchemas.ValueTypeString;
                    ssSchemas.ObjValue = ssSchemas.ObjValue;
                    SwagWindow.GlobalSettings["Test"]["Schemas"] = ssSchemas;
                    //((SwagWindowSettingGroup)SwagWindow.GlobalSettings).Save();
                }

                return (List<Schema>)SwagWindow.GlobalSettings["Test"]["Schemas"].GetValue<List<Schema>>();
            }
        }
        #endregion Schemas

        #region Options
        public SwagOptionGroup Options
        {
            get
            {
                if (!((SwagOverFlow.ViewModels.SwagSettingGroup)SwagWindow.GlobalSettings["Test"]).ContainsKey("Options"))
                {
                    SwagSetting<SwagOptionGroup> ssOpt = new SwagSetting<SwagOptionGroup>()
                    {
                        Icon = PackIconCustomKind.Variable,
                        Value = new SwagOptionGroup()
                    };

                    ssOpt.IconString = ssOpt.IconString;
                    ssOpt.IconTypeString = ssOpt.IconTypeString;
                    ssOpt.ValueTypeString = ssOpt.ValueTypeString;
                    ssOpt.ObjValue = ssOpt.ObjValue;
                    SwagWindow.GlobalSettings["Test"]["Options"] = ssOpt;
                    //((SwagWindowSettingGroup)SwagWindow.GlobalSettings).Save();
                }

                return (SwagOptionGroup)SwagWindow.GlobalSettings["Test"]["Options"].GetValue<SwagOptionGroup>();
            }
        }
        #endregion Option

        #region Initialization
        public InstructionTestWindow()
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
            if (!SwagWindow.GlobalSettings.ContainsKey("Test"))
            {
                SwagSettingGroup swagDataSetting = new SwagSettingGroup() { Icon = PackIconCustomKind.ClipboardTest };
                SwagWindow.GlobalSettings["Test"] = swagDataSetting;
                swagDataSetting.IconString = swagDataSetting.IconString;
                swagDataSetting.IconTypeString = swagDataSetting.IconTypeString;
                //((SwagWindowSettingGroup)SwagWindow.GlobalSettings).Save();
            }
            #endregion Test

        }
        #endregion Initialization

        private void InstructionControl_Save(object sender, RoutedEventArgs e)
        {
            SwagWindow.GlobalSettings["Test"]["Instruction"].SetValue(Instruction);
        }

        private void SwagOptionControl_Save(object sender, RoutedEventArgs e)
        {
            SwagWindow.GlobalSettings["Test"]["Options"].SetValue(Options);
            SwagWindow.GlobalSettings["Test"]["Schemas"].SetValue(Schemas);
        }

        private void Run(object sender, RoutedEventArgs e)
        {

        }
    }
}
