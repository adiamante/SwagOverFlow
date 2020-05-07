using SwagOverflow.WPF.Data;
using SwagOverflow.WPF.Interface;

namespace SwagOverflow.WPF.Repository
{
    public class SwagDataTableUnitOfWork : ISwagDataTableUnitOfWork
    {
        private readonly SwagContext _context;

        public ISwagDataTableRepository DataTables { get; private set; }
        public ISwagDataRowRepository DataRows { get; private set; }

        public SwagDataTableUnitOfWork(SwagContext context)
        {
            _context = context;
            DataTables = new SwagDataTableRepository(_context);
            DataRows = new SwagDataRowRepository(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
