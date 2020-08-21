using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SwagOverFlow.WPF.Controls
{
    /// <summary>
    /// Interaction logic for IconControl.xaml
    /// </summary>
    public partial class IconControl : UserControl
    {
        #region Kind
        private static readonly DependencyProperty KindProperty =
        DependencyProperty.Register("Kind", typeof(Enum), typeof(IconControl));

        public Enum Kind
        {
            get { return (Enum)GetValue(KindProperty); }
            set { SetValue(KindProperty, value); }
        }
        #endregion Kind

        #region Kind2
        private static readonly DependencyProperty Kind2Property =
        DependencyProperty.Register("Kind2", typeof(Enum), typeof(IconControl));

        public Enum Kind2
        {
            get { return (Enum)GetValue(Kind2Property); }
            set { SetValue(Kind2Property, value); }
        }
        #endregion Kind2

        #region Text
        public static DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(String),
                typeof(IconControl));

        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        #endregion Text

        #region TextMaxWidth
        public static DependencyProperty TextMaxWidthProperty =
            DependencyProperty.Register(
                "TextMaxWidth",
                typeof(Double),
                typeof(IconControl),
                new PropertyMetadata(Double.MaxValue));

        public Double TextMaxWidth
        {
            get { return (Double)GetValue(TextMaxWidthProperty); }
            set { SetValue(TextMaxWidthProperty, value); }
        }
        #endregion TextMaxWidth

        #region TextWrapping
        public static DependencyProperty TextWrappingProperty =
            DependencyProperty.Register(
                "TextWrapping",
                typeof(TextWrapping),
                typeof(IconControl));

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }
        #endregion TextWrapping

        #region TextDock
        public static DependencyProperty TextDockProperty =
            DependencyProperty.Register(
                "TextDock",
                typeof(Dock),
                typeof(IconControl),
                new PropertyMetadata(Dock.Right));

        public Dock TextDock
        {
            get { return (Dock)GetValue(TextDockProperty); }
            set { SetValue(TextDockProperty, value); }
        }
        #endregion TextDock

        #region TextMargin
        public static DependencyProperty TextMarginProperty =
            DependencyProperty.Register(
                "TextMargin",
                typeof(Thickness),
                typeof(IconControl),
                new PropertyMetadata(new Thickness(3,0,0,0)));

        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }
        #endregion TextMargin

        #region IconWidth
        public static DependencyProperty IconWidthProperty =
            DependencyProperty.Register(
                "IconWidth",
                typeof(Double),
                typeof(IconControl),
                new PropertyMetadata(20.00));

        public Double IconWidth
        {
            get { return (Double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }
        #endregion IconWidth

        #region IconHeight
        public static DependencyProperty IconHeightProperty =
            DependencyProperty.Register(
                "IconHeight",
                typeof(Double),
                typeof(IconControl),
                new PropertyMetadata(20.00));

        public Double IconHeight
        {
            get { return (Double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }
        #endregion IconHeight

        #region ShowText
        public static DependencyProperty ShowTextProperty =
            DependencyProperty.Register(
                "ShowText",
                typeof(Boolean),
                typeof(IconControl),
                new PropertyMetadata(true));

        public Boolean ShowText
        {
            get { return (Boolean)GetValue(ShowTextProperty) && !String.IsNullOrEmpty(Text); }
            set { SetValue(ShowTextProperty, value); }
        }
        #endregion ShowText

        #region ShowIcon
        public static DependencyProperty ShowIconProperty =
            DependencyProperty.Register(
                "ShowIcon",
                typeof(Boolean),
                typeof(IconControl),
                new PropertyMetadata(true));

        public Boolean ShowIcon
        {
            get { return (Boolean)GetValue(ShowIconProperty) && Kind != null; }
            set { SetValue(ShowIconProperty, value); }
        }
        #endregion ShowIcon

        #region SecondaryIconBackground
        public static DependencyProperty SecondaryIconBackgroundProperty =
            DependencyProperty.Register(
                "SecondaryIconBackground",
                typeof(SolidColorBrush),
                typeof(IconControl));

        public SolidColorBrush SecondaryIconBackground
        {
            get { return (SolidColorBrush)GetValue(SecondaryIconBackgroundProperty); }
            set { SetValue(SecondaryIconBackgroundProperty, value); }
        }
        #endregion SecondaryIconBackground

        public IconControl()
        {
            InitializeComponent();
        }
    }
}
