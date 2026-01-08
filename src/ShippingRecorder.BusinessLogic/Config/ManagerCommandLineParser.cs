using ShippingRecorder.Entities.Config;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.BusinessLogic.Config
{
    public class ManagerCommandLineParser : CommandLineParser
    {
        public ManagerCommandLineParser(IHelpGenerator generator) : base(generator)
        {
            Add(CommandLineOptionType.Help, false, "--help", "-h", "Show command line help", 0, 0);
            Add(CommandLineOptionType.Update, false, "--update", "-u", "Apply the latest database migrations", 0, 0);
            Add(CommandLineOptionType.ImportCountries, false, "--import-countries", "-ic", "Import countries from a CSV file", 1, 1);
            Add(CommandLineOptionType.ImportOperators, false, "--import-operators", "-io", "Import operators from a CSV file", 1, 1);
            Add(CommandLineOptionType.ImportVesselTypes, false, "--import-vessel-types", "-ivt", "Import vessel types from a CSV file", 1, 1);
            Add(CommandLineOptionType.ImportVessels, false, "--import-vessels", "-iv", "Import vessels from a CSV file", 1, 1);
            Add(CommandLineOptionType.ImportSightings, false, "--import-sightings", "-is", "Import sightings from a CSV file", 1, 1);
            Add(CommandLineOptionType.ExportLocations, false, "--export-locations", "-el", "Export locations to a CSV file", 1, 1);
            Add(CommandLineOptionType.ExportVessels, false, "--export-vessels", "-ev", "Export vessels to a CSV file", 1, 1);
            Add(CommandLineOptionType.ExportSightings, false, "--export-sightings", "-es", "Export sightings to a CSV file", 1, 1);
            Add(CommandLineOptionType.AddUser, false, "--add-user", "-au", "Add a user to the database", 2, 2);
            Add(CommandLineOptionType.SetPassword, false, "--set-password", "-sp", "Set the password for an existing user", 2, 2);
            Add(CommandLineOptionType.DeleteUser, false, "--delete-user", "-du", "Delete an existing user", 1, 1);
        }
    }
}
