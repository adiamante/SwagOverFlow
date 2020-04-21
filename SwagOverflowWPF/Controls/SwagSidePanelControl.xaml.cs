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
            DependencyProperty.Register("TabItemTemplates", typeof(SwagTemplateCollection), typeof(SwagSidePanelControl),
            new FrameworkPropertyMetadata(new SwagTemplateCollection(), FrameworkPropertyMetadataOptions.Inherits));

        public SwagTemplateCollection TabItemTemplates
        {
            get { return (SwagTemplateCollection)GetValue(TabItemTemplatesProperty); }
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

            if (IsVisible && sender is TabControl)
            {
                TabControl tabControl = (TabControl)sender;

                if (tabControl.DataContext is SwagTabCollection)
                {
                    SwagTabCollection swagTabCollection = tabControl.DataContext as SwagTabCollection;
                    swagTabCollection.IsInitialized = true;
                }
            }
        }
    }
}
