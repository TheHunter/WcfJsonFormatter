using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Xml;

namespace WcfJsonFormatter.Formatters
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DispatchJsonMessageFormatter
        : MessageFormatter, IDispatchJsonMessageFormatter
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="serviceRegister"></param>
        protected DispatchJsonMessageFormatter(OperationDescription operation, IServiceRegister serviceRegister)
            : base (operation.Messages[1].Action,
                    operation.SyncMethod.GetParameters(),
                    operation.SyncMethod.ReturnType,
                    serviceRegister)
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
            bodyReader.ReadStartElement(BinaryRawBodyWriter.DefaultRootName);

            this.DecodeParameters(bodyReader.ReadContentAsBase64(), parameters);
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
            byte[] body = this.EncodeReply(parameters, result);
            Message replyMessage = Message.CreateMessage(messageVersion, this.Action, new BinaryRawBodyWriter(body));

            replyMessage.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));
            HttpResponseMessageProperty respProp = new HttpResponseMessageProperty();
            respProp.Headers[HttpResponseHeader.ContentType] = "application/json";
            replyMessage.Properties.Add(HttpResponseMessageProperty.Name, respProp);

            return replyMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="parameters"></param>
        public abstract void DecodeParameters(byte[] body, object[] parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public abstract byte[] EncodeReply(object[] parameters, object result);
    }
}
