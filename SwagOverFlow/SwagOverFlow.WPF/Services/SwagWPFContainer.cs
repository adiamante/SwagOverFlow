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
        private static IServiceProvider _serviceProvider;

        public static SwagContext Context
        {
            get { return _serviceProvider.GetService<SwagContext>(); }
        }

        public static SwagWindowSettingService SettingsService
        {
            get { return _serviceProvider.GetService<SwagWindowSettingService>(); }
        }

        public static SwagDataTableService DataTableService
        {
            get { return _serviceProvider.GetService<SwagDataTableService>(); }
        }

        public static IServiceProvider ServiceProvider
        {
            get { return _serviceProvider; }
        }

        static SwagWPFContainer()
        {
            ConfigureServices();
            //WPF assembly depends on JsonHelper utility static class to enable 
            //BooleanExpression abstract hierarchy conversion between native and wpf classes
            //SwagContext uses JsonHelper to dynamically convert SwagItem values
            JsonHelper.ResolveServices(_serviceProvider);
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<SwagContext>(options => SwagContext.SetSqliteOptions(options));
            //services.AddDbContext<SwagContext>(options => SwagContext.SetSqlServerOptions(options));
            services.AddTransient<SwagWindowSettingService>();
            services.AddTransient<SwagDataTableService>();
            services.AddSingleton<IJsonConverterProviderService, JsonConverterProviderServiceWPF>();
            _serviceProvider = services.BuildServiceProvider();
        }
    }
}
