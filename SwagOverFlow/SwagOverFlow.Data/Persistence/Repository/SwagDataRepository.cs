using SwagOverFlow.ViewModels;

namespace SwagOverFlow.Data.Persistence
{
    public class SwagDataRepository : SwagEFRepository<SwagData>, ISwagDataRepository
    {
        public SwagDataRepository(SwagContext context) : base(context)
        {
        }
    }
}
