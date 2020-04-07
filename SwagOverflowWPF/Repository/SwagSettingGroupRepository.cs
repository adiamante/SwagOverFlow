using SwagOverflowWPF.Data;
using SwagOverflowWPF.Interface;
using SwagOverflowWPF.ViewModels;

namespace SwagOverflowWPF.Repository
{
    public class SwagSettingGroupRepository : SwagEFRepository<SwagSettingGroup>, ISwagSettingGroupRepository
    {
        //Custom query method implementation here
        public SwagSettingGroupRepository(SwagContext context) : base(context)
        {
            
        }

        public void RecursiveLoadChildren(SwagSetting swagSetting)
        {
            if (swagSetting is SwagSettingGroup)
            {
                SwagSettingGroup swagSettingGroup = (SwagSettingGroup)swagSetting;
                context.Entry(swagSettingGroup).Collection(ss => ss.Children).Load();

                foreach (SwagSetting child in swagSettingGroup.Children)
                {
                    RecursiveLoadChildren(child);
                }
            }
        }
    }
}
