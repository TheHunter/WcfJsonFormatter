using System.ServiceModel.Dispatcher;

namespace WcfJsonFormatter.Formatters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IClientJsonMessageFormatter
        : IClientMessageFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        byte[] EncodeParameters(object[] parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object DecodeReply(byte[] body, object[] parameters);
    }
}
