using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverFlow.Data.Persistence
{
    public interface ISwagSettingUnitOfWork : IDisposable
    {
        ISwagSettingGroupRepository SettingGroups { get; }
        ISwagSettingRepository Settings { get; }
    }
}
