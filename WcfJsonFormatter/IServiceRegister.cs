using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WcfJsonFormatter.Configuration;

namespace WcfJsonFormatter
{
    /// <summary>
    /// 
    /// </summary>
    public interface IServiceRegister
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Type TryToNormalize(Type type);

        /// <summary>
        /// 
        /// </summary>
        bool CheckOperationTypes { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isFullname"></param>
        /// <returns></returns>
        Type GetTypeByName(string name, bool isFullname);
    }
}
