using SwagOverflowWPF.Data;
using SwagOverflowWPF.Interface;
using SwagOverflowWPF.ViewModels;

namespace SwagOverflowWPF.Repository
{
    public class SwagWindowSettingGroupRepository : SwagEFRepository<SwagWindowSettingGroup>, ISwagWindowSettingGroupRepository
    {
        //Custom query method implementation here
        public SwagWindowSettingGroupRepository(SwagContext context) : base(context)
        {

        }
    }
}
