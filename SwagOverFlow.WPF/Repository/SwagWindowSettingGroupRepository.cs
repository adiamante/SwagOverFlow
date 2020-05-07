using SwagOverflow.WPF.Data;
using SwagOverflow.WPF.Interface;
using SwagOverflow.WPF.ViewModels;

namespace SwagOverflow.WPF.Repository
{
    public class SwagWindowSettingGroupRepository : SwagEFRepository<SwagWindowSettingGroup>, ISwagWindowSettingGroupRepository
    {
        //Custom query method implementation here
        public SwagWindowSettingGroupRepository(SwagContext context) : base(context)
        {

        }
    }
}
