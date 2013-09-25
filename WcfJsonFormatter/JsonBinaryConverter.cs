using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace WcfJsonFormatter
{
    /// <summary>
    /// 
    /// </summary>
    public static class JsonBinaryConverter
    {
        private static readonly OperationTypeBinder Binder;
        private static readonly JsonSerializer Serializer;

        static JsonBinaryConverter()
        {
            Binder = new OperationTypeBinder();
            CustomContractResolver resolver = new CustomContractResolver(true, false)
                {
                    DefaultMembersSearchFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
                };

            Serializer = new JsonSerializer
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Objects,
                ContractResolver = resolver,
                Binder = Binder
            };
        }

        #region Wcf client usage

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodParameters"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal static byte[] SerializeRequest(IEnumerable<OperationParameter> methodParameters, object[] parameters)
        {
            byte[] body;
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
                {
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        int index = -1;
                        writer.WriteStartObject();
                        foreach (var parameter in methodParameters)
                        {
                            object paramValue = parameters[++index];
                            writer.WritePropertyName(parameter.Name);

                            if (paramValue == null)
                            {
                                Serializer.Serialize(writer, null);
                            }
                            else
                            {
                                JToken current = JToken.FromObject(paramValue, Serializer);
                                JTokenToSerialize(current);
                                Serializer.Serialize(writer, current);
                            }
                        }

                        writer.WriteEndObject();
                        writer.Flush();
                    }
                }
                body = ms.ToArray();
            }
            return body;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="returnType"></param>
        /// <returns></returns>
        internal static object DeserializeReply(byte[] body, OperationResult returnType)
        {
            using (MemoryStream ms = new MemoryStream(body))
            {
                using (StreamReader sr = new StreamReader(ms))
                {
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        JToken token = Serializer.Deserialize<JToken>(reader);
                        Type type = DynamicTypeRegister.GetTypeByShortName(JTokenToDeserialize(token))
                                    ?? returnType.NormalizedType;

                        object ret = token.ToObject(type, Serializer);
                        return ret;
                    }
                }
            }
        }

        #endregion

        #region Wcf service usage

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="methodParameters"></param>
        /// <param name="parameters"></param>
        internal static void DeserializeRequest(byte[] body, IEnumerable<OperationParameter> methodParameters, object[] parameters)
        {
            using (MemoryStream ms = new MemoryStream(body))
            {
                using (StreamReader sr = new StreamReader(ms))
                {
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        JObject wrappedParameters = Serializer.Deserialize<JObject>(reader);
                        int indexParam = -1;

                        foreach (var parameter in methodParameters)
                        {
                            JProperty property = wrappedParameters.Property(parameter.Name);
                            if (property != null)
                            {
                                Type type = DynamicTypeRegister.GetTypeByShortName(JTokenToDeserialize(property.Value))
                                            ?? parameter.NormalizedType;

                                // NOTA: se l'oggetto type non fosse nullo, viene sempre richiamato il binder
                                // in presenza della proprietà $id ??

                                parameters[++indexParam] = property.Value.ToObject(type, Serializer);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnType"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal static byte[] SerializeReply(OperationResult returnType, object result)
        {
            byte[] body;
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
                {
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        if (result == null)
                        {
                            Serializer.Serialize(writer, null);
                        }
                        else
                        {
                            JToken token = JToken.FromObject(result, Serializer);
                            JTokenToSerialize(token);
                            Serializer.Serialize(writer, token);
                        }
                        writer.Flush();
                    }
                }
                body = ms.ToArray();
            }
            return body;
        }

        #endregion

        #region common methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        private static void JTokenToSerialize(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Array:
                    {
                        JTokenToSerialize(token as JArray);
                        break;
                    }
                case JTokenType.Property:
                    {
                        JTokenToSerialize(token as JProperty);
                        break;
                    }
                case JTokenType.Object:
                    {
                        JTokenToSerialize(token as JObject);
                        break;
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        private static void JTokenToSerialize(JProperty token)
        {
            if (token.Name == "$type")
                token.Value = NormalizeTypeName(token.Value.ToString());
            else
                JTokenToSerialize(token.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        private static void JTokenToSerialize(JArray token)
        {
            foreach (var current in token)
            {
                JTokenToSerialize(current);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        private static void JTokenToSerialize(JObject token)
        {
            if (token != null)
            {
                foreach (var property in token.Properties())
                {
                    JTokenToSerialize(property);
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private static string NormalizeTypeName(string typeName)
        {
            if (typeName == null)
                return null;

            typeName = typeName.Trim();

            int index = typeName.IndexOf(',');
            if (index > -1)
                typeName = typeName.Substring(0, index).Trim();

            return typeName.Substring(typeName.LastIndexOf('.') + 1).Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static string JTokenToDeserialize(JToken token)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static string JTokenToDeserialize(JObject token)
        {
            if (token == null)
                return null;

            JProperty typeProperty = token.Property("$type");
            if (typeProperty == null)
                return null;

            string typeName = typeProperty.Value.ToString();
            token.Remove(typeProperty.Name);
            return typeName;
        }

        /// <summary>
        /// Returns true if the given object type is an array or any kind of collection which implements
        /// IEnumerable interface.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsJArrayType(Type type)
        {
            if (type == null)
                return false;

            if (type.IsArray)
                return true;

            Type collectionType = type.GetInterface("IEnumerable", true)
                                  ?? type.GetInterface("IEnumerable`1", true);

            return collectionType != null;

        }
        #endregion
    }
}
