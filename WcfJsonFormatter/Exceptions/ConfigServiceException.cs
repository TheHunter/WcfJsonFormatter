using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace WcfJsonFormatter.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfigServiceException
        : Exception
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ConfigServiceException(string message)
            :base(message)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ConfigServiceException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
