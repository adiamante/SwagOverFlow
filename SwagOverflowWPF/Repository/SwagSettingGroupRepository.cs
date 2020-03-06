using SwagOverflowWPF.Data;
using SwagOverflowWPF.Interface;
using SwagOverflowWPF.ViewModels;

namespace SwagOverflowWPF.Repository
{
    public class SwagSettingGroupRepository : SwagEFRepository<SwagSettingGroupViewModel>, ISwagSettingGroupRepository
    {
        //Custom query method implementation here
        public SwagSettingGroupRepository(SwagContext context) : base(context)
        {

        }
    }
}
