using SwagOverFlow.ViewModels;

namespace SwagOverFlow.WPF.Interface
{
    public interface ISwagSettingGroupRepository : IRepository<SwagSettingGroup>
    {
        void RecursiveLoadChildren(SwagSetting swagSettingViewModel);
    }
}
