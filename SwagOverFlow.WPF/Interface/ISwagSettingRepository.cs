using SwagOverflow.WPF.Repository;
using SwagOverflow.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverflow.WPF.Interface
{
    public interface ISwagSettingRepository : IRepository<SwagSetting>
    {
        void RecursiveLoadChildren(SwagSetting swagSettingViewModel);
    }
}
