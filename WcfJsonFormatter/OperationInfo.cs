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
        private readonly Func<Type, Type> normalizer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="originalType"></param>
        /// <param name="normalizer"></param>
        protected OperationInfo(string action, Type originalType, Func<Type, Type> normalizer)
        {
            this.action = action;
            this.originalType = originalType;
            this.normalizer = normalizer;
            this.normalizedType = this.normalizer.Invoke(originalType);
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

    }
}
