// See https://aka.ms/new-console-template for more information

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using SonaRep.Commands;
using SonaRep.Helper;
using SonaRep.Models;
using SonaRep.Services;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        return await new HostBuilder()
            .ConfigureLogging((context, builder) =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog("nlog.config");
            })
            .ConfigureServices((context, services) =>
            {
                services.AddHttpClient();
                services
                    .AddSingleton<IConsole>(PhysicalConsole.Singleton);
                services
                    .AddSingleton<ISonarService,SonarService>();
                services
                    .AddSingleton<ICsvHelper,CsvHelper>();
                services
                    .AddSingleton<IReportExportService,ReportExportService>();
                services
                    .AddSingleton<UserProfileModel>();
            })
            .RunCommandLineApplicationAsync<SonarepCmd>(args);
    }
}