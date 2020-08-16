using Microsoft.EntityFrameworkCore;
using SwagOverFlow.Data.Persistence;
using SwagOverFlow.ViewModels;

namespace SwagOverFlow.WPF.Services
{
    public class SwagDataService
    {
        private readonly SwagContext _context;
        private readonly ISwagDataRepository _swagDataRepository;

        public DbSet<SwagDataSet> SwagDataSets
        {
            get { return _context.SwagDataSets; }
        }

        public DbSet<SwagDataTable> SwagDataTables
        {
            get { return _context.SwagDataTables; }
        }

        public DbSet<SwagDataColumn> SwagDataColumns
        {
            get { return _context.SwagDataColumns; }
        }

        public DbSet<SwagDataRow> SwagDataRows
        {
            get { return _context.SwagDataRows; }
        }

        public SwagDataService(SwagContext context, ISwagDataRepository swagDataRepository)
        {
            _context = context;
            _swagDataRepository = swagDataRepository;
        }

        public void Init(SwagData swagData)
        {
            if (swagData is SwagDataSet swagDataSet)
            {
                _swagDataRepository.RecursiveLoadCollection(swagDataSet, "Children");
            }
        }

        public void Save()
        {
            _swagDataRepository.Save();
        }
    }
}
