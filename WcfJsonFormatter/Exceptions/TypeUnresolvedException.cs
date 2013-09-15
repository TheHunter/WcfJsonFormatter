using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class TypeUnresolvedException
        : ServiceException
    {
        private readonly Type type;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="typeUnresolved"></param>
        public TypeUnresolvedException(string message, Type typeUnresolved)
            :base(message)
        {
            this.type = typeUnresolved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="typeUnresolved"></param>
        /// <param name="innerException"></param>
        public TypeUnresolvedException(string message, Type typeUnresolved, Exception innerException)
            : base(message, innerException)
        {
            this.type = typeUnresolved;
        }

        /// <summary>
        /// 
        /// </summary>
        public Type TypeUnresolved { get { return this.type; } }
    }
}
