using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WcfJsonFormatter.Exceptions;

namespace WcfJsonFormatter
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class OperationInfo
        : IOperationInfo
    {
        private readonly Type originalType;
        private readonly Type normalizedType;
        private readonly string action;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalType"></param>
        /// <param name="action"></param>
        protected OperationInfo(Type originalType, string action)
        {
            this.action = action;
            this.originalType = originalType;
            this.normalizedType = DynamicTypeRegister.NormalizeType(originalType);
        }

        /// <summary>
        /// 
        /// </summary>
        public Type OrginalType { get { return this.originalType; } }

        /// <summary>
        /// 
        /// </summary>
        public Type NormalizedType { get { return this.normalizedType; } }

        /// <summary>
        /// 
        /// </summary>
        public string Action { get { return this.action; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal static TypeUnresolvedException MakeGenericError(OperationInfo info)
        {
            return new TypeUnresolvedException("The service is not able to use the given object type for serializing / deserializing objects, in order for resolving this kind of problem, you must to use a serviceTypeRegister on file *.config", info.originalType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <returns></returns>
        internal static ServiceOperationException MakeOperationException(OperationInfo info, string message, Exception innerException)
        {
            return new ServiceOperationException(message, info.action, innerException);
        }
    }
}
