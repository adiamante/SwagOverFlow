using SwagOverflow.WPF.Data;
using SwagOverflow.WPF.Interface;
using SwagOverFlow.ViewModels;

namespace SwagOverflow.WPF.Repository
{
    public class SwagDataRepository : SwagEFRepository<SwagData>, ISwagDataRepository
    {
        public SwagDataRepository(SwagContext context) : base(context)
        {
        }
    }
}
