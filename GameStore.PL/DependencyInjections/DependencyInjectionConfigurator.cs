using Autofac;
using GameStore.DomainModels.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace GameStore.PL.DependencyInjections
{
    public static class DependencyInjectionConfigurator
    {
        public static void RegisterDependencies(ContainerBuilder builder, Assembly[] assemblies, IServiceCollection services)
        {
            builder.RegisterAssemblyTypes(assemblies).Where(
                x => !Attribute.IsDefined(x, typeof(SingletonAttribute))
                && !Attribute.IsDefined(x, typeof(IgnoreInjectionsAttribute))
                && !services.Any(service => service.ServiceType == x))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies).Where(
                x => Attribute.IsDefined(x, typeof(SingletonAttribute))
                && !Attribute.IsDefined(x, typeof(IgnoreInjectionsAttribute))
                && !services.Any(service => service.ServiceType == x))
                .SingleInstance();
        }
    }
}
