using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverFlow.WPF.Interface
{
    public interface ISwagSettingUnitOfWork : IDisposable
    {
        ISwagWindowSettingGroupRepository WindowSettingGroups { get; }
        ISwagSettingGroupRepository SettingGroups { get; }
        ISwagSettingRepository Settings { get; }
    }
}
