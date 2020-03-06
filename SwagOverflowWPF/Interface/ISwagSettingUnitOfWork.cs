using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverflowWPF.Interface
{
    public interface ISwagSettingUnitOfWork : IDisposable
    {
        ISwagSettingGroupRepository SettingGroups { get; }
        ISwagSettingRepository Settings { get; }
    }
}
