using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Text;
using WcfJsonFormatter.Configuration;

namespace WcfJsonFormatter
{
    /// <summary>
    /// A register of object types which registers all known types used by operations services.
    /// </summary>
    public static class DynamicTypeRegister
    {
        private static readonly ServiceTypeRegister ConfigRegister;

        /// <summary>
        /// 
        /// </summary>
        static DynamicTypeRegister()
        {
            ConfigRegister = ConfigurationManager.GetSection("serviceTypeRegister") as ServiceTypeRegister
                            ?? new ServiceTypeRegister();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static Type NormalizeType(Type arg)
        {
            if (arg == null)
                return null;

            Type supplier;
            lock (ConfigRegister)
            {
                supplier = ConfigRegister.TryToNormalize(arg);
            }
            return supplier;
        }

        /// <summary>
        /// Registers the given object type if there's no registered.
        /// </summary>
        /// <param name="current"></param>
        public static void LoadType(Type current)
        {
            if (current == null)
                return;

            lock (ConfigRegister)
            {
                ConfigRegister.LoadType(current);
            }
        }

        /// <summary>
        /// Registers all types from the given collection.
        /// </summary>
        /// <param name="types"></param>
        public static void LoadTypes(IEnumerable<Type> types)
        {
            if (types == null)
                return;

            lock (ConfigRegister)
            {
                ConfigRegister.LoadTypes(types);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        public static void LoadTypes(Assembly assembly)
        {
            if (assembly == null)
                return;

            lock (ConfigRegister)
            {
                ConfigRegister.LoadTypes(assembly);
            }
        }

        /// <summary>
        /// Gets an object type from the given name.
        /// </summary>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public static Type GetTypeByShortName(string shortName)
        {
            if (string.IsNullOrEmpty(shortName))
                return null;

            lock (ConfigRegister)
            {
                return ConfigRegister.KnownTypes.FirstOrDefault(n => n.Name.Equals(shortName));
            }
        }

        /// <summary>
        /// Gets the object type from the given fullname.
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static Type GetTypeByFullName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            lock (ConfigRegister)
            {
                return ConfigRegister.KnownTypes.FirstOrDefault(n => n.FullName.Equals(fullName));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void RefreshServiceRegister()
        {
            ConfigRegister.RefreshRegister();
        }
    }
}
