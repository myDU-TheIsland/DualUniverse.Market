// <copyright file="Initializer.cs" company="Paul Layne">
// Copyright (c) Paul Layne. All rights reserved.
// </copyright>

namespace DualUniverse.Market
{
    using System.Reflection;
    using DualUniverse.Market.Services;
    using DualUniverse.Market.Services.SQL;
    using Microsoft.Extensions.DependencyInjection;

    public static class Initializer
    {
        public static void InitializeCore()
        {
            // Initialize Custom SQL Mappers.
            //SqlMapper.AddTypeHandler(typeof(List<string>), new JsonTypeHandler<List<string>>());
        }

        public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
        {
            Type[] types = Assembly.Load("DualUniverse.Market").GetTypes()
                .Where(type => (type.BaseType?.IsGenericType ?? false) && type.BaseType.GetGenericTypeDefinition() == typeof(EntityRepository<>))
                .ToArray();

            foreach (Type? typeDefinition in types)
            {
                Console.WriteLine($@"Adding Singleton Repository : {typeDefinition.Name}");
                services.AddSingleton(typeDefinition);
            }

            types = Assembly.Load("DualUniverse.Market").GetTypes()
                .Where(type => typeof(IAppService).IsAssignableFrom(type) && !type.IsInterface)
                .ToArray();

            foreach (Type? typeDefinition in types)
            {
                Type type = typeDefinition.GetInterfaces().First();

                if (type == typeof(IAppService))
                {
                    Console.WriteLine($@"Adding Singleton Repository : {typeDefinition.Name}");
                    services.AddSingleton(typeDefinition);
                }
                else
                {
                    Console.WriteLine($@"Adding Singleton Repository : {type.Name}");
                    services.AddSingleton(type, typeDefinition);
                }
            }

            return services;
        }
    }
}
