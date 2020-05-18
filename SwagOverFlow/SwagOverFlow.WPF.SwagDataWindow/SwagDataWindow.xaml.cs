using SwagOverFlow.WPF.Controls;
using SwagOverFlow.WPF.ViewModels;
using SwagOverFlow.ViewModels;
using System.Windows;

namespace SwagOverFlow.WPF.SwagDataWindow
{
    /// <summary>
    /// Interaction logic for SwagDataWindow.xaml
    /// </summary>
    public partial class SwagDataWindow : SwagWindow
    {
        #region SwagDataSet
        private static readonly DependencyProperty SwagDataSetProperty =
            DependencyProperty.Register("SwagDataSet", typeof(SwagDataSetWPF), typeof(SwagDataWindow));

        public SwagDataSetWPF SwagDataSet
        {
            get { return (SwagDataSetWPF)GetValue(SwagDataSetProperty); }
            set { SetValue(SwagDataSetProperty, value); }
        }
        #endregion SwagDataSet

        public SwagDataWindow()
        {
            InitializeComponent();

            SwagDataSet = new SwagDataSetWPF();
        }
    }
}
