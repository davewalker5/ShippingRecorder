
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShippingRecorder.BusinessLogic.Config;
using ShippingRecorder.BusinessLogic.Factory;
using ShippingRecorder.BusinessLogic.Logging;
using ShippingRecorder.Data;
using ShippingRecorder.Entities.Config;
using ShippingRecorder.Entities.Logging;
using ShippingRecorder.Manager.Logic;

namespace ShippingRecorder.Manager
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // Process the command line arguments
            var parser = new ManagerCommandLineParser(new HelpTabulator());
            parser.Parse(args);

            // If help's been requested, show help and exit
            if (parser.IsPresent(CommandLineOptionType.Help))
            {
                parser.Help();
            }
            else
            {
                // Read the application config file
                var settings = new ManagerSettingsBuilder().BuildSettings(parser, "appsettings.json");

                // Configure the log file
                var logger = new FileLogger();
                logger.Initialise(settings.LogFile, settings.MinimumLogLevel);

                // Get the version number and application title
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
                var title = $"Shipping Recorder Management Tool v{info.FileVersion}";

                // Show the startup messages
                Console.WriteLine(new string('=', 80));
                Console.WriteLine(title);
                Console.WriteLine($"Output will be logged to {settings.LogFile}");

                // Log the startup messages
                logger.LogMessage(Severity.Info, new string('=', 80));
                logger.LogMessage(Severity.Info, title);

                // Create the database management factory and API factory
                var context = new ShippingRecorderDbContextFactory().CreateDbContext([]);
                var factory = new ShippingRecorderFactory(context, logger);
    
                // Apply the latest database migrations
                if (parser.IsPresent(CommandLineOptionType.Update))
                {
                    context.Database.Migrate();
                    var message = "Latest database migrations have been applied";
                    Console.WriteLine(message);
                    logger.LogMessage(Severity.Info, message);
                }

                // If a CSV file containing country details has been supplied, import it
                if (parser.IsPresent(CommandLineOptionType.ImportCountries))
                {
                    await new ImportHandler(settings, parser, factory).HandleCountryImportAsync();
                }

                // If a CSV file containing operator details has been supplied, import it
                if (parser.IsPresent(CommandLineOptionType.ImportOperators))
                {
                    await new ImportHandler(settings, parser, factory).HandleOperatorImportAsync();
                }

                // If a CSV file containing port details has been supplied, import it
                if (parser.IsPresent(CommandLineOptionType.ImportPorts))
                {
                    await new ImportHandler(settings, parser, factory).HandlePortImportAsync();
                }

                // Handle user addition
                if (parser.IsPresent(CommandLineOptionType.AddUser))
                {
                    await new UserManagementHandler(settings, parser, factory).HandleAddUserAsync();
                }

                // Handle user deletion
                if (parser.IsPresent(CommandLineOptionType.DeleteUser))
                {
                    await new UserManagementHandler(settings, parser, factory).HandleDeleteUserAsync();
                }

                // Handle user password updates
                if (parser.IsPresent(CommandLineOptionType.SetPassword))
                {
                    await new UserManagementHandler(settings, parser, factory).HandleSetPasswordAsync();
                }
            }
        }
    }
}