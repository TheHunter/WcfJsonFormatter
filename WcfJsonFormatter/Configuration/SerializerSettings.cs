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
    public class SerializerSettings
        : ConfigServiceElement
    {
        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("onlyPublicConstructor", IsRequired = false, DefaultValue = false)]
        public bool OnlyPublicConstructor
        {
            get { return (bool)this["onlyPublicConstructor"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("enablePolymorphicMembers", IsRequired = false, DefaultValue = false)]
        public bool EnablePolymorphicMembers
        {
            get { return (bool)this["enablePolymorphicMembers"]; }
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
            return this.OnlyPublicConstructor.GetHashCode() - this.EnablePolymorphicMembers.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("onlyPublicConstructor: {0}, enablePolymorphicMembers: {1}", this.OnlyPublicConstructor, this.EnablePolymorphicMembers);
        }
    }
}
