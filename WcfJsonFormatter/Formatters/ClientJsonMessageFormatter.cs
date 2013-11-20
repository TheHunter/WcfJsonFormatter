using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;

namespace WcfJsonFormatter.Formatters
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ClientJsonMessageFormatter
        : MessageFormatter, IClientJsonMessageFormatter
    {
        private readonly Uri operationUri;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="endpoint"></param>
        /// <param name="serviceRegister"></param>
        protected ClientJsonMessageFormatter(OperationDescription operation, ServiceEndpoint endpoint, IServiceRegister serviceRegister)
            : base (operation.Messages[0].Action,
                    operation.SyncMethod.GetParameters(),
                    operation.SyncMethod.ReturnType,
                    serviceRegister)
        {
            string endpointAddress = endpoint.Address.Uri.ToString();
            if (!endpointAddress.EndsWith("/"))
                endpointAddress = endpointAddress + "/";

            this.operationUri = new Uri(endpointAddress + operation.Name);
        }


        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            byte[] body = this.EncodeParameters(parameters);
            Message requestMessage = Message.CreateMessage(messageVersion, this.Action, new BinaryRawBodyWriter(body));

            requestMessage.Headers.To = operationUri;
            requestMessage.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));
            HttpRequestMessageProperty reqProp = new HttpRequestMessageProperty();
            reqProp.Headers[HttpRequestHeader.ContentType] = "application/json";
            requestMessage.Properties.Add(HttpRequestMessageProperty.Name, reqProp);
            return requestMessage;
        }


        public object DeserializeReply(Message message, object[] parameters)
        {
            object bodyFormatProperty;
            if (!message.Properties.TryGetValue(WebBodyFormatMessageProperty.Name, out bodyFormatProperty))
                throw new InvalidOperationException("Incoming message cannot be null.");

            WebBodyFormatMessageProperty bodyMsg = bodyFormatProperty as WebBodyFormatMessageProperty;
            if (bodyMsg == null)
                throw new InvalidCastException("The type of body message must be WebBodyFormatMessageProperty.");

            if (bodyMsg.Format != WebContentFormat.Raw)
                throw new InvalidOperationException("The body message type must be equals to WebContentFormat.Raw.");

            XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();

            bodyReader.ReadStartElement(BinaryRawBodyWriter.DefaultRootName);
            return this.DecodeReply(bodyReader.ReadContentAsBase64(), parameters);
        }

        
        public abstract byte[] EncodeParameters(object[] parameters);


        public abstract object DecodeReply(byte[] body, object[] parameters);
    }
}
