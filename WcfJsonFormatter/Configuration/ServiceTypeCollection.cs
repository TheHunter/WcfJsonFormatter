using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter.Configuration
{
    public class ServiceTypeCollection
        : ConfigurationElementCollection
    {

        public void Add(ServiceType custom)
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
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceType();
        }

        
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceType)element).Name;
        }


        public ServiceType this[int Index]
        {
            get
            {
                return (ServiceType)BaseGet(Index);
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


        new public ServiceType this[string Name]
        {
            get
            {
                return (ServiceType)BaseGet(Name);
            }
        }


        public int indexof(ServiceType element)
        {
            return BaseIndexOf(element);
        }


        public void Remove(ServiceType url)
        {
            if (BaseIndexOf(url) >= 0)
                BaseRemove(url.Name);
        }


        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }


        public void Remove(string name)
        {
            BaseRemove(name);
        }


        public void Clear()
        {
            BaseClear();
        }
    }
}
