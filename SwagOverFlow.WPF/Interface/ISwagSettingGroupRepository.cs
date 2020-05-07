using SwagOverflow.ViewModels;

namespace SwagOverflow.WPF.Interface
{
    public interface ISwagSettingGroupRepository : IRepository<SwagSettingGroup>
    {
        void RecursiveLoadChildren(SwagSetting swagSettingViewModel);
    }
}
