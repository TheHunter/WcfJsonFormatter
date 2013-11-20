using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter.Exceptions
{
    public class NullMessageFormatterException
        : ConfigServiceException
    {
        public NullMessageFormatterException(string message)
            : base(message)
        {
        }

        public NullMessageFormatterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
