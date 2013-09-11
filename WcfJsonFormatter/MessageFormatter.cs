using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Description;
using System.Text;
using WcfJsonFormatter.Configuration;

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
            if (register != null)
            {
                for (int index = 0; index < register.ServiceTypeCollection.Count; index++)
                {
                    DynamicTypeRegister.RegisterServiceType(register.ServiceTypeCollection[index]);
                }
            }
                
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
                    parameters.Select(n => new OperationParameter(n.Name, n.ParameterType))
                );
            this.operationResult = new OperationResult(returnType);

            this.operationParameters.All
                (
                    info =>
                    {
                        DynamicTypeRegister.LoadTypes(info.OrginalType.Assembly);
                        return true;
                    }
                );

            DynamicTypeRegister.LoadTypes(this.operationResult.OrginalType.Assembly);
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
