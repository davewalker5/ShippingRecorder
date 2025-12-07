using System.Diagnostics;
using System.Reflection;
using System.Text;
using ShippingRecorder.Api.Interfaces;
using ShippingRecorder.Api.Services;
using ShippingRecorder.BusinessLogic.Factory;
using ShippingRecorder.BusinessLogic.Logging;
using ShippingRecorder.Data;
using ShippingRecorder.Entities.Config;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ShippingRecorder.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Read the configuration file
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Configure strongly typed application settings
            IConfigurationSection section = configuration.GetSection("ApplicationSettings");
            builder.Services.Configure<ShippingRecorderApplicationSettings>(section);
            var settings = section.Get<ShippingRecorderApplicationSettings>();

            // Configure the flight log DB context and business logic
            var connectionString = configuration.GetConnectionString("ShippingRecorderDB");
            builder.Services.AddScoped<ShippingRecorderDbContext>();
            builder.Services.AddDbContextPool<ShippingRecorderDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });

            // Get the version number and application title
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
            var title = $"Flight Recorder API v{info.FileVersion}";

            // Create the file logger and log the startup messages
            var logger = new FileLogger();
            logger.Initialise(settings.LogFile, settings.MinimumLogLevel);
            logger.LogMessage(Severity.Info, new string('=', 80));
            logger.LogMessage(Severity.Info, title);

            // Log the connection string
            var message = $"Database connection string = {connectionString}";
            logger.LogMessage(Severity.Info, message);

            // Register the logger with the DI framework
            builder.Services.AddSingleton<IShippingRecorderLogger>(x => logger);

            // Configure the business logic
            builder.Services.AddSingleton<ShippingRecorderApplicationSettings>(settings);
            builder.Services.AddScoped<ShippingRecorderFactory>();
            builder.Services.AddScoped<IUserService, UserService>();

            // Configure JWT
            byte[] key = Encoding.ASCII.GetBytes(settings!.Secret);
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            var app = builder.Build();

            // Configure the exception handling middleware to write to the log file
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    // Get an instance of the error handling feature
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = errorFeature?.Error;

                    if (exception != null)
                    {
                        // Log the exception
                        var logger = context.RequestServices.GetRequiredService<IShippingRecorderLogger>();
                        logger.LogMessage(Severity.Error, exception.Message);
                        logger.LogException(exception);

                        // Set a 500 response code and return the exception details in the response
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "application/json";

                        var response = new
                        {
                            Message = exception.Message
                        };

                        await context.Response.WriteAsJsonAsync(response);
                    }
                });
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
