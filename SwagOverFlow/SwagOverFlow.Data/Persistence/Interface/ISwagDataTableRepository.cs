using SwagOverFlow.ViewModels;

namespace SwagOverFlow.Data.Persistence
{
    public interface ISwagDataTableRepository : IRepository<SwagDataTable>
    {
        void LoadChildren(SwagDataTable swagDataTable);
    }
}
