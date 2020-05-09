﻿using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.Data;
using SwagOverFlow.WPF.Interface;

namespace SwagOverFlow.WPF.Repository
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
