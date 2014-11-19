using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using WcfJsonFormatter.Configuration;
using Xunit;

namespace WcfJsonFormatter.Test
{

    public class SerializerSettingsTest
    {
        [Fact]
        public void LoadConfigSection()
        {
            var settings = ConfigurationManager.GetSection("serviceTypeRegister") as ServiceTypeRegister;
            Assert.NotNull(settings);
        }
    }
}
