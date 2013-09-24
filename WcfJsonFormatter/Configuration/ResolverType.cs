using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace WcfJsonFormatter.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class ResolverType
        : ConfigServiceElement
    {
        private bool wasResolved;

        [ConfigurationProperty("serviceType", IsRequired = true)]
        public ServiceType ServiceType
        {
            get { return (ServiceType)this["serviceType"]; }
            set { this["serviceType"] = value; }
        }

        [ConfigurationProperty("binderType", IsRequired = true)]
        public ServiceType BinderType
        {
            get { return (ServiceType)this["binderType"]; }
            set { this["binderType"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PostDeserialize()
        {
            base.PostDeserialize();
            if (this.ServiceType.Name == "*")
                throw new ArgumentException("The ServiceType property of ResolverType must contain a valid CLR type name to be resolved by binder type associated.");

            if (this.BinderType.Name == "*")
                throw new ArgumentException("The BinderType property of ResolverType must contain a valid CLR type name to resolve the service type associated.");

            this.wasResolved = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public override object Key
        {
            get { return this.ServiceType; }
        }


        public bool WasResolved
        {
            get { return this.wasResolved; }
            internal set { this.wasResolved = value; }
        }


        public override string ToString()
        {
            return string.Format("serviceType: {0}, binderType: {1}", this.ServiceType, this.BinderType);
        }
    }
}
