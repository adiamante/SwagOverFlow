using SwagOverflowWPF.Controls;
using SwagOverflowWPF.Data;
using SwagOverflowWPF.Interface;

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
