using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using WcfJsonFormatter.Exceptions;
using WcfJsonFormatter.Formatters;

namespace WcfJsonFormatter
{
    /// <summary>
    /// 
    /// </summary>
    public class WebHttpJsonResolverBehavior
        : WebHttpJsonBehavior
    {
        private readonly Func<OperationDescription, ServiceEndpoint, IDispatchJsonMessageFormatter> dispatchFormatter;
        private readonly Func<OperationDescription, ServiceEndpoint, IClientJsonMessageFormatter> clientFormatter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispatchFormatter"></param>
        /// <param name="clientFormatter"></param>
        public WebHttpJsonResolverBehavior(Func<OperationDescription, ServiceEndpoint, IDispatchJsonMessageFormatter> dispatchFormatter,
                                           Func<OperationDescription, ServiceEndpoint, IClientJsonMessageFormatter> clientFormatter )
            : this(new List<Type>(), dispatchFormatter, clientFormatter)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="knownTypes"></param>
        /// <param name="dispatchFormatter"></param>
        /// <param name="clientFormatter"></param>
        public WebHttpJsonResolverBehavior(IEnumerable<Type> knownTypes,
                                           Func<OperationDescription, ServiceEndpoint, IDispatchJsonMessageFormatter> dispatchFormatter,
                                           Func<OperationDescription, ServiceEndpoint, IClientJsonMessageFormatter> clientFormatter)
            : base(knownTypes)
        {
            if (dispatchFormatter == null)
                throw new ConfigServiceException("The dispatchFormatter function used by WebHttpJsonResolverBehavior instance cannot be null");

            if (clientFormatter == null)
                throw new ConfigServiceException("The clientFormatter function used by WebHttpJsonResolverBehavior instance cannot be null");

            this.dispatchFormatter = dispatchFormatter;
            this.clientFormatter = clientFormatter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationDescription"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public override IDispatchJsonMessageFormatter MakeDispatchMessageFormatter(OperationDescription operationDescription,
                                                                                   ServiceEndpoint endpoint)
        {
            return this.dispatchFormatter.Invoke(operationDescription, endpoint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationDescription"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public override IClientJsonMessageFormatter MakeClientMessageFormatter(OperationDescription operationDescription,
                                                                               ServiceEndpoint endpoint)
        {
            return this.clientFormatter.Invoke(operationDescription, endpoint);
        }
    }
}
