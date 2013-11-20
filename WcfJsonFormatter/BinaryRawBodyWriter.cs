using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace WcfJsonFormatter
{
    /// <summary>
    /// 
    /// </summary>
    public class BinaryRawBodyWriter
        : BodyWriter
    {
        private readonly byte[] content;
        private readonly string headerElement;
        private const string RootNaming = "Binary";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        public BinaryRawBodyWriter(byte[] content)
            : this(content, RootNaming)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="headerElement"></param>
        public BinaryRawBodyWriter(byte[] content, string headerElement)
            : base(true)
        {
            if (headerElement == null || headerElement.Trim().Equals(string.Empty))
                throw new ArgumentException("The header element of RawBodyWriter cannot be empty or null.", "headerElement");

            if (content == null)
                throw new ArgumentNullException("content", "The body content cannot be null.");

            this.content = content;
            this.headerElement = headerElement;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement(headerElement);
            writer.WriteBase64(content, 0, content.Length);
            writer.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        public static string DefaultRootName
        {
            get
            {
                return RootNaming;
            }
        }
    }
}
