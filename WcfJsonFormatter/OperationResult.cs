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
    public class OperationResult
        : OperationInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="originalType"></param>
        /// <param name="serviceRegister"></param>
        public OperationResult(string action, Type originalType, IServiceRegister serviceRegister)
            : base(action, originalType, serviceRegister.TryToNormalize)
        {
            if (this.NormalizedType == null && serviceRegister.CheckOperationTypes)
                throw new TypeUnresolvedException("The service is not able to use the given object return type for serializing / deserializing objects, in order to resolve this kind of problem, you must to use a serviceTypeRegister on *.config file", originalType);

        }

    }
}
