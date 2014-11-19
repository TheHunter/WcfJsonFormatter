using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter.Configuration
{
    /// <summary>
    /// Specifies how date formatted string into date type.
    /// </summary>
    public enum DateParseStyle
    {
        /// <summary>
        /// string date are passed into DateTime.
        /// </summary>
        DateTime = 1,
        /// <summary>
        /// string date are passed into DateTimeOffset.
        /// </summary>
        DateTimeOffset = 3,
        /// <summary>
        /// no conversion to do.
        /// </summary>
        None = 5
    }
}
