using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter.Configuration
{
    public class ServiceTypeRegister
        : ConfigurationSection
    {
        
        [ConfigurationProperty("serviceTypes", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ServiceTypeCollection), AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ServiceTypeCollection ServiceTypeCollection
        {
            get { return (ServiceTypeCollection)base["serviceTypes"]; }
        }

    }
}
