using System;
using System.Collections.Generic;
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
        private readonly List<Type> knownTypes;
        private readonly Dictionary<Type, Type> resolvers;

        private List<ServiceType> serviceTypes;
        private List<ResolverType> resolverTypes;


        public ServiceTypeRegister()
        {
            this.assemblies = new HashSet<Assembly>();
            this.errorMessages = new List<string>();
            this.knownTypes = new List<Type>();
            this.resolvers = new Dictionary<Type, Type>();

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
                if (entry == null) return;

                if (serviceType.Name == "*")
                    knownTypes.AddRange(entry.GetTypes());
                else
                {
                    Type type = entry.GetType(serviceType.Name, false, true);
                    if (type != null)
                        knownTypes.Add(type);
                }
            }
        }


        private void RegisterResolverType()
        {
            foreach (ResolverType resolverType in resolverTypes)
            {
                if (resolverType == null || resolverType.WasResolved) continue;

                Assembly serviceAss = GetAssemblyFrom(resolverType.ServiceType);
                Assembly resolverAss = GetAssemblyFrom(resolverType.ServiceType);

                if (serviceAss == null || resolverAss == null) continue;

                Type serviceType = serviceAss.GetType(resolverType.ServiceType.Name, true, true);
                Type binderType = resolverAss.GetType(resolverType.BinderType.Name, true, true);

                resolverType.WasResolved = true;

                if (!resolvers.ContainsKey(serviceType))
                    resolvers.Add(serviceType, binderType);
            }
        }


        private Assembly GetAssemblyFrom(ServiceType serviceType)
        {
           Assembly ass = this.assemblies.FirstOrDefault(n => n.GetName().Name == serviceType.Assembly)
                ?? Assembly.Load(serviceType.Assembly);

            if (!assemblies.Contains(ass)) assemblies.Add(ass);

            return ass;
        }
    }
}
