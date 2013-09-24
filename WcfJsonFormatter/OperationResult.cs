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
        /// <param name="action"></param>
        /// <param name="originalType"></param>
        /// <param name="normalizer"></param>
        public OperationResult(string action, Type originalType, Func<Type, Type> normalizer)
            : base(action, originalType, normalizer)
        {
        }

    }
}
