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

            Assert.True(settings.SerializerConfig.EnablePolymorphicMembers);
            Assert.True(settings.SerializerConfig.OnlyPublicConstructor);
        }

        [Fact]
        public void ConvertPrimitiveType()
        {
            object a = "true";
            var res1 = Convert.ToBoolean(a);

            object b = "1";
            var res2 = Convert.ToBoolean(b);
        }
    }
}
