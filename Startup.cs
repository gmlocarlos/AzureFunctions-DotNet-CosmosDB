using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using  Serverless.FunctionsDemo;
using Serverless.FunctionsDemo.Repository;
using Serverless.FunctionsDemo.Contract;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Serverless.FunctionsDemo{
    public class Startup: FunctionsStartup{
        public override void Configure(IFunctionsHostBuilder builder)
        {
        var config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("local.settings.json",optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables()
                            .Build();
            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddTransient<ICreditCardRepository,CreditCardRepository>();

        }
    }
}