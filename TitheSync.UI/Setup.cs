using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvvmCross.Platforms.Wpf.Core;
using Serilog;
using Serilog.Extensions.Logging;
using System.IO;

namespace TitheSync.UI
{
    public class Setup:MvxWpfSetup<Core.App>
    {
        protected override ILoggerProvider? CreateLogProvider() => new SerilogLoggerProvider();

        protected override ILoggerFactory? CreateLogFactory()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                                               .SetBasePath(Directory.GetCurrentDirectory())
                                               .AddJsonFile("appsettings.json", false, true)
                                               .Build();

            Log.Logger = new LoggerConfiguration()
                         .ReadFrom.Configuration(configuration)
                         .CreateLogger();

            return new SerilogLoggerFactory();
        }
    }
}
