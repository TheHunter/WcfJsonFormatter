using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class AssemblyUnresolvedException
        : ConfigServiceException
    {

        private readonly string assemblyName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="assemblyName"></param>
        public AssemblyUnresolvedException(string message, string assemblyName)
            :base(message)
        {
            this.assemblyName = assemblyName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="assemblyName"></param>
        /// <param name="innerException"></param>
        public AssemblyUnresolvedException(string message, string assemblyName, Exception innerException)
            :base(message, innerException)
        {
            this.assemblyName = assemblyName;
        }

        /// <summary>
        /// 
        /// </summary>
        public string AssemblyName { get { return this.assemblyName; } }
    }
}
