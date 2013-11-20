using System.Collections.Generic;

namespace WcfJsonFormatter.Formatters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMessageFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        string Action { get; }

        /// <summary>
        /// 
        /// </summary>
        OperationResult OperationResult { get; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<OperationParameter> OperationParameters { get; }

        /// <summary>
        /// 
        /// </summary>
        IServiceRegister ServiceRegister { get; }

    }
}
