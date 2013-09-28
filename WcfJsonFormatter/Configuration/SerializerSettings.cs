using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter.Configuration
{
    public class SerializerSettings
        : ConfigServiceElement
    {

        [ConfigurationProperty("onlyPublicConstructor", IsRequired = false, DefaultValue = false)]
        public bool OnlyPublicConstructor
        {
            get { return (bool)this["onlyPublicConstructor"]; }
        }


        [ConfigurationProperty("enablePolymorphicMembers", IsRequired = false, DefaultValue = false)]
        public bool EnablePolymorphicMembers
        {
            get { return (bool)this["enablePolymorphicMembers"]; }
        }


        public override object Key
        {
            get { return GetHashCode(); }
        }


        public override int GetHashCode()
        {
            return this.OnlyPublicConstructor.GetHashCode() - this.EnablePolymorphicMembers.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("onlyPublicConstructor: {0}, enablePolymorphicMembers: {1}", this.OnlyPublicConstructor, this.EnablePolymorphicMembers);
        }
    }
}
