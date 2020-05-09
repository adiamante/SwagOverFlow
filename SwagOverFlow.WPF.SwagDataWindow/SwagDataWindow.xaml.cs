using SwagOverflow.WPF.Controls;
using SwagOverflow.WPF.ViewModels;
using SwagOverFlow.ViewModels;
using System.Windows;

namespace SwagOverflow.WPF.SwagDataWindow
{
    /// <summary>
    /// Interaction logic for SwagDataWindow.xaml
    /// </summary>
    public partial class SwagDataWindow : SwagWindow
    {
        #region SwagDataSet
        private static readonly DependencyProperty SwagDataSetProperty =
            DependencyProperty.Register("SwagDataSet", typeof(SwagDataSet), typeof(SwagDataWindow));

        public SwagDataSet SwagDataSet
        {
            get { return (SwagDataSet)GetValue(SwagDataSetProperty); }
            set { SetValue(SwagDataSetProperty, value); }
        }
        #endregion SwagDataSet

        public SwagDataWindow()
        {
            InitializeComponent();

            SwagDataSet = new SwagDataSet();
        }
    }
}
