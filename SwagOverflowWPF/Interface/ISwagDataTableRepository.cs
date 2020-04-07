using SwagOverflowWPF.ViewModels;

namespace SwagOverflowWPF.Interface
{
    public interface ISwagDataTableRepository : IRepository<SwagDataTable>
    {
        void RecursiveLoadChildren(SwagDataRow swagDataRow);
    }
}
