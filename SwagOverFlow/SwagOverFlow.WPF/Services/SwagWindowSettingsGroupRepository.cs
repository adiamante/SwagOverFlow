using SwagOverFlow.Data.Persistence;
using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverFlow.WPF.Services
{
    public class SwagWindowSettingsGroupRepository : SwagEFRepository<SwagContext, SwagWindowSettingGroup>
    {
        public SwagWindowSettingsGroupRepository(SwagContext context) : base(context)
        {

        }
    }
}
