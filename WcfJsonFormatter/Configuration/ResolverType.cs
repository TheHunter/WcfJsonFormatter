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
    public class ResolverType
        : ConfigServiceElement
    {
        [ConfigurationProperty("serviceType", IsRequired = true)]
        public string ServiceType
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("binderType", IsRequired = true)]
        public string BinderType
        {
            get { return (string)this["assembly"]; }
            set { this["assembly"] = value; }
        }


        public override object Key
        {
            get { return this.ServiceType; }
        }
    }
}
