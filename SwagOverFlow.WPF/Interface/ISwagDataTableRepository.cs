using SwagOverFlow.ViewModels;

namespace SwagOverFlow.WPF.Interface
{
    public interface ISwagDataTableRepository : IRepository<SwagDataTable>
    {
        void LoadChildren(SwagDataTable swagDataTable);
    }
}
