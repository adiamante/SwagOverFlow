using SwagOverflowWPF.Controls;
using SwagOverflowWPF.Data;
using SwagOverflowWPF.Repository;
using SwagOverflowWPF.ViewModels;
using System;
using System.Linq;
using System.Reflection;

namespace TestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SwagWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            String groupName = $"{Assembly.GetEntryAssembly().GetName().Name}";


            SwagWindowSettings settings = null;
            using (var work = new SwagSettingUnitOfWork(new SwagContext()))
            {
                SwagSettingGroupViewModel settingGroups = work.SettingGroups.Get(sg => sg.Name == groupName).FirstOrDefault();

                if (settingGroups == null)  //Create settingGroup
                {
                    settings = new SwagWindowSettings();
                    settings.Name = settings.AlternateId = groupName;
                    work.SettingGroups.Insert(settings);
                    work.Complete();
                }
                else
                {
                    settings = new SwagWindowSettings(settingGroups);
                }
            }

            this.Settings = settings;
        }
    }
}
