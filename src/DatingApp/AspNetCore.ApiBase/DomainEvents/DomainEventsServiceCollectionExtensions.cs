﻿using AspNetCore.ApiBase.DomainEvents;
using AspNetCore.ApiBase.DomainEvents.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AspNetCore.ApiBase.Cqrs
{
    public static class DomainEventsServiceCollectionExtensions
    {
        public static void AddDomainEvents(this IServiceCollection services)
        {
            services.AddDomainEventMediator();
            services.AddDomainEventHandlers(new List<Assembly>() { Assembly.GetCallingAssembly() });
        }

        public static void AddDomainEvents(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            services.AddDomainEventMediator();
            services.AddDomainEventHandlers(assemblies);
        }

        public static void AddDomainEventMediator(this IServiceCollection services)
        {
            services.TryAddTransient<IDomainEventsMediator, DomainEventsHangfireMediator>();
        }

        public static void AddDomainEventHandlers(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            List<Type> domainEventHandlerTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(x => x.GetInterfaces().Any(y => IsHandlerInterface(y)))
                .Where(x => x.Name.EndsWith("Handler"))
                .ToList();

            foreach (Type domainEventHandlerType in domainEventHandlerTypes)
            {
                AddHandlerAsService(services, domainEventHandlerType);
            }

            services.AddSingleton<IDomainEventSubscriptionsManager>(sp => {
                var subManager = new InMemoryDomainEventSubscriptionsManager();
                foreach (Type domainEventHandlerType in domainEventHandlerTypes.Where(x => x.GetInterfaces().Any(y => IsDomainEventHandlerInterface(y))))
                {
                    IEnumerable<Type> interfaceTypes = domainEventHandlerType.GetInterfaces().Where(y => IsHandlerInterface(y));

                    foreach (var interfaceType in interfaceTypes)
                    {
                        Type commandType = interfaceType.GetGenericArguments()[0];
                        subManager.AddSubscription(commandType, domainEventHandlerType);
                    }
                }
                return subManager;
            });
        }

        private static void AddHandlerAsService(IServiceCollection services, Type type)
        {
            object[] attributes = type.GetCustomAttributes(false);

            List<Type> pipeline = attributes
                .Select(x => ToDecorator(x))
                .Concat(new[] { type })
                .Reverse()
                .ToList();

            IEnumerable<Type> interfaceTypes = type.GetInterfaces().Where(y => IsHandlerInterface(y));

            foreach (var interfaceType in interfaceTypes)
            {
                Func<IServiceProvider, object> factory = BuildPipeline(pipeline, interfaceType);
                services.AddTransient(interfaceType, factory);
            }
        }

        private static Func<IServiceProvider, object> BuildPipeline(List<Type> pipeline, Type interfaceType)
        {
            List<ConstructorInfo> ctors = pipeline
                .Select(x =>
                {
                    Type type = x.IsGenericType ? x.MakeGenericType(interfaceType.GenericTypeArguments) : x;
                    return type.GetConstructors().Single();
                })
                .ToList();

            Func<IServiceProvider, object> func = provider =>
            {
                object current = null;

                foreach (ConstructorInfo ctor in ctors)
                {
                    List<ParameterInfo> parameterInfos = ctor.GetParameters().ToList();

                    object[] parameters = GetParameters(parameterInfos, current, provider);

                    current = ctor.Invoke(parameters);
                }

                return current;
            };

            return func;
        }

        private static object[] GetParameters(List<ParameterInfo> parameterInfos, object current, IServiceProvider provider)
        {
            var result = new object[parameterInfos.Count];

            for (int i = 0; i < parameterInfos.Count; i++)
            {
                result[i] = GetParameter(parameterInfos[i], current, provider);
            }

            return result;
        }

        private static object GetParameter(ParameterInfo parameterInfo, object current, IServiceProvider provider)
        {
            Type parameterType = parameterInfo.ParameterType;

            if (IsHandlerInterface(parameterType))
                return current;

            object service = provider.GetService(parameterType);
            if (service != null)
                return service;

            throw new ArgumentException($"Type {parameterType} not found");
        }

        private static Type ToDecorator(object attribute)
        {
            Type type = attribute.GetType();


            throw new ArgumentException(attribute.ToString());
        }

        private static bool IsHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            return IsDomainEventHandlerInterface(type) || IsDynamicDomainEventHandlerInterface(type);
        }

        private static bool IsDomainEventHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type typeDefinition = type.GetGenericTypeDefinition();

            return typeDefinition == typeof(IDomainEventHandler<>);
        }

        private static bool IsDynamicDomainEventHandlerInterface(Type type)
        {
            return type == typeof(IDynamicDomainEventHandler);
        }
    }
}
