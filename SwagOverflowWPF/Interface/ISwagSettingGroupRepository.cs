using SwagOverflowWPF.ViewModels;

namespace SwagOverflowWPF.Interface
{
    public interface ISwagSettingGroupRepository : IRepository<SwagSettingGroup>
    {
        void RecursiveLoadChildren(SwagSetting swagSettingViewModel);
    }
}
