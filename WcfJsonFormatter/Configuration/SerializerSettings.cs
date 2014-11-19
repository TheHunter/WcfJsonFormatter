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
        [ConfigurationProperty("dateFormatHandling", IsRequired = false, DefaultValue = DateFormatStyle.IsoDateFormat)]
        public DateFormatStyle DateFormatHandling
        {
            get { return (DateFormatStyle)this["dateFormatHandling"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("dateFormatString", IsRequired = false, DefaultValue = null)]
        public string DateFormatString
        {
            get { return this["dateFormatString"] as string; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("dateParseHandling", IsRequired = false, DefaultValue = DateParseStyle.DateTime)]
        public DateParseStyle DateParseHandling
        {
            get { return (DateParseStyle)this["dateParseHandling"]; }
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
