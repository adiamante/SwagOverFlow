using SwagOverflow.ViewModels;

namespace SwagOverflow.WPF.Interface
{
    public interface ISwagSettingRepository : IRepository<SwagSetting>
    {
        void RecursiveLoadChildren(SwagSetting swagSettingViewModel);
    }
}
