using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Administration;
using System.ServiceModel.Description;
using System.Text;
using WcfJsonFormatter.Formatters;

namespace WcfJsonFormatter
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWebHttpJsonBehavior
         : IEndpointBehavior
    {
        /// <summary>
        /// 
        /// </summary>
        IServiceRegister ConfigRegister { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationDescription"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        IDispatchJsonMessageFormatter MakeDispatchMessageFormatter(OperationDescription operationDescription,
                                                                   ServiceEndpoint endpoint);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationDescription"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        IClientJsonMessageFormatter MakeClientMessageFormatter(OperationDescription operationDescription,
                                                               ServiceEndpoint endpoint);
    }
}
