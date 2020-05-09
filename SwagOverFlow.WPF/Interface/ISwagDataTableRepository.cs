using SwagOverFlow.ViewModels;

namespace SwagOverflow.WPF.Interface
{
    public interface ISwagDataTableRepository : IRepository<SwagDataTable>
    {
        void LoadChildren(SwagDataTable swagDataTable);
    }
}
