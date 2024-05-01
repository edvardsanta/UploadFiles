using InfluxDB.Client;
using MassTransit;
using Microsoft.AspNetCore.Http.Features;
using System.Collections.Immutable;
using UploadFiles.Services.Interfaces;
using UploadFiles.Services.Services.Upload;
using UploadFiles.Shared.Configuration.DB;
namespace UploadFiles.Configurations
{
    public static class ServicesConfiguration
    {
        private static ImmutableList<IFileHandler> ConfigureFileHandlers() => [
                new PDFUpload(),
                new XLSXUpload()
            ];

        public static void ConfigureServices(IServiceCollection services)
        {
            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 209715200;
            });
            ConfigureMassTransit(services, config);

            services.AddScoped(_ => ConfigureFileHandlers());
            services.AddScoped<UploadManager>();

            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
            });


            var influxDBSettings = config.GetSection("InfluxDB").Get<InfluxDBConfig>();
            if (influxDBSettings != null)
            {
                services.AddSingleton<IInfluxDBClient>(_ => new InfluxDBClient(influxDBSettings.Url, influxDBSettings.Token));
            }
        }

        private static void ConfigureMassTransit(IServiceCollection services, IConfiguration config)
        {
            var rabbitMqUri = config["RabbitMq:Uri"];
            var rabbitMqUser = config["RabbitMq:Username"];
            var rabbitMqPass = config["RabbitMq:Password"];

            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqUri, "/", h =>
                    {
                        h.Username(rabbitMqUser);
                        h.Password(rabbitMqPass);
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }

}
