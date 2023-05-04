using GameStore.DAL.Entities;
using GameStore.DAL.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace GameStore.PL.Configurations
{
    public static class LoggingBuilderExtension
    {
        private static readonly string OutputTemplate =
            @"[{Level:u3}]|{Timestamp:yyyy-MM-dd HH:mm:ss.fff}|{CorrelationId}|{SourceContext}|{Message:lj}|{NewLine}{Exception}";

        public static ILoggingBuilder AddGenericLogging(this ILoggingBuilder loggingBuilder, HostBuilderContext context)
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
                .WriteTo.Logger(l =>
                {
                    l.WriteTo.MongoDBBson(cfg =>
                    {
                        var mongoDbInstance = new MongoClient(
                            context.Configuration["MongoDbSettings:Url"])
                            .GetDatabase(context.Configuration["MongoDbSettings:Name"]);
                        cfg.SetMongoDatabase(mongoDbInstance);
                        cfg.SetCollectionName("Logs");
                        cfg.SetBatchPeriod(TimeSpan.FromSeconds(10));
                        cfg.SetCreateCappedCollection(10);
                    });
                    l.Destructure.ByTransforming<UserEntity>(cfg =>
                        cfg.Password = null);
                    l.Filter.ByIncludingOnly(e =>
                        e.Properties.GetValueOrDefault("SourceContext") is ScalarValue sv &&
                        sv.Value.ToString().Contains(typeof(GoodsRepository).Namespace));
                })
                .WriteTo.Console(outputTemplate: OutputTemplate)
                .WriteTo.File(
                    "Logs/log.log",
                    outputTemplate: OutputTemplate,
                    shared: true,
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 1000000,
                    rollOnFileSizeLimit: true);

            if (context.Configuration.GetSection("Serilog") != null)
            {
                loggingBuilder.AddConfiguration(context.Configuration);
                loggerConfiguration.ReadFrom.Configuration(context.Configuration);
            }

            loggerConfiguration = loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.WithCorrelationId();

            Logger logger = loggerConfiguration.CreateLogger();
            Log.Logger = logger;

            loggingBuilder.AddProvider(new SerilogLoggerProvider(logger));

            return loggingBuilder;
        }
    }
}
