using SwagOverFlow.ViewModels;

namespace SwagOverFlow.Data.Persistence
{
    public interface ISwagSettingGroupRepository : IRepository<SwagSettingGroup>
    {
        void RecursiveLoadChildren(SwagSetting swagSettingViewModel);
    }
}
