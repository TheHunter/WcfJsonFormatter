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


        public ServiceTypeCollection()
        {
            string naming = (typeof(TElement).Name);
            this.propertyName = string.Format("{0}{1}", naming.Substring(0, 1).ToLower(), naming.Substring(1));
        }


        protected override string ElementName
        {
            get
            {
                return this.propertyName;
            }
        }

        protected override bool IsElementName(string elementName)
        {
            return elementName.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public void Add(TElement custom)
        {
            BaseAdd(custom);
        }


        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }


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
        /// <param name="Index"></param>
        /// <returns></returns>
        public TElement this[int Index]
        {
            get
            {
                return (TElement)BaseGet(Index);
            }
            set
            {
                if (BaseGet(Index) != null)
                {
                    BaseRemoveAt(Index);
                }
                BaseAdd(Index, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        new public TElement this[string Name]
        {
            get
            {
                return (TElement)BaseGet(Name);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public int indexof(TElement element)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            BaseRemove(name);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }
    }
}
