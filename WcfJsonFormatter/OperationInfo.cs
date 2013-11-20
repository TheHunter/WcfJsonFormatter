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
        private readonly IServiceRegister serviceRegister;
        private readonly OperationInfoType operationType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="originalType"></param>
        /// <param name="operationType"></param>
        /// <param name="serviceRegister"></param>
        protected OperationInfo(string action, Type originalType, OperationInfoType operationType, IServiceRegister serviceRegister)
        {
            this.action = action;
            this.originalType = originalType;
            this.serviceRegister = serviceRegister;
            this.operationType = operationType;
            this.normalizedType = this.serviceRegister.TryToNormalize(originalType);

            string messageError;
            switch (operationType)
            {
                case OperationInfoType.Parameter:
                    messageError = "The service is not able to use the given object parameter type for serializing / deserializing objects, in order to resolve this kind of problem, you must to use a serviceTypeRegister on *.config file";
                    break;
                case OperationInfoType.Result:
                    messageError = "The service is not able to use the given object return type for serializing / deserializing objects, in order to resolve this kind of problem, you must to use a serviceTypeRegister on *.config file";
                    break;
                default:
                    messageError = "Operation Type unknown.";
                    break;
            }

            if (normalizedType == null && serviceRegister.CheckOperationTypes)
                throw new TypeUnresolvedException(messageError, originalType);

            //
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

        /// <summary>
        /// 
        /// </summary>
        public OperationInfoType OperationType { get { return this.operationType; } }
    }
}
