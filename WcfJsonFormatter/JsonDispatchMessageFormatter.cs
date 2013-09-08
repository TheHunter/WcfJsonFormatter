using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;
using Newtonsoft.Json;

namespace WcfJsonFormatter
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonDispatchMessageFormatter
        : MessageFormatter, IDispatchMessageFormatter
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        public JsonDispatchMessageFormatter(OperationDescription operation)
            : base (operation.Messages[1].Action,
                    operation.SyncMethod.GetParameters(),
                    operation.SyncMethod.ReturnType)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="parameters"></param>
        public void DeserializeRequest(Message message, object[] parameters)
        {
            object bodyFormatProperty;
            if (!message.Properties.TryGetValue(WebBodyFormatMessageProperty.Name, out bodyFormatProperty))
                throw new InvalidOperationException("Incoming message cannot be null.");

            WebBodyFormatMessageProperty bodyMsg = bodyFormatProperty as WebBodyFormatMessageProperty;

            if (bodyMsg == null)
                throw new InvalidCastException("The type of body message must be WebBodyFormatMessageProperty.");

            if (bodyMsg.Format != WebContentFormat.Raw)
                throw new InvalidOperationException("The body message type must be equals to WebContentFormat.Raw.");

            ////
            XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
            bodyReader.ReadStartElement(RawBodyWriter.DefaultRootName);

            JsonBinaryConverter.DeserializeRequest(bodyReader.ReadContentAsBase64(), this.OperationParameters, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageVersion"></param>
        /// <param name="parameters"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            byte[] body = JsonBinaryConverter.SerializeReply(this.OperationResult, result);

            Message replyMessage = Message.CreateMessage(messageVersion, this.Action, new RawBodyWriter(body));
            replyMessage.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));
            HttpResponseMessageProperty respProp = new HttpResponseMessageProperty();
            respProp.Headers[HttpResponseHeader.ContentType] = "application/json";
            replyMessage.Properties.Add(HttpResponseMessageProperty.Name, respProp);

            return replyMessage;
        }
    }
}
