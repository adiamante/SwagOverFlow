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

        public IconControl()
        {
            InitializeComponent();
        }
    }
}
