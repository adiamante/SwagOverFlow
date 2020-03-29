using SwagOverflowWPF.Repository;
using SwagOverflowWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverflowWPF.Interface
{
    public interface ISwagSettingRepository : IRepository<SwagSetting>
    {
        void RecursiveLoadChildren(SwagSetting swagSettingViewModel);
    }
}
