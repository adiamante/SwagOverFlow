using Microsoft.Extensions.DependencyInjection;
using SwagOverflow.WPF.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverflow.WPF.Services
{
    public static class SwagWPFServices
    {
        private static IServiceProvider serviceProvider;

        public static SwagContext Context
        {
            get { return serviceProvider.GetService<SwagContext>(); }
        }

        public static SwagWindowSettingService SettingsService
        {
            get { return serviceProvider.GetService<SwagWindowSettingService>(); }
        }

        public static SwagDataTableService DataTableService
        {
            get { return serviceProvider.GetService<SwagDataTableService>(); }
        }

        static SwagWPFServices()
        {
            ConfigureServices();
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<SwagContext>(options => SwagContext.SetSqliteOptions(options));
            //services.AddDbContext<SwagContext>(options => SwagContext.SetSqlServerOptions(options));
            services.AddTransient<SwagWindowSettingService>();
            services.AddTransient<SwagDataTableService>();
            
            serviceProvider = services.BuildServiceProvider();
        }
    }
}
