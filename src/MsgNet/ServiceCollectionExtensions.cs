using System.Reflection;
using MsgNet;
using MsgNet.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class MsgNetServiceCollectionExtensions
{
    public static IServiceCollection AddMsgNet(this IServiceCollection services)
    {
        return AddMsgNet(services, ConfigureDefaultBuilder);
    }

    public static IServiceCollection AddMsgNet(this IServiceCollection services, Action<IMessageAppBuilder> configure)
    {
        return AddMsgNet(services, AppDomain.CurrentDomain.GetAssemblies(), configure);
    }

    public static IServiceCollection AddMsgNet(this IServiceCollection services, IEnumerable<Assembly> assemblies, Action<IMessageAppBuilder> configure)
    {
        var builder = new MessageAppBuilder(services);
        var types = assemblies.SelectMany(i => i.DefinedTypes).Where(t => t.HasGenericElements() == false && t.IsConcreteType());

        ConfigureDefaultBuilder(builder);

        ServiceLifetime serviceLifetimeSelector(Type type)
        {
            var attributes = Attribute.GetCustomAttributes(type);
            var result = (ServiceInjectionAttribute?)attributes.SingleOrDefault(attr => attr is ServiceInjectionAttribute);

            if (result == null)
            {
                return builder.DefaultMessageReceiverLifetime;
            }

            return result.Lifetime;
        }

        configure(builder);

        FilterAndRegisterServices(typeof(IMessageReceiver<>), types, services, serviceLifetimeSelector);
        FilterAndRegisterServices(typeof(IMessageReceiver<,>), types, services, serviceLifetimeSelector);
        FilterAndRegisterServices(typeof(IAsyncMessageReceiver<>), types, services, serviceLifetimeSelector);
        FilterAndRegisterServices(typeof(IAsyncMessageReceiver<,>), types, services, serviceLifetimeSelector);

        return services;
    }

    private static void ConfigureDefaultBuilder(IMessageAppBuilder builder)
    {
        var services = builder.GetServiceDescriptors();

        builder.DefaultMessageReceiverLifetime = ServiceLifetime.Transient;

        services.AddSingleton<IMessenger, MsgNetMessenger>();
    }

    private static void FilterAndRegisterServices(Type requestInterface, IEnumerable<TypeInfo> types, IServiceCollection services, Func<Type, ServiceLifetime> serviceLifetimeSelector)
    {
        var registeredInterfaces = new List<Type>();

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces()
                .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == requestInterface)
                .Where(t => t.IsAssignableFrom(type));

            foreach (var @interface in interfaces)
            {
                var registeredInterface = registeredInterfaces.Find(t => t == @interface);

                if (registeredInterface != null)
                {
                    throw new Exception($"Implementation type {type.FullName} has duplicated interfaces. Duplicated interfaces: {registeredInterface.FullName}, {@interface.FullName}");
                }

                RegisterService(@interface, type, serviceLifetimeSelector(type), services);
            }
        }
    }

    private static void RegisterService(Type @interface, TypeInfo implementationType, ServiceLifetime serviceLifetime, IServiceCollection services)
    {
        if (serviceLifetime == ServiceLifetime.Transient)
        {
            services.AddTransient(@interface, implementationType);
        }
        else if (serviceLifetime == ServiceLifetime.Scoped)
        {
            services.AddScoped(@interface, implementationType);
        }
        else if (serviceLifetime == ServiceLifetime.Singleton)
        {
            services.AddSingleton(@interface, implementationType);
        }
        else
        {
            throw new MsgNetException($"Unsupported service life time: {serviceLifetime}");
        }
    }

    private static bool HasGenericElements(this Type type)
    {
        return type.IsGenericTypeDefinition || type.ContainsGenericParameters;
    }

    private static bool IsConcreteType(this Type type)
    {
        return type.IsAbstract == false && type.IsInterface == false;
    }
}