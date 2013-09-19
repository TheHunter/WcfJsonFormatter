using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;

namespace WcfJsonFormatter
{
    public class WebHttpJsonBehaviorElement
        : BehaviorExtensionElement
    {

        public override Type BehaviorType
        {
            get { return typeof(WebHttpJsonBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new WebHttpJsonBehavior();
        }
    }
}
