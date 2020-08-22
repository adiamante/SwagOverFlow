using MahApps.Metro.IconPacks;
using SwagOverFlow.Parsing;
using SwagOverFlow.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace SwagOverFlow.WPF.Controls.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TestWindow : SwagWindow
    {
        #region TestTabIndex
        public Int32 TestTabIndex
        {
            get 
            { 
                return SwagWindow.GlobalSettings["Test"]["TabIndex"].GetValue<Int32>(); 
            }
            set 
            { 
                SwagWindow.GlobalSettings["Test"]["TabIndex"].SetValue(value);
                OnPropertyChanged();
            }
        }
        #endregion TestTabIndex

        #region BooleanExpression
        #region Expression
        public BooleanContainerExpression BooleanExpressionExpression
        {
            get { return SwagWindow.GlobalSettings["Test"]["BooleanExpression"]["Expression"].GetValue<BooleanContainerExpression>(); }
        }
        #endregion Expression
        #region Options
        public SwagOptionGroup BooleanExpressionOptions
        {
            get { return (SwagOptionGroup)SwagWindow.GlobalSettings["Test"]["BooleanExpression"]["Options"].GetValue<SwagOptionGroup>(); }
        }
        #endregion Options
        #endregion BooleanExpression

        #region MessageTemplate
        String _message;
        #region MessageTemplateTemplate
        public String MessageTemplateTemplate
        {
            get { return SwagWindow.GlobalSettings["Test"]["MessageTemplate"]["Template"].GetValue<String>(); }
            set { SwagWindow.GlobalSettings["Test"]["MessageTemplate"]["Template"].SetValue(value); }
        }
        #endregion MessageTemplateTemplate

        #region MessageTemplateOptions
        public SwagOptionGroup MessageTemplateOptions
        {
            get { return (SwagOptionGroup)SwagWindow.GlobalSettings["Test"]["MessageTemplate"]["Options"].GetValue<SwagOptionGroup>(); }
        }
        #endregion MessageTemplateOptions
        #region MessageTemplateMessage
        public String MessageTemplateMessage
        {
            get { return _message; }
            set { SetValue(ref _message, value); }
        }
        #endregion MessageTemplateMessage
        #endregion MessageTemplate

        #region Initialization
        public TestWindow()
        {
            Initialize();
            InitializeComponent();
        }

        public void Initialize()
        {
            #region Prevents Designer Error
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }
            #endregion Prevents Designer Error

            SwagWindow.GlobalSettings.TryAddChildSetting("Test", new SwagSettingGroup() { Icon = PackIconOcticonsKind.Beaker });
            SwagWindow.GlobalSettings["Test"].TryAddChildSetting("TabIndex", new SwagSettingInt() { Value = 0 });

            #region BooleanExpression
            SwagWindow.GlobalSettings["Test"].TryAddChildSetting("BooleanExpression", new SwagSettingGroup() { Icon = PackIconModernKind.TypeBoolean });
            BooleanContainerExpression boolContainer = new BooleanContainerExpression() { Display = "Root" };
            SwagSetting<BooleanContainerExpression> ssBoolContainer = new SwagSetting<BooleanContainerExpression>() { Icon = PackIconMaterialKind.IframeVariable, Value = boolContainer };
            SwagWindow.GlobalSettings["Test"]["BooleanExpression"].TryAddChildSetting("Expression", ssBoolContainer);
            SwagSetting<SwagOptionGroup> ssBoolExprOptions = new SwagSetting<SwagOptionGroup>() { Icon = PackIconMaterialKind.Variable, Value = new SwagOptionGroup() };
            SwagWindow.GlobalSettings["Test"]["BooleanExpression"].TryAddChildSetting("Options", ssBoolExprOptions);
            #endregion BooleanExpression

            #region MessageTemplate
            SwagWindow.GlobalSettings["Test"].TryAddChildSetting("MessageTemplate", new SwagSettingGroup() { Icon = PackIconMaterialKind.AlphaTBox });
            SwagWindow.GlobalSettings["Test"]["MessageTemplate"].TryAddChildSetting("Template", new SwagSettingString() { Icon = PackIconMaterialKind.Alphabetical });
            SwagWindow.GlobalSettings["Test"]["MessageTemplate"].TryAddChildSetting("Options", new SwagSetting<SwagOptionGroup>() { Icon = PackIconMaterialKind.Variable, Value = new SwagOptionGroup() });
            #endregion MessageTemplate
        }
        #endregion Initialization

        #region BooleanExpression Methods
        private void BooleanExpression_Evaluate(object sender, RoutedEventArgs e)
        {
            bool result = BooleanExpressionExpression.Evaluate(BooleanExpressionOptions.Dict);
            txtBooleanExpressionResult.Text = result.ToString();

            if (result)
            {
                txtBooleanExpressionResult.Foreground = Brushes.Green;
            }
            else
            {
                txtBooleanExpressionResult.Foreground = Brushes.Red;
            }
        }
        #endregion BooleanExpression Methods

        #region MessageTemplate Methods
        private void MessageTemplate_Resolve(object sender, RoutedEventArgs e)
        {
            MessageTemplateMessage = MessageTemplateHelper.ParseTemplate(MessageTemplateTemplate, MessageTemplateOptions.Dict);
        }
        #endregion MessageTemplate Methods
    }
}
