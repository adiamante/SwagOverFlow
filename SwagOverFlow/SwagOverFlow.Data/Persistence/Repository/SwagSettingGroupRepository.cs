using SwagOverFlow.ViewModels;

namespace SwagOverFlow.Data.Persistence
{
    public class SwagSettingGroupRepository : SwagEFRepository<SwagContext, SwagSettingGroup>, ISwagSettingGroupRepository
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
