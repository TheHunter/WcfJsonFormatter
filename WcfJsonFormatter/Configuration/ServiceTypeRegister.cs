using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WcfJsonFormatter.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    internal class ServiceTypeRegister
        : ConfigurationSection
    {

        private readonly Assembly entryAssembly;
        private readonly HashSet<Assembly> assemblies;
        private readonly IList<string> errorMessages;
        private readonly HashSet<Type> knownTypes;

        private readonly Dictionary<Type, Type> concreteResolvers;
        private readonly Dictionary<Type, Type> undefinedResolvers;

        private List<ServiceType> serviceTypes;
        private List<ResolverType> resolverTypes;


        public ServiceTypeRegister()
        {
            this.assemblies = new HashSet<Assembly>();
            this.errorMessages = new List<string>();
            this.knownTypes = new HashSet<Type>();

            this.concreteResolvers = new Dictionary<Type, Type>();

            this.undefinedResolvers = new Dictionary<Type, Type>();
            this.undefinedResolvers.Add(typeof(IEnumerable<>), typeof(List<>));
            this.undefinedResolvers.Add(typeof(IList<>), typeof(List<>));
            this.undefinedResolvers.Add(typeof(ICollection<>), typeof(Collection<>));
            this.undefinedResolvers.Add(typeof(IDictionary<,>), typeof(Dictionary<,>));

            this.entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                AssemblyName[] ass = entryAssembly.GetReferencedAssemblies();
                foreach (var assemblyName in ass)
                {
                    try
                    {
                        this.assemblies.Add(Assembly.Load(assemblyName));
                    }
                    catch (Exception ex)
                    {
                        errorMessages.Add(string.Format("AssemblyName: {0}, Error message: {1}", assemblyName.Name, ex.Message));
                    }
                }
            }
        }

        [ConfigurationProperty("serviceTypes", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ServiceTypeCollection<ServiceType>))]
        public ServiceTypeCollection<ServiceType> ServiceTypeCollection
        {
            get { return (ServiceTypeCollection<ServiceType>)base["serviceTypes"]; }
        }


        [ConfigurationProperty("resolverTypes", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ServiceTypeCollection<ResolverType>))]
        public ServiceTypeCollection<ResolverType> ResolverTypeCollection
        {
            get { return (ServiceTypeCollection<ResolverType>)base["resolverTypes"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        public void AddAssembly(Assembly assembly)
        {
            if (!this.assemblies.Contains(assembly))
                this.assemblies.Add(assembly);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PostDeserialize()
        {
            base.PostDeserialize();

            serviceTypes = new List<ServiceType>(this.ServiceTypeCollection.Cast<ServiceType>());
            resolverTypes = new List<ResolverType>(this.ResolverTypeCollection.Cast<ResolverType>());

            RegisterServiceType();
            RegisterResolverType();
        }


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
                    Type type = entry.GetType(serviceType.Name, false, true);
                    if (type != null)
                        knownTypes.Add(type);
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
           Assembly ass = this.assemblies.FirstOrDefault(n => n.GetName().Name == serviceType.Assembly)
                ?? Assembly.Load(serviceType.Assembly);

            assemblies.Add(ass);
            return ass;
        }


        private void RegisterResolver(Type serviceType, Type binderType)
        {
            if (serviceType.IsGenericType)
            {
                if (serviceType.IsGenericTypeDefinition)
                {
                    this.undefinedResolvers.Add(serviceType, binderType);
                }
                else
                {
                    if (binderType.IsGenericType)
                    {
                        binderType = binderType.GetGenericTypeDefinition();
                        binderType = binderType.MakeGenericType(serviceType.GetGenericArguments());
                        if (serviceType.IsAssignableFrom(binderType))
                            this.concreteResolvers.Add(serviceType, binderType);
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


        internal Type TryToNormalize(Type type)
        {
            Type ret = this.concreteResolvers.ContainsKey(type)
                            ? this.concreteResolvers[type]
                            : this.concreteResolvers.Values.FirstOrDefault(type.IsAssignableFrom);

            if (ret == null && type.IsGenericType)
            {
                //ret = this.undefinedResolvers[type];
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
            this.assemblies.Add(type.Assembly);

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
        /// <returns></returns>
        internal bool RefreshRegister()
        {
            this.RegisterResolverType();
            return resolverTypes.All(n => n.WasResolved);
        }
    }
}
