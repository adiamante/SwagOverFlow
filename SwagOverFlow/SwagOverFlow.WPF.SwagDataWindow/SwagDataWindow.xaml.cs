using SwagOverFlow.WPF.Controls;
using SwagOverFlow.ViewModels;
using System.Windows;
using SwagOverFlow.WPF.UI;
using MahApps.Metro.IconPacks;
using SwagOverFlow.WPF.Services;
using System.Linq;

namespace SwagOverFlow.WPF.SwagDataWindow
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
            Icon = UIHelper.GetImageSource(PackIconOcticonsKind.Telescope, 2.0);
            //UIHelper.CreateIcon(PackIconOcticonsKind.Telescope, 2.0);
        }
    }
}
