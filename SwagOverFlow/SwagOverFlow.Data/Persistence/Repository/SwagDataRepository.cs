using SwagOverFlow.ViewModels;

namespace SwagOverFlow.Data.Persistence
{
    public class SwagDataRepository : SwagEFRepository<SwagContext, SwagData>, ISwagDataRepository
    {
        public SwagDataRepository(SwagContext context) : base(context)
        {
        }
    }
}
