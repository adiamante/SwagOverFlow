using Microsoft.EntityFrameworkCore;
using SwagOverflowWPF.Data;
using SwagOverflowWPF.Interface;
using SwagOverflowWPF.ViewModels;
using System;
using System.Linq;

namespace SwagOverflowWPF.Repository
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
