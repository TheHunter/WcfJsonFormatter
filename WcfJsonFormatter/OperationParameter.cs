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
        public OperationParameter(string name, string action, Type originalType)
            :base(originalType, action)
        {
            this.name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get { return this.name; } }

    }

}
