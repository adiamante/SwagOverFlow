using Microsoft.EntityFrameworkCore;
using SwagOverflowWPF.Data;
using SwagOverflowWPF.Interface;
using SwagOverflowWPF.ViewModels;
using System.Linq;

namespace SwagOverflowWPF.Repository
{
    public class SwagSettingRepository : SwagEFRepository<SwagSetting>, ISwagSettingRepository
    {
        //Custom query method implementation here
        public SwagSettingRepository(SwagContext context) : base(context)
        {

        }

        public void RecursiveLoadChildren(SwagSetting swagSettingViewModel)
        {
            context.Entry(swagSettingViewModel).Collection(ss => ((SwagSetting)ss).Children).Load();

            foreach (SwagSetting child in swagSettingViewModel.Children)
            {
                RecursiveLoadChildren(child);
            }
        }
    }
}
