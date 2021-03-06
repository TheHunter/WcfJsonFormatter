﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;

namespace WcfJsonFormatter.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class SerializerSettings
        : ConfigServiceElement
    {
        private bool onlyPublicConstructor;
        private bool enablePolymorphicMembers;

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("onlyPublicConstructor", IsRequired = false, DefaultValue = "0")]
        private string onlyPublicConstructorCfg
        {
            get { return this["onlyPublicConstructor"] as string; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool OnlyPublicConstructor
        {
            get { return this.onlyPublicConstructor; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("enablePolymorphicMembers", IsRequired = false, DefaultValue = "0")]
        private string enablePolymorphicMembersCfg
        {
            get { return this["enablePolymorphicMembers"] as string; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool EnablePolymorphicMembers
        {
            get { return this.enablePolymorphicMembers; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("dateFormatHandling", IsRequired = false, DefaultValue = DateFormatStyle.IsoDateFormat)]
        public DateFormatStyle DateFormatHandling
        {
            get { return (DateFormatStyle)this["dateFormatHandling"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("dateFormatString", IsRequired = false, DefaultValue = null)]
        public string DateFormatString
        {
            get { return this["dateFormatString"] as string; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("dateParseHandling", IsRequired = false, DefaultValue = DateParseStyle.DateTime)]
        public DateParseStyle DateParseHandling
        {
            get { return (DateParseStyle)this["dateParseHandling"]; }
        }

        /// <inheritdoc/>
        public override object Key
        {
            get { return GetHashCode(); }
        }

        /// <inheritdoc/>
        protected override void PostDeserialize()
        {
            base.PostDeserialize();

            var onlyPublicCtr = this.onlyPublicConstructorCfg;
            var enablePolyMembers = this.enablePolymorphicMembersCfg;

            this.onlyPublicConstructor = (onlyPublicCtr != null) && (onlyPublicCtr.Equals("true") || onlyPublicCtr.Equals("1"));
            this.enablePolymorphicMembers = (enablePolyMembers != null) && (enablePolyMembers.Equals("true") || enablePolyMembers.Equals("1"));
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.OnlyPublicConstructor.GetHashCode() - this.EnablePolymorphicMembers.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("onlyPublicConstructor: {0}, enablePolymorphicMembers: {1}", this.OnlyPublicConstructor, this.EnablePolymorphicMembers);
        }
    }
}
