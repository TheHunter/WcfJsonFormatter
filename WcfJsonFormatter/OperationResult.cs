using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter
{
    /// <summary>
    /// 
    /// </summary>
    public class OperationResult
        : OperationInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalType"></param>
        /// <param name="action"></param>
        public OperationResult(Type originalType, string action)
            : base(originalType, action)
        {
        }

    }
}
