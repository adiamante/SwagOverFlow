using Microsoft.EntityFrameworkCore;
using SwagOverflowWPF.Data;
using SwagOverflowWPF.Interface;
using SwagOverflowWPF.ViewModels;
using System.Linq;

namespace SwagOverflowWPF.Repository
{
    public class SwagSettingRepository : SwagEFRepository<SwagSettingViewModel>, ISwagSettingRepository
    {
        //Custom query method implementation here
        public SwagSettingRepository(SwagContext context) : base(context)
        {

        }

        public void RecursiveLoadChildren(SwagSettingViewModel swagSettingViewModel)
        {
            context.Entry(swagSettingViewModel).Collection(ss => ((SwagSettingViewModel)ss).Children).Load();

            foreach (SwagSettingViewModel child in swagSettingViewModel.Children)
            {
                RecursiveLoadChildren(child);
            }
        }
    }
}
