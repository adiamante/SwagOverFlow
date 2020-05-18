

namespace SwagOverFlow.Data.Persistence
{
    public class SwagDataTableUnitOfWork : ISwagDataTableUnitOfWork
    {
        private readonly SwagContext _context;

        public ISwagDataTableRepository DataTables { get; private set; }
        public ISwagDataRepository Data { get; private set; }

        public SwagDataTableUnitOfWork(SwagContext context)
        {
            _context = context;
            DataTables = new SwagDataTableRepository(_context);
            Data = new SwagDataRepository(_context);
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
