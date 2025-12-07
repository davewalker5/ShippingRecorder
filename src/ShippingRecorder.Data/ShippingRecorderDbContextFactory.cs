using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ShippingRecorder.Data
{
    public class ShippingRecorderDbContextFactory : IDesignTimeDbContextFactory<ShippingRecorderDbContext>
    {
        /// <summary>
        /// Create a context for the real database 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public ShippingRecorderDbContext CreateDbContext(string[] args)
        {
            // Construct a configuration object that contains the key/value pairs from the settings file
            // at the root of the main applicatoin
            IConfigurationRoot configuration = new ConfigurationBuilder()
                                                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                                                    .Build();

            // Use the configuration object to read the connection string
            var optionsBuilder = new DbContextOptionsBuilder<ShippingRecorderDbContext>();
            optionsBuilder.UseSqlite(configuration.GetConnectionString("ShippingRecorderDB"));

            // Construct and return a database context
            return new ShippingRecorderDbContext(optionsBuilder.Options);
        }

        /// <summary>
        /// Create an in-memory context for unit testing
        /// </summary>
        /// <returns></returns>
        public static ShippingRecorderDbContext CreateInMemoryDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ShippingRecorderDbContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            return new ShippingRecorderDbContext(optionsBuilder.Options);
        }
    }
}
