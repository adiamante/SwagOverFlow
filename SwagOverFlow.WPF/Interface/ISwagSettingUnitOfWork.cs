using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverflow.WPF.Interface
{
    public interface ISwagSettingUnitOfWork : IDisposable
    {
        ISwagWindowSettingGroupRepository WindowSettingGroups { get; }
        ISwagSettingGroupRepository SettingGroups { get; }
        ISwagSettingRepository Settings { get; }
    }
}
