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
    public abstract class ConfigServiceElement
        : ConfigurationElement
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract object Key { get; }

        /// <inheritdoc/>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            if (name.Equals("xmlns"))
                return true;

            return base.OnDeserializeUnrecognizedAttribute(name, value);
        }
    }
}
