using SwagOverFlow.ViewModels;

namespace SwagOverFlow.Data.Persistence
{
    public class SwagDataTableRepository : SwagEFRepository<SwagDataTable>, ISwagDataTableRepository
    {
        public SwagDataTableRepository(SwagContext context) : base(context)
        {

        }
        public void LoadChildren(SwagDataTable swagDataTable)
        {
            context.Entry(swagDataTable).Collection(ss => ss.Children).Load();
        }
    }
}
