using System.Reflection;
using MsgNet;
using MsgNet.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class MsgNetServiceCollectionExtensions
{
    public static IServiceCollection AddMsgNet(this IServiceCollection services)
    {
        return AddMsgNet(services, options => { });
    }

    public static IServiceCollection AddMsgNet(this IServiceCollection services, Action<MsgNetOptions> configure)
    {
        return AddMsgNet(services, AppDomain.CurrentDomain.GetAssemblies(), configure);
    }

    public static IServiceCollection AddMsgNet(this IServiceCollection services, IEnumerable<Assembly> assemblies, Action<MsgNetOptions> configure)
    {
        var types = assemblies.SelectMany(i => i.DefinedTypes).Where(t => t.HasGenericElements() == false && t.IsConcreteType());

        RegisterServices(typeof(IMessageReceiver<>), types, services);
        RegisterServices(typeof(IMessageReceiver<,>), types, services);
        RegisterServices(typeof(IAsyncMessageReceiver<>), types, services);
        RegisterServices(typeof(IAsyncMessageReceiver<,>), types, services);

        services.AddSingleton<IMessenger, MsgNetMessenger>();

        return services;
    }

    private static void RegisterServices(Type requestInterface, IEnumerable<TypeInfo> types, IServiceCollection services)
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

                services.AddTransient(@interface, type);
            }
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