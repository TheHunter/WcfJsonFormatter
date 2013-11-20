using System.ServiceModel.Dispatcher;

namespace WcfJsonFormatter.Formatters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDispatchJsonMessageFormatter
        : IDispatchMessageFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="parameters"></param>
        void DecodeParameters(byte[] body, object[] parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        byte[] EncodeReply(object[] parameters, object result);
    }
}
