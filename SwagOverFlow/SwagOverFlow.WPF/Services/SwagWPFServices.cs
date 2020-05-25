using Microsoft.Extensions.DependencyInjection;
using SwagOverFlow.Data.Persistence;
using SwagOverFlow.Utils;
using SwagOverFlow.WPF.ViewModels;
using System;

namespace SwagOverFlow.WPF.Services
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
            //This is some tight coupling here. WPF assembly depends on JsonHelper utility static class to enable 
            //BooleanExpression abstract hierarchy conversion between native and wpf classes
            //SwagContext uses JsonHelper to dynamically convert SwagItem values
            JsonHelper.Converters.Add(new BooleanExpressionWPFJsonConverter());
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
