using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalType"></param>
        protected OperationInfo(Type originalType)
        {
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
    }
}
