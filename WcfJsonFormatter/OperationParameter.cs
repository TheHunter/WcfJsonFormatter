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
        /// <param name="originalType"></param>
        public OperationParameter(string name, Type originalType)
            :base(originalType)
        {
            this.name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get { return this.name; } }

        
    }

}
