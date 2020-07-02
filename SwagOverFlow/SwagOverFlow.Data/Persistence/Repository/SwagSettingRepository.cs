using System;
using SwagOverFlow.ViewModels;

namespace SwagOverFlow.Data.Persistence
{
    public class SwagSettingRepository : SwagEFRepository<SwagContext, SwagSetting>, ISwagSettingRepository
    {
        //Custom query method implementation here
        public SwagSettingRepository(SwagContext context) : base(context)
        {

        }

        public void RecursiveLoadChildren(SwagSetting swagSetting)
        {
            throw new NotImplementedException();
        }
    }
}
