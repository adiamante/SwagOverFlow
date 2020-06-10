using Microsoft.Extensions.DependencyInjection;
using SwagOverFlow.Data.Persistence;
using SwagOverFlow.Services;
using SwagOverFlow.Utils;
using SwagOverFlow.WPF.ViewModels;
using System;

namespace SwagOverFlow.WPF.Services
{
    public static class SwagWPFContainer
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

        static SwagWPFContainer()
        {
            ConfigureServices();
            //This is some tight coupling here. WPF assembly depends on JsonHelper utility static class to enable 
            //BooleanExpression abstract hierarchy conversion between native and wpf classes
            //SwagContext uses JsonHelper to dynamically convert SwagItem values
            JsonHelper.JsonConverterProviderService = serviceProvider.GetService<IJsonConverterProviderService>();
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<SwagContext>(options => SwagContext.SetSqliteOptions(options));
            //services.AddDbContext<SwagContext>(options => SwagContext.SetSqlServerOptions(options));
            services.AddTransient<SwagWindowSettingService>();
            services.AddTransient<SwagDataTableService>();
            services.AddSingleton<IJsonConverterProviderService, JsonConverterProviderServiceWPF>();
            serviceProvider = services.BuildServiceProvider();
        }
    }
}
