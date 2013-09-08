using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;

namespace WcfJsonFormatter
{
    /// <summary>
    /// Specifies the format to which the content type of an incoming message is mapped.
    /// </summary>
    public class RawContentMapper
        : WebContentTypeMapper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public override WebContentFormat GetMessageFormatForContentType(string contentType)
        {
            return WebContentFormat.Raw;
        }
    }
}
