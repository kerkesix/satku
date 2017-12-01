namespace KsxEventTracker.Cli
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Commands;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using static System.Console;

    class Program
    {
        // Create instance of every command handler via reflection
        // TODO: Switch back to Assembly.GetExecutingAssembly() when it is available on .NET core
        private static readonly IEnumerable<ICliCommandHandler> CommandHandlers = typeof(Program).GetTypeInfo().Assembly.GetTypes()
            .Where(
                t =>
                t.GetInterfaces().Contains(typeof(ICliCommandHandler))
                && t.GetConstructor(Type.EmptyTypes) != null)
            .Select(t => Activator.CreateInstance(t) as ICliCommandHandler);

        // TODO: Instead of exposing this, use the IOption capabilities in ASP.NET and inject settings
        public static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
            const string pathEnvKey = "KSXEVENTTRACKER_SETTINGS";
            var appSettingsPath = Environment.GetEnvironmentVariable(pathEnvKey);
            if (string.IsNullOrWhiteSpace(appSettingsPath)) 
            {
                WriteLine($"Set an environment variable {pathEnvKey} pointing to the folder where appsettings.json and other config files rest.");
                Environment.Exit(1);
            }

            WriteLine($"Using configuration files from path {appSettingsPath}");
            WriteLine($"Use {pathEnvKey} to change.");

            // Collect configuration (add sources when needed), and expose to the whole program
            var builder = new ConfigurationBuilder()
                .SetBasePath(appSettingsPath)
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();

            MainAsync(args).Wait();
            WriteLine();
        }

        static async Task MainAsync(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsage();
                return;
            }

            var commandName = args[0];
            var handler = CommandHandlers.FirstOrDefault(m => m.GetType().Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));

            if (handler == null)
            {
                PrintUsage();
                return;
            }

            // Set up logging for the domain project
            var loggerFactory = new LoggerFactory().AddConsole();
            Domain.Trace.Logger = loggerFactory.CreateLogger(commandName);

            await handler.Execute(args.Skip(1).ToArray());
        }

        static void PrintUsage()
        {
            WriteLine(
                "Usage: satku CommandName [Options..]\r\n\r\n"
                + "Commands: \r\n");

            foreach (var cli in CommandHandlers)
            {
                ForegroundColor = ConsoleColor.DarkCyan;
                Write("\t" + cli.GetType().Name + " ");

                ForegroundColor = ConsoleColor.White;
                WriteLine(cli.Usage);

                WriteLine("\t" + cli.Help + "\r\n");
            }
        }
    }
}
