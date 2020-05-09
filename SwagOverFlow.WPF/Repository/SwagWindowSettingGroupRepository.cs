using SwagOverFlow.WPF.Data;
using SwagOverFlow.WPF.Interface;
using SwagOverFlow.WPF.ViewModels;

namespace SwagOverFlow.WPF.Repository
{
    public class SwagWindowSettingGroupRepository : SwagEFRepository<SwagWindowSettingGroup>, ISwagWindowSettingGroupRepository
    {
        //Custom query method implementation here
        public SwagWindowSettingGroupRepository(SwagContext context) : base(context)
        {

        }
    }
}
