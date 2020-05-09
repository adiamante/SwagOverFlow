using SwagOverFlow.ViewModels;

namespace SwagOverFlow.WPF.Interface
{
    public interface ISwagSettingRepository : IRepository<SwagSetting>
    {
        void RecursiveLoadChildren(SwagSetting swagSettingViewModel);
    }
}
