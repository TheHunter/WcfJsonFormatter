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
    public class ServiceException
        : Exception
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ServiceException(string message)
            :base(message)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ServiceException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
