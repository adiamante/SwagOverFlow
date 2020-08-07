using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.UI;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SwagOverFlow.WPF.Controls
{

    /// <summary>
    /// Interaction logic for SwagSidePanel.xaml
    /// </summary>
    public partial class SwagSidePanelControl : SwagControlBase
    {
        #region SwagTabCollection
        public static DependencyProperty SwagTabCollectionProperty =
                DependencyProperty.Register(
                    "SwagTabCollection",
                    typeof(SwagTabGroup),
                    typeof(SwagSidePanelControl),
                    new UIPropertyMetadata(null, SwagTabCollectionPropertyChanged));

        private static void SwagTabCollectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwagSidePanelControl swagSidePanelControl = (SwagSidePanelControl)d;
            if (swagSidePanelControl.IsVisible && e.NewValue != null)
            {
                swagSidePanelControl.SwagTabCollection.IsInitialized = true;
            }
        }

        public SwagTabGroup SwagTabCollection
        {
            get { return (SwagTabGroup)GetValue(SwagTabCollectionProperty); }
            set
            {
                SetValue(SwagTabCollectionProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion SwagTabCollection

        #region TabItemTemplates
        public static readonly DependencyProperty TabItemTemplatesProperty =
            DependencyProperty.Register("TabItemTemplates", typeof(SwagTemplateCollection), typeof(SwagSidePanelControl),
            new FrameworkPropertyMetadata(new SwagTemplateCollection(), FrameworkPropertyMetadataOptions.Inherits));

        public SwagTemplateCollection TabItemTemplates
        {
            get { return (SwagTemplateCollection)GetValue(TabItemTemplatesProperty); }
            set { SetValue(TabItemTemplatesProperty, value); }
        }
        #endregion TabItemTemplates

        #region BottomContentTemplate
        public static readonly DependencyProperty BottomContentTemplateProperty =
            DependencyProperty.Register("BottomContentTemplate", typeof(DataTemplate), typeof(SwagSidePanelControl));

        public DataTemplate BottomContentTemplate
        {
            get { return (DataTemplate)GetValue(BottomContentTemplateProperty); }
            set { SetValue(BottomContentTemplateProperty, value); }
        }
        #endregion BottomContentTemplate

        public SwagSidePanelControl()
        {
            InitializeComponent();
        }

        private void ControlInstance_Loaded(object sender, RoutedEventArgs e)
        {
            #region Prevents Designer Error
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }
            #endregion Prevents Designer Error

            if (IsVisible && sender is TabControl)
            {
                TabControl tabControl = (TabControl)sender;

                if (tabControl.DataContext is SwagTabGroup)
                {
                    SwagTabGroup swagTabCollection = tabControl.DataContext as SwagTabGroup;
                    swagTabCollection.IsInitialized = true;
                }
            }
        }
    }
}
