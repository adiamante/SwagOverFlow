using SwagOverFlow.WPF.Data;
using SwagOverFlow.WPF.Interface;
using SwagOverFlow.ViewModels;

namespace SwagOverFlow.WPF.Repository
{
    public class SwagDataRepository : SwagEFRepository<SwagData>, ISwagDataRepository
    {
        public SwagDataRepository(SwagContext context) : base(context)
        {
        }
    }
}
