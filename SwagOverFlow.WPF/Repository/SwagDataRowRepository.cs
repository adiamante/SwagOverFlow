using SwagOverflow.WPF.Data;
using SwagOverflow.WPF.Interface;
using SwagOverflow.WPF.ViewModels;

namespace SwagOverflow.WPF.Repository
{
    public class SwagDataRowRepository : SwagEFRepository<SwagDataRow>, ISwagDataRowRepository
    {
        public SwagDataRowRepository(SwagContext context) : base(context)
        {
        }
    }
}
