using SwagOverflowWPF.Data;
using SwagOverflowWPF.Interface;
using SwagOverflowWPF.ViewModels;

namespace SwagOverflowWPF.Repository
{
    public class SwagDataTableRepository : SwagEFRepository<SwagDataTable>, ISwagDataTableRepository
    {
        public SwagDataTableRepository(SwagContext context) : base(context)
        {

        }
        public void RecursiveLoadChildren(SwagDataRow swagDataRow)
        {
            if (swagDataRow is SwagDataTable)
            {
                SwagDataTable swagDataTable = (SwagDataTable)swagDataRow;
                context.Entry(swagDataTable).Collection(ss => ss.Children).Load();

                foreach (SwagDataRow child in swagDataTable.Children)
                {
                    RecursiveLoadChildren(child);
                }
            }
        }
    }
}
