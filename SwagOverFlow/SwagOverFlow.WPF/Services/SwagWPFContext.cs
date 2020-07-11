using Microsoft.EntityFrameworkCore;
using SwagOverFlow.Data.Persistence;
using SwagOverFlow.WPF.ViewModels;


namespace SwagOverFlow.WPF.Services
{
    public class SwagWPFContext : SwagContext
    {
        DbSet<SwagWindowSettingGroup> SwagWindowSettingGroups { get; set; }

        public SwagWPFContext(DbContextOptions<SwagWPFContext> options)
        {
        }
    }
}
