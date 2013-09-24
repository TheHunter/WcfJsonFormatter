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
        private static readonly ServiceTypeRegister ConfigRegister;

        /// <summary>
        /// 
        /// </summary>
        static DynamicTypeRegister()
        {
            ConfigRegister = ConfigurationManager.GetSection("serviceTypeRegister") as ServiceTypeRegister
                            ?? new ServiceTypeRegister();
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

            lock (ConfigRegister)
            {
                if (arg.IsGenericType && !arg.IsGenericTypeDefinition)
                    ConfigRegister.LoadTypes(arg.GetGenericArguments());
            }

            if (IsConcreteType(arg))
                return arg;

            Type supplier;
            lock (ConfigRegister)
            {
                supplier = ConfigRegister.TryToNormalize(arg);
            }
            return supplier;
        }

        
        //private static void LoadServiceTypeRegister()
        //{
        //    if (ConfigRegister == null) return;

        //    DynamicTypeRegister.RegisterServiceType(ConfigRegister.ServiceTypeCollection);
        //    DynamicTypeRegister.RegisterResolverType(ConfigRegister.ResolverTypeCollection);
        //}

        
        //internal static void RegisterServiceType(ServiceTypeCollection<ServiceType> serviceTypes)
        //{
        //    if (serviceTypes != null)
        //    {
        //        for (int index = 0; index < serviceTypes.Count; index++ )
        //        {
        //            ServiceType serviceType = serviceTypes[index];

        //            if (serviceType == null) continue;

        //            Assembly entry = GetAssemblyFrom(serviceType);
        //            if (entry == null) return;

        //            if (serviceType.Name == "*")
        //                LoadTypes(entry.GetTypes());
        //            else
        //                LoadType(entry.GetType(serviceType.Name, false, true));
        //        }
        //    }
        //}

        
        //internal static void RegisterResolverType(ServiceTypeCollection<ResolverType> resolverTypes)
        //{
        //    if (resolverTypes != null)
        //    {
        //        for (int index = 0; index < resolverTypes.Count; index++)
        //        {
        //            ResolverType resolverType = resolverTypes[index];
        //            if (resolverType == null || resolverType.WasResolved) continue;

        //            Assembly serviceAss = GetAssemblyFrom(resolverType.ServiceType);
        //            Assembly resolverAss = GetAssemblyFrom(resolverType.ServiceType);

        //            if (serviceAss == null || resolverAss == null) continue;

        //            Type serviceType = serviceAss.GetType(resolverType.ServiceType.Name, true, true);
        //            Type binderType = resolverAss.GetType(resolverType.BinderType.Name, true, true);

        //            resolverType.WasResolved = true;
        //            lock (Resolvers)
        //            {
        //                if (!Resolvers.ContainsKey(serviceType))
        //                    Resolvers.Add(serviceType, binderType);
        //            }
        //        }
        //    }
        //}

        
        //private static Assembly GetAssemblyFrom(ServiceType serviceType)
        //{
        //    if (serviceType == null) return null;

        //    AssemblyName assemblyName = Assembly.GetEntryAssembly()
        //                                   .GetReferencedAssemblies()
        //                                   .FirstOrDefault(n => n.Name == serviceType.Assembly);

        //    return assemblyName != null ? Assembly.Load(assemblyName) : Assemblies.FirstOrDefault( n => n.GetName().Name == serviceType.Assembly);
        //}

        /// <summary>
        /// Registers the given object type if there's no registered.
        /// </summary>
        /// <param name="current"></param>
        public static void LoadType(Type current)
        {
            if (current == null)
                return;

            lock (ConfigRegister)
            {
                ConfigRegister.LoadType(current);
            }
        }

        /// <summary>
        /// Registers all types from the given collection.
        /// </summary>
        /// <param name="types"></param>
        public static void LoadTypes(IEnumerable<Type> types)
        {
            if (types == null)
                return;

            lock (ConfigRegister)
            {
                ConfigRegister.LoadTypes(types);
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

            lock (ConfigRegister)
            {
                ConfigRegister.LoadTypes(assembly);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsCollectionType(Type type)
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

            lock (ConfigRegister)
            {
                return ConfigRegister.KnownTypes.FirstOrDefault(n => n.Name.Equals(shortName));
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

            lock (ConfigRegister)
            {
                return ConfigRegister.KnownTypes.FirstOrDefault(n => n.FullName.Equals(fullName));
            }
        }

        public static void RefreshServiceRegister()
        {
            ConfigRegister.RefreshRegister();
        }
    }
}
