using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Text;
using WcfJsonFormatter.Configuration;

namespace WcfJsonFormatter
{
    /// <summary>
    /// A register of object types which registers all known types from operations services.
    /// </summary>
    public static class DynamicTypeRegister
    {
        private static readonly HashSet<Type> KnownTypes;
        private static readonly HashSet<string> Assemblies;
        private static readonly Dictionary<Type, Type> Converters;

        /// <summary>
        /// 
        /// </summary>
        static DynamicTypeRegister()
        {
            KnownTypes = new HashSet<Type>();
            Assemblies = new HashSet<string>();

            Converters = new Dictionary<Type, Type>();
            Converters.Add(typeof(IEnumerable<>), typeof(List<>));
            Converters.Add(typeof(IList<>), typeof(List<>));
            Converters.Add(typeof(ICollection<>), typeof(Collection<>));
            Converters.Add(typeof(IDictionary<,>), typeof(Dictionary<,>));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static Type NormalizeType(Type arg)
        {
            #region OLD CODE
            //if (!arg.IsAbstract && !arg.IsInterface) 
            //    return arg;

            //var t0 = Converters.FirstOrDefault(n => n.Key.Name == arg.Name);
            //if (t0.Value != null)
            //{
            //    if (!t0.Value.IsGenericType) return t0.Value;

            //    return t0.Value.MakeGenericType(arg.GetGenericArguments());
            //}

            //// non generico
            //if (!arg.IsGenericType)
            //    return Converters.Values.FirstOrDefault(arg.IsAssignableFrom);

            //return Converters.Values.Select(current => current.MakeGenericType(arg.GetGenericArguments()))
            //                        .FirstOrDefault(arg.IsAssignableFrom);
            #endregion

            //ICollection<>
            //BindingElementCollection cl1 = new BindingElementCollection();
            //Collection<BindingElement> cl2 = new Collection<BindingElement>();
            //cl2 = cl1;

            //EntryCollection a;

            if (arg.IsPrimitive || (!arg.IsAbstract && !arg.IsInterface))
                return arg;

            KeyValuePair<Type, Type> t0 = Converters.FirstOrDefault(n => n.Key.Name == arg.Name);

            if (t0.Value != null)
            {
                if (!t0.Value.IsGenericType)
                    return t0.Value;

                return t0.Value.MakeGenericType(arg.GetGenericArguments());
            }

            // non generico
            if (!arg.IsGenericType)
                return Converters.Values.FirstOrDefault(arg.IsAssignableFrom);

            Type[] genericArgs = arg.GetGenericArguments();
            //return Converters.Values.Select(current => current.MakeGenericType(genericArgs))
            //                        .FirstOrDefault(arg.IsAssignableFrom);

            Type supplier = null;
            // tutti i VALORI del dizionario devono essere oggetti di classe concreta..
            Converters.Values.All
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
        /// <param name="serviceType"></param>
        internal static void RegisterServiceType(ServiceType serviceType)
        {
            if (serviceType != null)
            {
                AssemblyName assemblyName = Assembly.GetEntryAssembly()
                                           .GetReferencedAssemblies()
                                           .FirstOrDefault(n => n.Name == serviceType.Assembly);

                if (assemblyName != null)
                {
                    Assembly assembly = Assembly.Load(assemblyName);
                    if (serviceType.Name == "*")
                        LoadTypes(assembly.GetTypes());
                    else
                        LoadType(assembly.GetType(serviceType.Name, false, true));
                }
            }
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
            }
        }

        /// <summary>
        /// Registers all types from the given collection.
        /// </summary>
        /// <param name="types"></param>
        public static void LoadTypes(IEnumerable<Type> types)
        {
            lock (KnownTypes)
            {
                if (types != null)
                {
                    types.All
                        (
                            type =>
                            {
                                if (!KnownTypes.Any(n => n.Name.Equals(type.Name)))
                                    KnownTypes.Add(type);

                                return true;
                            }
                        );
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        public static void LoadTypes(Assembly assembly)
        {
            lock (KnownTypes)
            {
                string assName = assembly.GetName().Name;
                if (Assemblies.Contains(assName))
                    return;

                Assemblies.Add(assName);
            }
            LoadTypes(assembly.GetTypes());
        }

        /// <summary>
        /// Gets an object type from the given name.
        /// </summary>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public static Type GetTypeByShortName(string shortName)
        {
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
            lock (KnownTypes)
            {
                return KnownTypes.FirstOrDefault(n => n.FullName.Equals(fullName));
            }
        }

    }
}
