using System;

namespace SwagOverFlow.Data.Persistence
{
    public interface ISwagDataTableUnitOfWork : IDisposable
    {
        ISwagDataTableRepository DataTables { get; }
        ISwagDataRepository Data { get; }
    }
}
