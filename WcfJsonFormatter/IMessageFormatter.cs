using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMessageFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        string Action { get; }

        /// <summary>
        /// 
        /// </summary>
        OperationResult OperationResult { get; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<OperationParameter> OperationParameters { get; }

    }
}
