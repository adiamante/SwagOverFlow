using System;

namespace SwagOverflowWPF.Interface
{
    public interface ISwagDataTableUnitOfWork : IDisposable
    {
        ISwagDataTableRepository DataTables { get; }
        ISwagDataRowRepository DataRows { get; }
    }
}
