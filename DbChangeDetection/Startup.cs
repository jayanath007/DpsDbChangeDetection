using DbChangeDetection.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;


[assembly: FunctionsStartup(typeof(DbChangeDetection.Startup))]

namespace DbChangeDetection
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<IDbChangesServices, DbChangesServices>();
            builder.Services.AddTransient<IMainInformationServices, MainInformationServices>();
        }
    }
}