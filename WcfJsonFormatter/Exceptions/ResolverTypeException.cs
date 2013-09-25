using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class ResolverTypeException
        : ConfigServiceException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ResolverTypeException(string message)
            :base(message)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ResolverTypeException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
