using SwagOverFlow.Parsing;
using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.Controls;
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

namespace SwagOverFlow.Test.MessageTemplate.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MessageTemplateTestWindow : SwagWindow
    {
        String _message;

        #region MessageTemplate
        public String MessageTemplate
        {
            get 
            {
                if (!((SwagSettingGroup)SwagWindow.GlobalSettings["Test"]).ContainsKey("MessageTemplate"))
                {
                    SwagSettingString ssMessageTemplate = new SwagSettingString()
                    {
                        Icon = PackIconCustomKind.Template,
                        Value = ""
                    };

                    ssMessageTemplate.IconString = ssMessageTemplate.IconString;
                    ssMessageTemplate.IconTypeString = ssMessageTemplate.IconTypeString;
                    ssMessageTemplate.ValueTypeString = ssMessageTemplate.ValueTypeString;
                    ssMessageTemplate.ObjValue = ssMessageTemplate.ObjValue;

                    SwagWindow.GlobalSettings["Test"]["MessageTemplate"] = ssMessageTemplate;
                    ((SwagWindowSettingGroup)SwagWindow.GlobalSettings).Save();
                }

                return SwagWindow.GlobalSettings["Test"]["MessageTemplate"].GetValue<String>(); 
            }
            set { SwagWindow.GlobalSettings["Test"]["MessageTemplate"].SetValue(value); }
        }
        #endregion MessageTemplate

        #region Options
        public SwagOptionGroup Options
        {
            get
            {
                if (!((SwagSettingGroup)SwagWindow.GlobalSettings["Test"]).ContainsKey("Options"))
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
                    ((SwagWindowSettingGroup)SwagWindow.GlobalSettings).Save();
                }

                return (SwagOptionGroup)SwagWindow.GlobalSettings["Test"]["Options"].GetValue<SwagOptionGroup>();
            }
        }
        #endregion Option

        #region Message
        public String Message
        {
            get { return _message; }
            set { SetValue(ref _message, value); }
        }
        #endregion Message

        public MessageTemplateTestWindow()
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

        private void GenerateMessage(object sender, RoutedEventArgs e)
        {
            Message = MessageTemplateHelper.ParseTemplate(MessageTemplate, Options.Dict);
        }

        private void SwagOptionControl_Save(object sender, RoutedEventArgs e)
        {
            SwagWindow.GlobalSettings["Test"]["Options"].SetValue(Options);
        }
    }
}
