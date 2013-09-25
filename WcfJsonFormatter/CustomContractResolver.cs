using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace WcfJsonFormatter
{
    /// <summary>
    /// 
    /// </summary>
    internal class CustomContractResolver
        : DefaultContractResolver
    {
        private readonly bool includeFields;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shareCache"></param>
        /// <param name="includeFields"></param>
        public CustomContractResolver(bool shareCache, bool includeFields)
            : base(shareCache)
        {
            this.includeFields = includeFields;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetPropertyMembers(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;
            return type.GetProperties(flags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            List<JsonProperty> properties = new List<JsonProperty>(base.CreateProperties(type, memberSerialization));
            if (!includeFields)
            {
                IEnumerable<string> propertyMembers = CustomContractResolver.GetPropertyMembers(type).Select(n => n.Name);
                properties.RemoveAll(n => !propertyMembers.Contains(n.PropertyName));
                
                foreach (var property in properties)
                {
                    Type normalized = DynamicTypeRegister.NormalizeType(property.PropertyType);
                    if (normalized != null && normalized != property.PropertyType)
                        property.MemberConverter = new JsonReaderConverter(normalized);
                }
            }

            return properties;
        }
    }
}
