using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter.Configuration
{
    /// <summary>
    /// Class ConfigServiceElement.
    /// </summary>
    public abstract class ConfigServiceElement
        : ConfigurationElement
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public abstract object Key { get; }

        /// <summary>
        /// Gets a value indicating whether an unknown attribute is encountered during deserialization.
        /// </summary>
        /// <param name="name">The name of the unrecognized attribute.</param>
        /// <param name="value">The value of the unrecognized attribute.</param>
        /// <returns>true when an unknown attribute is encountered while deserializing; otherwise, false.</returns>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            if (name.Equals("xmlns"))
                return true;

            return base.OnDeserializeUnrecognizedAttribute(name, value);
        }
    }
}
