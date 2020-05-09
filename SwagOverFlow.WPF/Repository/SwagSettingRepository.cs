using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.Data;
using SwagOverFlow.WPF.Interface;
using System;

namespace SwagOverFlow.WPF.Repository
{
    public class SwagSettingRepository : SwagEFRepository<SwagSetting>, ISwagSettingRepository
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
