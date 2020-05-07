using SwagOverflow.WPF.Data;
using SwagOverflow.WPF.Interface;
using SwagOverflow.WPF.ViewModels;

namespace SwagOverflow.WPF.Repository
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
