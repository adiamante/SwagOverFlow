using SwagOverFlow.ViewModels;

namespace SwagOverFlow.Data.Persistence
{
    public interface ISwagSettingRepository : IRepository<SwagSetting>
    {
        void RecursiveLoadChildren(SwagSetting swagSettingViewModel);
    }
}
