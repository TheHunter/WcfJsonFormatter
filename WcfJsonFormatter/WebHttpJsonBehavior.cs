using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;

namespace WcfJsonFormatter
{
    /// <summary>
    /// 
    /// </summary>
    public class WebHttpJsonBehavior
        : WebHttpBehavior
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpoint"></param>
        public override void Validate(ServiceEndpoint endpoint)
        {
            base.Validate(endpoint);

            BindingElementCollection elements = endpoint.Binding.CreateBindingElements();
            WebMessageEncodingBindingElement webEncoder = elements.Find<WebMessageEncodingBindingElement>();
            if (webEncoder == null)
                throw new InvalidOperationException("This behavior must be used in an endpoint with the WebHttpBinding (or a custom binding with the WebMessageEncodingBindingElement).");

            foreach (OperationDescription operation in endpoint.Contract.Operations)
            {
                this.ValidateOperation(operation);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationDescription"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        protected override IDispatchMessageFormatter GetRequestDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            if (this.IsGetOperation(operationDescription))
                // no change for GET operations
                return base.GetRequestDispatchFormatter(operationDescription, endpoint);

            if (operationDescription.Messages[0].Body.Parts.Count == 0)
                // nothing in the body, still use the default
                return base.GetRequestDispatchFormatter(operationDescription, endpoint);

            return new JsonDispatchMessageFormatter(operationDescription);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationDescription"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        protected override IDispatchMessageFormatter GetReplyDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            if (operationDescription.Messages.Count == 1 ||
                operationDescription.Messages[1].Body.ReturnValue.Type == typeof(void))
                return base.GetReplyDispatchFormatter(operationDescription, endpoint);

            return new JsonDispatchMessageFormatter(operationDescription);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationDescription"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        protected override IClientMessageFormatter GetRequestClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            if (operationDescription.Behaviors.Find<WebGetAttribute>() != null)
                // no change for GET operations
                return base.GetRequestClientFormatter(operationDescription, endpoint);

            WebInvokeAttribute wia = operationDescription.Behaviors.Find<WebInvokeAttribute>();
            if (wia != null && wia.Method == "HEAD")
                return base.GetRequestClientFormatter(operationDescription, endpoint);

            if (operationDescription.Messages[0].Body.Parts.Count == 0)
                // nothing in the body, still use the default
                return base.GetRequestClientFormatter(operationDescription, endpoint);

            return new JsonClientMessageFormatter(operationDescription, endpoint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationDescription"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        protected override IClientMessageFormatter GetReplyClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            if (operationDescription.Messages.Count == 1 ||
                operationDescription.Messages[1].Body.ReturnValue.Type == typeof(void))
                return base.GetReplyClientFormatter(operationDescription, endpoint);

            return new JsonClientMessageFormatter(operationDescription, endpoint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        private void ValidateOperation(OperationDescription operation)
        {
            if (operation.Messages.Count > 1 && operation.Messages[1].Body.Parts.Count > 0)
                throw new InvalidOperationException("Operations cannot have out/ref parameters.");

            string uriTemplate = this.GetUriTemplate(operation);
            if (uriTemplate != null)
                throw new InvalidOperationException("UriTemplate support not implemented in this behavior.");

            WebMessageBodyStyle bodyStyle = this.GetBodyStyle(operation);

            bool wrappedResponse = bodyStyle == WebMessageBodyStyle.Wrapped
                                   || bodyStyle == WebMessageBodyStyle.WrappedResponse;

            bool isVoidReturn = operation.Messages.Count == 1 || operation.Messages[1].Body.ReturnValue.Type == typeof(void);
            if (!isVoidReturn && wrappedResponse)
                throw new InvalidOperationException("Wrapped response not implemented in this behavior.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        private string GetUriTemplate(OperationDescription operation)
        {
            WebGetAttribute wga = operation.Behaviors.Find<WebGetAttribute>();
            if (wga != null)
                return wga.UriTemplate;

            WebInvokeAttribute wia = operation.Behaviors.Find<WebInvokeAttribute>();
            if (wia != null)
                return wia.UriTemplate;

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        private WebMessageBodyStyle GetBodyStyle(OperationDescription operation)
        {
            WebGetAttribute wga = operation.Behaviors.Find<WebGetAttribute>();
            if (wga != null)
                return wga.BodyStyle;

            WebInvokeAttribute wia = operation.Behaviors.Find<WebInvokeAttribute>();
            if (wia != null)
                return wia.BodyStyle;

            return this.DefaultBodyStyle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        private bool IsGetOperation(OperationDescription operation)
        {
            WebGetAttribute wga = operation.Behaviors.Find<WebGetAttribute>();
            if (wga != null)
                return true;

            WebInvokeAttribute wia = operation.Behaviors.Find<WebInvokeAttribute>();
            if (wia != null)
                return wia.Method == "HEAD";

            return false;
        }
    }
}
