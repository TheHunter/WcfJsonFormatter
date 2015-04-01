﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Description;
using System.Text;
using WcfJsonFormatter.Exceptions;

namespace WcfJsonFormatter
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceOperation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="action"></param>
        public ServiceOperation(OperationDescription operation, string action)
        {
            if (action == null || action.Trim().Equals(string.Empty))
                throw new ArgumentException("The given action related to the given operation cannot be null or empty.", "action");

            if (operation == null)
                throw new ArgumentNullException("operation", "the given operation description cannot be null.");

            if (operation.SyncMethod == null)
                throw new NullReferenceException("The SynMethod property on operation cannot be null.");
            
            this.Action = action.Trim();
            this.Parameters = operation.SyncMethod.GetParameters();
            this.ReturnType = operation.SyncMethod.ReturnType;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Action { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ParameterInfo[] Parameters { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Type ReturnType { get; private set; }
    }
}
