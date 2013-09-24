using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    public class ServiceTypeCollection<TElement>
        : ConfigurationElementCollection
        where TElement : ConfigServiceElement, new()
    {

        private readonly string propertyName;

        /// <summary>
        /// 
        /// </summary>
        public ServiceTypeCollection()
        {
            string naming = (typeof(TElement).Name);
            this.propertyName = string.Format("{0}{1}", naming.Substring(0, 1).ToLower(), naming.Substring(1));
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string ElementName
        {
            get
            {
                return this.propertyName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        protected override bool IsElementName(string elementName)
        {
            return elementName.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsReadOnly()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="custom"></param>
        public void Add(TElement custom)
        {
            BaseAdd(custom);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        /// <summary>
        /// 
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new TElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TElement)element).Key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TElement this[int index]
        {
            get
            {
                return (TElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public int IndexOf(TElement element)
        {
            return BaseIndexOf(element);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void Remove(TElement url)
        {
            if (BaseIndexOf(url) >= 0)
                BaseRemove(url.Key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="name"></param>
        //public void Remove(string name)
        //{
        //    BaseRemove(name);
        //}

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }
    }
}
