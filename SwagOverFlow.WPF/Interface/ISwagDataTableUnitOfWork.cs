using System;

namespace SwagOverflow.WPF.Interface
{
    public interface ISwagDataTableUnitOfWork : IDisposable
    {
        ISwagDataTableRepository DataTables { get; }
        ISwagDataRowRepository DataRows { get; }
    }
}
