using SwagOverflowWPF.ViewModels;

namespace SwagOverflowWPF.Interface
{
    public interface ISwagDataTableRepository : IRepository<SwagDataTable>
    {
        void LoadChildren(SwagDataTable swagDataTable);
    }
}
