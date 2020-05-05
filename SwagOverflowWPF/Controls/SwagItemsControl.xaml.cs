using SwagOverflowWPF.UI;
using SwagOverflowWPF.ViewModels;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for SwagItemsControl.xaml
    /// </summary>
    public partial class SwagItemsControl : SwagControlBase
    {
        #region SwagItemsSource
        private static readonly DependencyProperty SwagItemsSourceProperty =
        DependencyProperty.Register("SwagItemsSource", typeof(SwagItemBase), typeof(SwagItemsControl));

        public SwagItemBase SwagItemsSource
        {
            get { return (SwagItemBase)GetValue(SwagItemsSourceProperty); }
            set
            { 
                SetValue(SwagItemsSourceProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion SwagItemsSource

        #region CustomDefaultItemTemplate
        public static readonly DependencyProperty CustomDefaultItemTemplateProperty =
            DependencyProperty.Register("CustomDefaultItemTemplate", typeof(SwagTemplate), typeof(SwagItemsControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public SwagTemplate CustomDefaultItemTemplate
        {
            get { return (SwagTemplate)GetValue(CustomDefaultItemTemplateProperty); }
            set { SetValue(CustomDefaultItemTemplateProperty, value); }
        }
        #endregion CustomDefaultItemTemplate

        #region ItemTemplates
        public static readonly DependencyProperty ItemTemplatesProperty =
            DependencyProperty.Register("ItemTemplates", typeof(SwagTemplateCollection), typeof(SwagItemsControl),
            new FrameworkPropertyMetadata(new SwagTemplateCollection(), FrameworkPropertyMetadataOptions.Inherits));

        public SwagTemplateCollection ItemTemplates
        {
            get { return (SwagTemplateCollection)GetValue(ItemTemplatesProperty); }
            set { SetValue(ItemTemplatesProperty, value); }
        }
        #endregion ItemTemplates

        #region CustomDefaultItemContainerStyle
        public static readonly DependencyProperty CustomDefaultItemContainerStyleProperty =
            DependencyProperty.Register("CustomDefaultItemContainerStyle", typeof(SwagStyle), typeof(SwagItemsControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public SwagStyle CustomDefaultItemContainerStyle
        {
            get { return (SwagStyle)GetValue(CustomDefaultItemContainerStyleProperty); }
            set { SetValue(CustomDefaultItemContainerStyleProperty, value); }
        }
        #endregion CustomDefaultItemContainerStyle

        #region ItemContainerStyles
        public static readonly DependencyProperty ItemContainerStylesProperty =
            DependencyProperty.Register("ItemContainerStyles", typeof(SwagStyleCollection), typeof(SwagItemsControl),
            new FrameworkPropertyMetadata(new SwagStyleCollection(), FrameworkPropertyMetadataOptions.Inherits));

        public SwagStyleCollection ItemContainerStyles
        {
            get { return (SwagStyleCollection)GetValue(ItemContainerStylesProperty); }
            set { SetValue(ItemContainerStylesProperty, value); }
        }
        #endregion ItemContainerStyles

        public SwagItemsControl()
        {
            InitializeComponent();
        }
    }
}
