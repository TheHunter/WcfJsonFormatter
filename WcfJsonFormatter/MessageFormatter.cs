﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
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
        private readonly IServiceRegister serviceRegister;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="parameters"></param>
        /// <param name="returnType"></param>
        protected MessageFormatter(string action, IEnumerable<ParameterInfo> parameters, Type returnType, IServiceRegister serviceRegister)
        {
            try
            {
                this.serviceRegister = serviceRegister;

                this.action = action;
                this.operationParameters = new List<OperationParameter>
                    (
                        parameters.Select(n => new OperationParameter(n.Name, action, n.ParameterType, serviceRegister))
                    );

                this.operationResult = new OperationResult(action, returnType, serviceRegister);
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("The service operation cannot be invoked because It uses an invalid object type, see innerException for details.", action, ex);
            }
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
