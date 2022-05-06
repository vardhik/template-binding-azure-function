using System;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using TemplateGenerate;
using TemplateGenerate.Views;
using TemplateGenerate.ViewModel;

[assembly: FunctionsStartup(typeof(Startup))]
namespace TemplateGenerate
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string executionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var compiledViewAssembly = Assembly.LoadFile(Path.Combine(executionPath, "TemplateGenerate.Views.dll"));

            builder.Services
                .AddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>()
                .AddScoped<RazorViewToStringRenderer>()
                .AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>()
                .AddMvcCore()
                .AddViews()
                .AddRazorViewEngine()
                .AddApplicationPart(compiledViewAssembly);
        }
    }
    public static class ViewModelMap
    {
        //Note:keep all name in lowercase
        public static Dictionary<string, Type> GetModel = new Dictionary<string, Type>()
        {
            {
             "logindetail",typeof(LoginDetailViewModel)
            },
        };
    }
}
