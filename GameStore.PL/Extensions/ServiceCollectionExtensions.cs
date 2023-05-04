using GameStore.DAL.Entities.MongoEntities;
using GameStore.PL.Configurations;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;

namespace GameStore.PL.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, MongoDbSettings settings)
        {
            if (string.IsNullOrEmpty(settings.Name))
            {
                throw new ArgumentException($"Invalid parameter {nameof(settings.Name)}");
            }

            if (string.IsNullOrEmpty(settings.Url))
            {
                throw new ArgumentException($"Invalid parameter {nameof(settings.Url)}");
            }

            services.AddSingleton<IMongoClient>(provider => new MongoClient(settings.Url));
            services.AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(settings.Name));
            return services;
        }

        public static void ConfigureNorthwindDatabase(this IServiceCollection services)
        {
            BsonClassMap.RegisterClassMap<OrderDetailsMongoEntity>(x =>
            {
                x.AutoMap();
                x.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<OrderMongoEntity>(x =>
            {
                x.AutoMap();
                x.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<ProductMongoEntity>(x =>
            {
                x.AutoMap();
                x.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<ShipperMongoEntity>(x =>
            {
                x.AutoMap();
                x.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<SupplierMongoEntity>(x =>
            {
                x.AutoMap();
                x.SetIgnoreExtraElements(true);
            });
        }
    }
}
