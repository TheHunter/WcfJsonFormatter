using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceOperationException
        : ConfigServiceException
    {
        private readonly string operationName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="operationName"></param>
        public ServiceOperationException(string message, string operationName)
            :base(message)
        {
            this.operationName = operationName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="operationName"></param>
        /// <param name="innerException"></param>
        public ServiceOperationException(string message, string operationName, Exception innerException)
            : base(message, innerException)
        {
            this.operationName = operationName;
        }

        /// <summary>
        /// 
        /// </summary>
        public string OperationName { get { return this.operationName; } }

    }
}
