using MahApps.Metro.IconPacks;
using SwagOverflowWPF.UI;
using SwagOverflowWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SwagOverflowWPF.Controls
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
                    typeof(SwagTabCollection),
                    typeof(SwagSidePanelControl));

        public SwagTabCollection SwagTabCollection
        {
            get { return (SwagTabCollection)GetValue(SwagTabCollectionProperty); }
            set
            {
                SetValue(SwagTabCollectionProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion SwagTabCollection

        #region TabItemTemplates
        public static readonly DependencyProperty TabItemTemplatesProperty =
            DependencyProperty.Register("TabItemTemplates", typeof(TemplateCollection), typeof(SwagSidePanelControl),
            new FrameworkPropertyMetadata(new TemplateCollection(), FrameworkPropertyMetadataOptions.Inherits));

        public TemplateCollection TabItemTemplates
        {
            get { return (TemplateCollection)GetValue(TabItemTemplatesProperty); }
            set { SetValue(TabItemTemplatesProperty, value); }
        }
        #endregion TabItemTemplates

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

            SwagTabCollection swagTabCollection = (sender as TabControl).DataContext as SwagTabCollection;
            swagTabCollection.IsInitialized = true;
        }
    }
}
