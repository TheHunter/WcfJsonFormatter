using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    internal class ServiceTypeRegister
        : ConfigurationSection
    {

        [ConfigurationProperty("serviceTypes", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ServiceTypeCollection<ServiceType>))]
        public ServiceTypeCollection<ServiceType> ServiceTypeCollection
        {
            get { return (ServiceTypeCollection<ServiceType>)base["serviceTypes"]; }
        }


        [ConfigurationProperty("resolverTypes", IsDefaultCollection = false)]
        public ServiceTypeCollection<ResolverType> ResolverTypeCollection
        {
            get { return (ServiceTypeCollection<ResolverType>)base["resolverTypes"]; }
        }
    }
}
