using System;
using System.Collections.Generic;
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
    public class ServiceType
        : ConfigServiceElement
    {
        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("assembly", IsRequired = true)]
        public string Assembly
        {
            get { return (string)this["assembly"]; }
            set { this["assembly"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override object Key
        {
            get { return GetHashCode(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return 11 * (this.Name.GetHashCode() - this.Assembly.GetHashCode());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compareTo"></param>
        /// <returns></returns>
        public override bool Equals(object compareTo)
        {
            if (compareTo == null)
                return false;

            if (compareTo is ServiceType)
                return this.GetHashCode() == compareTo.GetHashCode();

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("name: {0}, assembly: {1}", this.Name, this.Assembly);
        }
    }
}
