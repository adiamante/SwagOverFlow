using Dreamporter.Core;
using MahApps.Metro.IconPacks;
using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.Controls;
using SwagOverFlow.WPF.UI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

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
            get { return SwagWindow.GlobalSettings["Test"]["Instruction"].GetValue<GroupInstruction>(); }
        }
        #endregion Instruction

        #region Schemas
        public List<Schema> Schemas
        {
            get { return (List<Schema>)SwagWindow.GlobalSettings["Test"]["Schemas"].GetValue<List<Schema>>(); }
        }
        #endregion Schemas

        #region Options
        public SwagOptionGroup Options
        {
            get { return (SwagOptionGroup)SwagWindow.GlobalSettings["Test"]["Options"].GetValue<SwagOptionGroup>(); }
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

            SwagWindow.GlobalSettings.TryAddChildSetting("Test", new SwagSettingGroup() { Icon = PackIconCustomKind.ClipboardTest });
            GroupInstruction grp = new GroupInstruction() { Display = "Root" };
            SwagSetting<GroupInstruction> ssGrp = new SwagSetting<GroupInstruction>()
            {
                Icon = PackIconRPGAwesomeKind.LightningBolt,
                Value = grp
            };
            SwagWindow.GlobalSettings["Test"].TryAddChildSetting("Instruction", ssGrp);
            SwagSetting<List<Schema>> ssSchemas = new SwagSetting<List<Schema>>()
            {
                Icon = PackIconCustomKind.Dataset,
                Value = new List<Schema>()
            };
            SwagWindow.GlobalSettings["Test"].TryAddChildSetting("Schemas", ssSchemas);
            SwagSetting<SwagOptionGroup> ssOpt = new SwagSetting<SwagOptionGroup>()
            {
                Icon = PackIconCustomKind.Variable,
                Value = new SwagOptionGroup()
            };
            SwagWindow.GlobalSettings["Test"].TryAddChildSetting("Options", ssOpt);

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
