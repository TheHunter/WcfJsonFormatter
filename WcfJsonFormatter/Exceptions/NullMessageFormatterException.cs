using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class NullMessageFormatterException
        : ConfigServiceException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public NullMessageFormatterException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public NullMessageFormatterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
