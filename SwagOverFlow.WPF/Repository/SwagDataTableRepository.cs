using SwagOverFlow.WPF.Data;
using SwagOverFlow.WPF.Interface;
using SwagOverFlow.ViewModels;

namespace SwagOverFlow.WPF.Repository
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
