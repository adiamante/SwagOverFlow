using SwagOverflowWPF.Data;
using SwagOverflowWPF.Interface;

namespace SwagOverflowWPF.Repository
{
    public class SwagSettingUnitOfWork : ISwagSettingUnitOfWork
    {
        private readonly SwagContext _context;

        public ISwagSettingGroupRepository SettingGroups { get; private set; }
        public ISwagSettingRepository Settings { get; private set; }

        public SwagSettingUnitOfWork(SwagContext context)
        {
            _context = context;
            SettingGroups = new SwagSettingGroupRepository(_context);
            Settings = new SwagSettingRepository(_context);
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
