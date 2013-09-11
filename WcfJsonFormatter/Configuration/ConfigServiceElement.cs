using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter.Configuration
{
    public abstract class ConfigServiceElement
        : ConfigurationElement
    {
        public abstract object Key { get; }
    }
}
