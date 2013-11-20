using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOperationInfo
    {
        /// <summary>
        /// 
        /// </summary>
        Type OrginalType { get; }

        /// <summary>
        /// 
        /// </summary>
        Type NormalizedType { get; }

        /// <summary>
        /// 
        /// </summary>
        string Action { get; }

        /// <summary>
        /// 
        /// </summary>
        OperationInfoType OperationType { get; }
    }
}
