using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using WcfJsonFormatter.Exceptions;

namespace WcfJsonFormatter.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceTypeRegister
        : ConfigurationSection, IServiceRegister
    {
        private readonly HashSet<Assembly> assemblies;
        private readonly HashSet<Type> knownTypes;
        private readonly Dictionary<Type, Type> concreteResolvers;
        private readonly Dictionary<Type, Type> undefinedResolvers;
        private List<ServiceType> serviceTypes;
        private List<ResolverType> resolverTypes;
        private SerializerSettings serializerConfig;


        public ServiceTypeRegister()
        {
            this.assemblies = new HashSet<Assembly>();
            this.knownTypes = new HashSet<Type>();

            this.concreteResolvers = new Dictionary<Type, Type>();

            this.undefinedResolvers = new Dictionary<Type, Type>();
            this.undefinedResolvers.Add(typeof(IEnumerable<>), typeof(List<>));
            this.undefinedResolvers.Add(typeof(IList<>), typeof(List<>));
            this.undefinedResolvers.Add(typeof(ICollection<>), typeof(Collection<>));
            this.undefinedResolvers.Add(typeof(IDictionary<,>), typeof(Dictionary<,>));
        }


        [ConfigurationProperty("serializer", IsRequired = false, DefaultValue = null)]
        protected SerializerSettings Serializer
        {
            get { return (SerializerSettings)base["serializer"]; }
        }

        [ConfigurationProperty("serviceTypes", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ServiceTypeCollection<ServiceType>))]
        protected ServiceTypeCollection<ServiceType> ServiceTypeCollection
        {
            get { return (ServiceTypeCollection<ServiceType>)base["serviceTypes"]; }
        }


        [ConfigurationProperty("resolverTypes", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ServiceTypeCollection<ResolverType>))]
        protected ServiceTypeCollection<ResolverType> ResolverTypeCollection
        {
            get { return (ServiceTypeCollection<ResolverType>)base["resolverTypes"]; }
        }

        [ConfigurationProperty("checkOperationTypes", IsRequired = false, DefaultValue = false)]
        public bool CheckOperationTypes
        {
            get { return (bool)base["checkOperationTypes"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PostDeserialize()
        {
            base.PostDeserialize();

            serviceTypes = new List<ServiceType>(this.ServiceTypeCollection.Cast<ServiceType>());
            resolverTypes = new List<ResolverType>(this.ResolverTypeCollection.Cast<ResolverType>());
            serializerConfig = this.Serializer ?? new SerializerSettings();

            RegisterServiceType();
            RegisterResolverType();
        }

        /// <summary>
        /// 
        /// </summary>
        public SerializerSettings SerializerConfig
        {
            get { return this.serializerConfig; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        internal void AddAssembly(Assembly assembly)
        {
            if (!this.assemblies.Contains(assembly))
                this.assemblies.Add(assembly);
        }

        /// <summary>
        /// 
        /// </summary>
        private void RegisterServiceType()
        {
            foreach (ServiceType serviceType in serviceTypes)
            {
                if (serviceType == null) continue;

                Assembly entry = GetAssemblyFrom(serviceType);
                if (entry == null) continue;

                if (serviceType.Name == "*")
                {
                    this.LoadTypes(entry);
                }
                else
                {
                    try
                    {
                        Type type = entry.GetType(serviceType.Name, true, true);
                        knownTypes.Add(type);
                    }
                    catch (Exception ex)
                    {
                        throw new TypeUnresolvedException(string.Format("The type indicated in config file wasn't loaded, name: {0}", serviceType.Name), ex);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RegisterResolverType()
        {
            foreach (ResolverType resolverType in resolverTypes.Where(n => !n.WasResolved))
            {
                if (resolverType == null || resolverType.WasResolved) continue;

                Assembly serviceAss = GetAssemblyFrom(resolverType.ServiceType);
                Assembly resolverAss = GetAssemblyFrom(resolverType.ServiceType);

                if (serviceAss == null || resolverAss == null) continue;

                Type serviceType = serviceAss.GetType(resolverType.ServiceType.Name, true, true);
                Type binderType = resolverAss.GetType(resolverType.BinderType.Name, true, true);

                resolverType.WasResolved = true;

                this.RegisterResolver(serviceType, binderType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        private Assembly GetAssemblyFrom(ServiceType serviceType)
        {
            try
            {
                Assembly ass = this.assemblies.FirstOrDefault(n => n.GetName().Name == serviceType.Assembly)
                ?? Assembly.Load(serviceType.Assembly);

                assemblies.Add(ass);
                return ass;
            }
            catch (Exception ex)
            {
                throw new AssemblyUnresolvedException("Error on loading the assembly indicated in config section, see innerException for details", serviceType.Assembly, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="binderType"></param>
        private void RegisterResolver(Type serviceType, Type binderType)
        {
            if (serviceType.IsGenericType)
            {
                if (serviceType.IsGenericTypeDefinition)
                {
                    if (!this.undefinedResolvers.Keys.Contains(serviceType))
                        this.undefinedResolvers.Add(serviceType, binderType);
                }
                else
                {
                    if (binderType.IsGenericType)
                    {
                        binderType = binderType.GetGenericTypeDefinition();
                        if (binderType != null)
                        {
                            binderType = binderType.MakeGenericType(serviceType.GetGenericArguments());
                            if (serviceType.IsAssignableFrom(binderType))
                                this.concreteResolvers.Add(serviceType, binderType);
                        }
                    }
                    //else
                    //{
                    //    // error...
                    //}
                }
            }
            else
            {
                if (serviceType.IsAssignableFrom(binderType))
                {
                    this.concreteResolvers.Add(serviceType, binderType);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Type TryToNormalize(Type type)
        {
            this.InspectType(type);

            if (IsConcreteClass(type)) return type;
            
            Type ret = this.concreteResolvers.ContainsKey(type)
                            ? this.concreteResolvers[type]
                            : this.concreteResolvers.Values.FirstOrDefault(type.IsAssignableFrom);

            if (ret == null && type.IsGenericType)
            {
                KeyValuePair<Type, Type> pair = this.undefinedResolvers.FirstOrDefault(n => n.Key.Name == type.Name);
                if (pair.Value != null && pair.Value.IsGenericTypeDefinition)
                {
                    ret = pair.Value.MakeGenericType(type.GetGenericArguments());
                    this.concreteResolvers.Add(type, ret);
                }
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        private void InspectType(Type type)
        {
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                var generics = type.GetGenericArguments().Where(n => !knownTypes.Contains(n));
                foreach (var generic in generics)
                {
                    InspectType(generic);
                }
            }

            if (!IsCollection(type))
            {
                this.LoadType(type);
                if (type.IsPrimitive)
                    return;

                const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty | BindingFlags.SetProperty;
                var properties = type.GetProperties(flags).Where(n => !knownTypes.Contains(n.PropertyType));
                foreach (var property in properties)
                {
                    this.InspectType(property.PropertyType);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal Type GetResolvedType(Type type)
        {
            return this.concreteResolvers.ContainsKey(type) ? this.concreteResolvers[type] : null;
        }

        /// <summary>
        /// 
        /// </summary>
        internal IEnumerable<Type> KnownTypes
        {
            get { return this.knownTypes; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        internal void LoadType(Type type)
        {
            if (type.IsGenericTypeDefinition)
                return;

            this.knownTypes.Add(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        internal void LoadTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                this.LoadType(type);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        internal void LoadTypes(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                this.knownTypes.Add(type);
            }
            this.assemblies.Add(assembly);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isFullname"></param>
        /// <returns></returns>
        public Type GetTypeByName(string name, bool isFullname)
        {
            if (name == null)
                return null;

            return isFullname
                       ? this.knownTypes.FirstOrDefault(n => n.FullName.Equals(name))
                       : this.knownTypes.FirstOrDefault(n => n.Name.Equals(name));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal void RefreshRegister()
        {
            this.RegisterResolverType();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsConcreteClass(Type type)
        {
            if (type == null)
                return false;

            return type.IsPrimitive || (!type.IsAbstract && !type.IsInterface);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsCollection(Type type)
        {
            if (type == null)
                return false;

            if (type.IsArray)
                return true;

            Type collectionType = type.GetInterface("IEnumerable", true)
                                  ?? type.GetInterface("IEnumerable`1", true);

            return collectionType != null;
        }
    }
}
