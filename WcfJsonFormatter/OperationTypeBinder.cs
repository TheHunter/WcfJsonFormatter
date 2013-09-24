﻿using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace WcfJsonFormatter
{
    /// <summary>
    /// 
    /// </summary>
    public class OperationTypeBinder
        : SerializationBinder, ISerializationBinder
        //: SerializationBinder, Newtonsoft.Json.Serialization.ISerializationBinder
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public override Type BindToType(string assemblyName, string typeName)
        {
            return DynamicTypeRegister.GetTypeByShortName(typeName);
        }


#if (BEFORE_NET40)
        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
#else
        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
#endif
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ISerializationBinder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        Type BindToType(string assemblyName, string typeName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializedType"></param>
        /// <param name="assemblyName"></param>
        /// <param name="typeName"></param>
        void BindToName(Type serializedType, out string assemblyName, out string typeName);
    }
}
