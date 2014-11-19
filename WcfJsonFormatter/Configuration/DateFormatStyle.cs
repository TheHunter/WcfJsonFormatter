using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter.Configuration
{
    /// <summary>
    /// Rappresents the style which date are written in JSON text.
    /// </summary>
    public enum DateFormatStyle
    {
        /// <summary>
        /// Data written in the ISO 8601 format (ex: "2012-05-01T01:15Z")
        /// </summary>
        IsoDateFormat = 0,
        /// <summary>
        /// Data written in the Microsoft format style (ex: "\/Date(12127854561247)\/")
        /// </summary>
        MSDateFormat = 2
    }
}
