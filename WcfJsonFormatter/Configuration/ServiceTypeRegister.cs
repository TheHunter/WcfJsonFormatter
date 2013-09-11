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
    public class ServiceTypeRegister
        : ConfigurationSection
    {
        
        [ConfigurationProperty("serviceTypes", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ServiceTypeCollection<ServiceType>), AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ServiceTypeCollection<ServiceType> ServiceTypeCollection
        {
            get { return (ServiceTypeCollection<ServiceType>)base["serviceTypes"]; }
        }


        [ConfigurationProperty("resolverTypes", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ServiceTypeCollection<ResolverType>), AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ServiceTypeCollection<ResolverType> ResolverTypeCollection
        {
            get { return (ServiceTypeCollection<ResolverType>)base["resolverTypes"]; }
        }
    }
}
