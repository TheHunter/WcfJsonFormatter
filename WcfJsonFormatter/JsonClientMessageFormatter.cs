using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace WcfJsonFormatter
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonClientMessageFormatter
        : MessageFormatter, IClientMessageFormatter
    {
        private readonly Uri operationUri;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="endpoint"></param>
        public JsonClientMessageFormatter(OperationDescription operation, ServiceEndpoint endpoint)
            : base (operation.Messages[0].Action,
                    operation.SyncMethod.GetParameters(),
                    operation.SyncMethod.ReturnType)
        {
            string endpointAddress = endpoint.Address.Uri.ToString();
            if (!endpointAddress.EndsWith("/"))
                endpointAddress = endpointAddress + "/";

            this.operationUri = new Uri(endpointAddress + operation.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageVersion"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            byte[] body = JsonBinaryConverter.SerializeRequest(this.OperationParameters, parameters);
            
            Message requestMessage = Message.CreateMessage(messageVersion, this.Action, new RawBodyWriter(body));
            requestMessage.Headers.To = operationUri;
            requestMessage.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));
            HttpRequestMessageProperty reqProp = new HttpRequestMessageProperty();
            reqProp.Headers[HttpRequestHeader.ContentType] = "application/json";
            requestMessage.Properties.Add(HttpRequestMessageProperty.Name, reqProp);
            return requestMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
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

            bodyReader.ReadStartElement(RawBodyWriter.DefaultRootName);
            return JsonBinaryConverter.DeserializeReply(bodyReader.ReadContentAsBase64(), this.OperationResult);

        }
    }
}
