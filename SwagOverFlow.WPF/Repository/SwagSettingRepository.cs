using Microsoft.EntityFrameworkCore;
using SwagOverflow.WPF.Data;
using SwagOverflow.WPF.Interface;
using SwagOverflow.WPF.ViewModels;
using System;
using System.Linq;

namespace SwagOverflow.WPF.Repository
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
