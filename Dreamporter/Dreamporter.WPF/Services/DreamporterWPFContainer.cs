using Dreamporter.Data;
using Microsoft.Extensions.DependencyInjection;
using SwagOverFlow.Services;
using SwagOverFlow.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dreamporter.WPF.Services
{
    public class DreamporterWPFContainer
    {
        private static IServiceProvider _serviceProvider;

        public static DreamporterContext Context
        {
            get { return _serviceProvider.GetService<DreamporterContext>(); }
        }

        public static IIntegrationRepository IntegrationDataRepository
        {
            get { return _serviceProvider.GetService<IIntegrationRepository>(); }
        }

        public static IBuildRepository BuildRepository
        {
            get { return _serviceProvider.GetService<IBuildRepository>(); }
        }

        public static IServiceProvider ServiceProvider
        {
            get { return _serviceProvider; }
        }

        static DreamporterWPFContainer()
        {
            ConfigureServices();
            //WPF assembly depends on JsonHelper utility static class to enable 
            //abstract hierarchy conversion between native and wpf classes (Ex. BooleanExpression)
            JsonHelper.ResolveServices(_serviceProvider);
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();

            //services.AddDbContext<DreamporterContext>(options => SwagContext.SetSqliteOptions(options));
            services.AddDbContext<DreamporterContext>();
            services.AddTransient<IIntegrationRepository, IntegrationEFRepository>();
            services.AddTransient<IBuildRepository, BuildEFRepository>();
            services.AddSingleton<IJsonConverterProviderService, DreamporterJsonConverterProvider>();
            _serviceProvider = services.BuildServiceProvider();
        }
    }
}
