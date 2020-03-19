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
    }
}
