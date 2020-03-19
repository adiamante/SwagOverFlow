using SwagOverflowWPF.ViewModels;

namespace SwagOverflowWPF.Interface
{
    public interface ISwagDataRowRepository : IRepository<SwagDataRow>
    {
        void RecursiveLoadChildren(SwagDataRow swagSettingViewModel);
    }
}
