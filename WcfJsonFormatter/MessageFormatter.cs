using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Description;
using System.Text;
using WcfJsonFormatter.Configuration;
using WcfJsonFormatter.Exceptions;

namespace WcfJsonFormatter
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MessageFormatter
        : IMessageFormatter
    {
        private readonly string action;
        private readonly List<OperationParameter> operationParameters;
        private readonly OperationResult operationResult;

        /// <summary>
        /// 
        /// </summary>
        static MessageFormatter()
        {
            var register = ConfigurationManager.GetSection("serviceTypeRegister") as ServiceTypeRegister;
            DynamicTypeRegister.LoadServiceTypeRegister(register);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="parameters"></param>
        /// <param name="returnType"></param>
        protected MessageFormatter(string action, IEnumerable<ParameterInfo> parameters, Type returnType)
        {
            this.action = action;
            this.operationParameters = new List<OperationParameter>
                (
                    parameters.Select(n => new OperationParameter(n.Name, action, n.ParameterType))
                );
            //new ServiceOperationException("The service operation cannot be invoked because It has wrong parameters, see innerException for details.", action, ex);

            this.operationResult = new OperationResult(returnType, action);
            //new ServiceOperationException("The service operation cannot be invoked because It has an invalid return object type, see innerException for details", action, ex);
        }

        /// <summary>
        /// 
        /// </summary>
        public string Action { get { return this.action; } }

        /// <summary>
        /// 
        /// </summary>
        public OperationResult OperationResult { get { return this.operationResult; } }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<OperationParameter> OperationParameters { get { return this.operationParameters; } }
    }
}
