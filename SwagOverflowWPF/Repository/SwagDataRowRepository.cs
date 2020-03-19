using SwagOverflowWPF.Data;
using SwagOverflowWPF.Interface;
using SwagOverflowWPF.ViewModels;

namespace SwagOverflowWPF.Repository
{
    public class SwagDataRowRepository : SwagEFRepository<SwagDataRow>, ISwagDataRowRepository
    {
        public SwagDataRowRepository(SwagContext context) : base(context)
        {
        }

        public void RecursiveLoadChildren(SwagDataRow row)
        {
            context.Entry(row).Collection(sdr => ((SwagDataRow)sdr).Children).Load();

            foreach (SwagDataRow child in row.Children)
            {
                RecursiveLoadChildren(child);
            }
        }
    }
}
