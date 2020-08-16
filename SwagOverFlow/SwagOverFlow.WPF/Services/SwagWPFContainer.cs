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

        public static SwagDataService SwagDataService
        {
            get { return _serviceProvider.GetService<SwagDataService>(); }
        }

        public static IServiceProvider ServiceProvider
        {
            get { return _serviceProvider; }
        }

        static SwagWPFContainer()
        {
            ConfigureServices();
            //WPF assembly depends on JsonHelper utility static class to enable 
            //abstract hierarchy conversion between native and wpf classes (Ex. BooleanExpression)
            //SwagContext uses JsonHelper to dynamically convert SwagItem values
            JsonHelper.ResolveServices(_serviceProvider);
            IconHelper.ResolveServices(_serviceProvider);
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();

            //services.AddDbContext<SwagWPFContext>(options => SwagContext.SetSqliteOptions(options));
            services.AddDbContext<SwagContext, SwagWPFContext>(options => SwagContext.SetSqliteOptions(options));
            //services.AddDbContext<SwagContext>(options => SwagContext.SetSqlServerOptions(options));
            services.AddTransient<SwagWindowSettingsGroupRepository>();
            services.AddTransient<SwagSettingGroupRepository>();
            services.AddSingleton<SwagWindowSettingService>();
            services.AddTransient<ISwagDataRepository, SwagDataRepository>();
            services.AddTransient<SwagDataService>();
            services.AddSingleton<IJsonConverterProviderService, JsonConverterProviderServiceWPF>();
            services.AddSingleton<IJObjectToIconEnumService, JObjectToIconEnumServiceWPF>();
            _serviceProvider = services.BuildServiceProvider();
        }
    }
}
