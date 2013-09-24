using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter
{
    /// <summary>
    /// 
    /// </summary>
    public class OperationParameter
        : OperationInfo
    {
        private readonly string name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <param name="originalType"></param>
        /// <param name="normalizer"></param>
        public OperationParameter(string name, string action, Type originalType, Func<Type, Type> normalizer)
            : base(action, originalType, normalizer)
        {
            this.name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get { return this.name; } }

    }

}
