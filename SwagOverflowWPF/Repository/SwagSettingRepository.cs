using SwagOverflowWPF.Data;
using SwagOverflowWPF.Interface;
using SwagOverflowWPF.ViewModels;

namespace SwagOverflowWPF.Repository
{
    public class SwagSettingRepository : SwagEFRepository<SwagSettingViewModel>, ISwagSettingRepository
    {
        //Custom query method implementation here
        public SwagSettingRepository(SwagContext context) : base(context)
        {

        }
    }
}
