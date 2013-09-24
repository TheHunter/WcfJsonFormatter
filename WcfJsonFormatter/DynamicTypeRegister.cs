using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Text;
using WcfJsonFormatter.Configuration;

namespace WcfJsonFormatter
{
    /// <summary>
    /// A register of object types which registers all known types used by operations services.
    /// </summary>
    public static class DynamicTypeRegister
    {
        private static readonly HashSet<Type> KnownTypes;
        private static readonly Dictionary<Type, Type> Resolvers;
        private static readonly HashSet<Assembly> Assemblies;
        private static readonly ServiceTypeRegister ConfigRegister;

        /// <summary>
        /// 
        /// </summary>
        static DynamicTypeRegister()
        {
            KnownTypes = new HashSet<Type>();

            Resolvers = new Dictionary<Type, Type>();
            Resolvers.Add(typeof(IEnumerable<>), typeof(List<>));
            Resolvers.Add(typeof(IList<>), typeof(List<>));
            Resolvers.Add(typeof(ICollection<>), typeof(Collection<>));
            Resolvers.Add(typeof(IDictionary<,>), typeof(Dictionary<,>));

            Assemblies = new HashSet<Assembly>();

            ConfigRegister = ConfigurationManager.GetSection("serviceTypeRegister") as ServiceTypeRegister;
            LoadServiceTypeRegister();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool IsConcreteType(Type arg)
        {
            return arg.IsPrimitive || (!arg.IsAbstract && !arg.IsInterface);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static Type NormalizeType(Type arg)
        {
            if (IsConcreteType(arg))
                return arg;

            KeyValuePair<Type, Type> t0 = Resolvers.FirstOrDefault(n => n.Key.Name == arg.Name);

            if (t0.Value != null)
            {
                if (!t0.Value.IsGenericType)
                    return t0.Value;

                return t0.Value.MakeGenericType(arg.GetGenericArguments());
            }

            // non generic
            if (!arg.IsGenericType)
                return Resolvers.Values.FirstOrDefault(arg.IsAssignableFrom);

            Type[] genericArgs = arg.GetGenericArguments();
            
            Type supplier = null;
            // tutti i VALORI del dizionario devono essere oggetti di classe concreta..
            Resolvers.Values.All
                (
                    type =>
                        {
                            try
                            {
                                supplier = null;

                                if (arg.IsAssignableFrom(type))
                                {
                                    supplier = type;
                                    return false;
                                }

                                if (type.IsGenericType)
                                {
                                    supplier = type.MakeGenericType(genericArgs);

                                    if (arg.IsAssignableFrom(supplier))
                                        return false;
                                }
                            }
                            catch (Exception)
                            {

                            }

                            return true;
                        }
                );

            return supplier;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void LoadServiceTypeRegister()
        {
            if (ConfigRegister == null) return;

            DynamicTypeRegister.RegisterServiceType(ConfigRegister.ServiceTypeCollection);
            DynamicTypeRegister.RegisterResolverType(ConfigRegister.ResolverTypeCollection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceTypes"></param>
        internal static void RegisterServiceType(ServiceTypeCollection<ServiceType> serviceTypes)
        {
            if (serviceTypes != null)
            {
                for (int index = 0; index < serviceTypes.Count; index++ )
                {
                    ServiceType serviceType = serviceTypes[index];

                    if (serviceType == null) continue;

                    Assembly entry = GetAssemblyFrom(serviceType);
                    if (entry == null) return;

                    if (serviceType.Name == "*")
                        LoadTypes(entry.GetTypes());
                    else
                        LoadType(entry.GetType(serviceType.Name, false, true));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolverTypes"></param>
        internal static void RegisterResolverType(ServiceTypeCollection<ResolverType> resolverTypes)
        {
            if (resolverTypes != null)
            {
                for (int index = 0; index < resolverTypes.Count; index++)
                {
                    ResolverType resolverType = resolverTypes[index];
                    if (resolverType == null || resolverType.WasResolved) continue;

                    Assembly serviceAss = GetAssemblyFrom(resolverType.ServiceType);
                    Assembly resolverAss = GetAssemblyFrom(resolverType.ServiceType);

                    if (serviceAss == null || resolverAss == null) continue;

                    Type serviceType = serviceAss.GetType(resolverType.ServiceType.Name, true, true);
                    Type binderType = resolverAss.GetType(resolverType.BinderType.Name, true, true);

                    resolverType.WasResolved = true;
                    lock (Resolvers)
                    {
                        if (!Resolvers.ContainsKey(serviceType))
                            Resolvers.Add(serviceType, binderType);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        private static Assembly GetAssemblyFrom(ServiceType serviceType)
        {
            if (serviceType == null) return null;

            AssemblyName assemblyName = Assembly.GetEntryAssembly()
                                           .GetReferencedAssemblies()
                                           .FirstOrDefault(n => n.Name == serviceType.Assembly);

            return assemblyName != null ? Assembly.Load(assemblyName) : Assemblies.FirstOrDefault( n => n.GetName().Name == serviceType.Assembly);
        }

        /// <summary>
        /// Registers the given object type if there's no registered.
        /// </summary>
        /// <param name="current"></param>
        public static void LoadType(Type current)
        {
            lock (KnownTypes)
            {
                if (!KnownTypes.Contains(current))
                    KnownTypes.Add(current);

                Assembly ass = current.Assembly;
                if (!Assemblies.Contains(ass))
                    Assemblies.Add(ass);
            }
        }

        /// <summary>
        /// Registers all types from the given collection.
        /// </summary>
        /// <param name="types"></param>
        public static void LoadTypes(IEnumerable<Type> types)
        {
            if (types != null)
            {
                types.All
                    (
                        type =>
                        {
                            LoadType(type);
                            return true;
                        }
                    );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        public static void LoadTypes(Assembly assembly)
        {
            if (assembly == null)
                return;

            LoadTypes(assembly.GetTypes());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        private static void InspectType(Type type)
        {
            if (type != null)
            {

                if (type.IsArray)
                {
                    InspectType(type.GetElementType());
                    return;
                }

                if (type.IsGenericType)
                    InspectTypes(type.GetGenericArguments());

                if (IsCollectionType(type))
                {
                    LoadType(type);
                    return;
                }

                const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;
                var properties = type.GetProperties(flags);
                foreach (var propertyInfo in properties)
                {
                    LoadType(propertyInfo.PropertyType);
                }
            }
        }


        public static void InspectTypes(IEnumerable<Type> types)
        {
            if (types == null) return;

            foreach (var type in types)
            {
                InspectType(type);
            }

            if (ConfigRegister != null)
                RegisterResolverType(ConfigRegister.ResolverTypeCollection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCollectionType(Type type)
        {
            if (type == null)
                return false;

            if (type.IsArray)
                return true;

            Type collectionType = type.GetInterface("IEnumerable", true)
                                  ?? type.GetInterface("IEnumerable`1", true);

            return collectionType != null;
        }

        /// <summary>
        /// Gets an object type from the given name.
        /// </summary>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public static Type GetTypeByShortName(string shortName)
        {
            if (string.IsNullOrEmpty(shortName))
                return null;

            lock (KnownTypes)
            {
                return KnownTypes.FirstOrDefault(n => n.Name.Equals(shortName));
            }
        }

        /// <summary>
        /// Gets the object type from the given fullname.
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static Type GetTypeByFullName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            lock (KnownTypes)
            {
                return KnownTypes.FirstOrDefault(n => n.FullName.Equals(fullName));
            }
        }

    }
}
