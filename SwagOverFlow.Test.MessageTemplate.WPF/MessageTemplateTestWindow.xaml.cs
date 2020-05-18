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
            get { return SwagWindow.GlobalSettings["Test"]["MessageTemplate"].GetValue<String>(); }
            set { SwagWindow.GlobalSettings["Test"]["MessageTemplate"].SetValue(value); }
        }
        #endregion MessageTemplate

        #region Variables
        public SwagItemGroupWPF<KeyValuePairViewModel<String, String>> Variables
        {
            get { return SwagWindow.GlobalSettings["Test"]["Variables"].GetValue<SwagItemGroupWPF<KeyValuePairViewModel<String, String>>>(); }
        }
        #endregion Variables

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

            #region MessageTemplate
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
            #endregion MessageTemplate

            #region Variables
            if (!((SwagSettingGroup)SwagWindow.GlobalSettings["Test"]).ContainsKey("Variables"))
            {
                SwagSetting<SwagItemGroupWPF<KeyValuePairViewModel<String, String>>> ssVariables =
                    new SwagSetting<SwagItemGroupWPF<KeyValuePairViewModel<String, String>>>()
                    {
                        Icon = PackIconCustomKind.Variable,
                        Value = new SwagItemGroupWPF<KeyValuePairViewModel<String, String>>()
                    };

                ssVariables.IconString = ssVariables.IconString;
                ssVariables.IconTypeString = ssVariables.IconTypeString;
                ssVariables.ValueTypeString = ssVariables.ValueTypeString;
                ssVariables.ObjValue = ssVariables.ObjValue;
                SwagWindow.GlobalSettings["Test"]["Variables"] = ssVariables;
                ((SwagWindowSettingGroup)SwagWindow.GlobalSettings).Save();
            }

            SwagWindow.GlobalSettings["Test"]["Variables"].GetValue<SwagItemGroupWPF<KeyValuePairViewModel<String, String>>>().SwagItemChanged += (s, e) =>
            {
                SwagWindow.GlobalSettings.OnSwagItemChanged(SwagWindow.GlobalSettings["Test"]["Variables"], e.PropertyChangedArgs);
            };
            #endregion Variables
        }

        private void GenerateMessage(object sender, RoutedEventArgs e)
        {
            //var messageTemplate = MessageTemplateRenderer.
            //MessageTemplateParser parser = new MessageTemplateParser();
            //Parsing.MessageTemplate messageTemplate = parser.Parse(MessageTemplate);

            Dictionary<String, String> variables = new Dictionary<string, string>();

            foreach (SwagItem<KeyValuePairViewModel<String, String>> variable in Variables.Children)
            {
                variables.Add(variable.Value.Key, variable.Value.Value);
            }

            //Message = messageTemplate.Render(variables);
            Message = MessageTemplateHelper.ParseTemplate(MessageTemplate, variables);
        }
    }
}
