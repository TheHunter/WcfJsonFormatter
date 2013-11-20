using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using NHibernate;
using NHibernate.Context;
using PersistentLayer.Domain;
using PersistentLayer.NHibernate;

namespace WcfJsonService.Example
{
    /// <summary>
    /// 
    /// </summary>
    public static class WcfServiceHolder
    {
        private static readonly ISessionFactory Sessionfactory;
        private static readonly IEnumerable<Type> KnownTypes;

        static WcfServiceHolder()
        {
            string rootPath = GetRootPathProject();

            XmlTextReader configReader = new XmlTextReader(File.OpenRead(string.Format("{0}/cfg/Configuration.xml", rootPath)));
            DirectoryInfo dir = new DirectoryInfo(string.Format("{0}/.hbm", rootPath));
            NhConfigurationBuilder bld = new NhConfigurationBuilder(configReader, dir);

            bld.SetProperty("connection.connection_string", GetConnectionString(rootPath));
            bld.BuildSessionFactory();

            Sessionfactory = bld.SessionFactory;

            KnownTypes = Assembly.GetAssembly(typeof(Salesman)).GetTypes();
        }

        static string GetRootPathProject()
        {
            var list = new List<string>(Directory.GetCurrentDirectory().Split('\\'));
            list.RemoveAt(list.Count - 1);
            list.RemoveAt(list.Count - 1);
            list.Add(string.Empty);
            return string.Join("\\", list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootPath"></param>
        /// <returns></returns>
        private static string GetConnectionString(string rootPath)
        {
            string output = rootPath + "db\\";

            var str = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
            return string.Format(str, output);
        }

        /// <summary>
        /// 
        /// </summary>
        public static ISessionFactory DefaultSessionFactory
        {
            get
            {
                return Sessionfactory;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void BindSession()
        {
            lock (Sessionfactory)
            {
                ISession session = Sessionfactory.OpenSession();
                CurrentSessionContext.Bind(session);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void UnbindSession()
        {
            lock (Sessionfactory)
            {
                ISession session = CurrentSessionContext.Unbind(Sessionfactory);
                if (session != null && session.IsOpen)
                    session.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<Type> CustomKnownTypes
        {
            get { return KnownTypes; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
        {
            return KnownTypes;
        }

    }
}
