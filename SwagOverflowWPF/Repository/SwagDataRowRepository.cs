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
    }
}
