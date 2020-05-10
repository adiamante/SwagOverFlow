using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverFlow.WPF.Interface
{
    public interface ISwagSettingUnitOfWork : IDisposable
    {
        ISwagSettingGroupRepository SettingGroups { get; }
        ISwagSettingRepository Settings { get; }
    }
}
