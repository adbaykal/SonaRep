// See https://aka.ms/new-console-template for more information

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SonaRep.Commands;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        return await new HostBuilder()
            .ConfigureLogging((context, builder) =>
            {
                builder.AddConsole();
            })
            .ConfigureServices((context, services) =>
            {
                services
                    .AddSingleton<IConsole>(PhysicalConsole.Singleton);
            })
            .RunCommandLineApplicationAsync<SonarepCmd>(args);
    }
}