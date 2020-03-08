using SwagOverflowWPF.Repository;
using SwagOverflowWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverflowWPF.Interface
{
    public interface ISwagSettingRepository : IRepository<SwagSettingViewModel>
    {
        void RecursiveLoadChildren(SwagSettingViewModel swagSettingViewModel);
    }
}
