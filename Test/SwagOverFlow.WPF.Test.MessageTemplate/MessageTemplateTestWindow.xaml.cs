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
            get {return SwagWindow.GlobalSettings["Test"]["MessageTemplate"].GetValue<String>(); }
            set { SwagWindow.GlobalSettings["Test"]["MessageTemplate"].SetValue(value); }
        }
        #endregion MessageTemplate

        #region Options
        public SwagOptionGroup Options
        {
            get { return (SwagOptionGroup)SwagWindow.GlobalSettings["Test"]["Options"].GetValue<SwagOptionGroup>(); }
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

            SwagWindow.GlobalSettings.TryAddChildSetting("Test", new SwagSettingGroup() { Icon = PackIconCustomKind.ClipboardTest });
            SwagWindow.GlobalSettings["Test"].TryAddChildSetting("MessageTemplate", new SwagSettingString() { Icon = PackIconCustomKind.Template });
            SwagWindow.GlobalSettings["Test"].TryAddChildSetting("Options", new SwagSetting<SwagOptionGroup>() { Icon = PackIconCustomKind.Variable, Value = new SwagOptionGroup() });
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
