using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Autofac;
using Autofac.Integration.Wcf;
using NHibernate;
using PersistentLayer.Domain;
using PersistentLayer.NHibernate;
using PersistentLayer.NHibernate.Impl;
using WcfJsonFormatter;

namespace WcfJsonService.Example
{
    public class WcfHost
    {
        static void Main()
        {
            WcfHost host = new WcfHost();
            
            host.Initialize();
            
            //host.Run();
            host.RunFromCfg();
        }

        private void Initialize()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<SalesService>();

            ISessionFactory current = WcfServiceHolder.DefaultSessionFactory;

            builder.Register(n => new EnterprisePagedDAO(new SessionManager(current)))
                .As<INhPagedDAO>()
                .SingleInstance();

            builder.Register(n => new SalesService(n.Resolve<INhPagedDAO>()))
                .As<ISalesService>();

            AutofacHostFactory.Container = builder.Build();

        }

        private void Run()
        {
            Console.WriteLine("Run a ServiceHost via programmatic configuration...");
            Console.WriteLine();

            string baseAddress = "http://" + Environment.MachineName + ":8000/Service.svc";

            using (ServiceHost serviceHost = new ServiceHost(typeof(SalesService), new Uri(baseAddress)))
            {
                WebHttpBinding webBinding = new WebHttpBinding
                {
                    ContentTypeMapper = new RawContentMapper(),
                    MaxReceivedMessageSize = 4194304,
                    MaxBufferSize = 4194304
                };

                serviceHost.AddServiceEndpoint(typeof(ISalesService), webBinding, "json")
                    .Behaviors.Add(new WebHttpJsonBehavior());

                serviceHost.AddServiceEndpoint(typeof(ISalesService), new BasicHttpBinding(), baseAddress);

                serviceHost.AddDependencyInjectionBehavior<ISalesService>(AutofacHostFactory.Container);

                Console.WriteLine("Opening the host");
                serviceHost.Open();

                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();

                serviceHost.Close();
            }
        }

        private void RunFromCfg()
        {
            Console.WriteLine("Run a ServiceHost via administrative configuration...");
            Console.WriteLine();

            using (ServiceHost serviceHost = new ServiceHost(typeof(SalesService)))
            {
                serviceHost.AddDependencyInjectionBehavior<ISalesService>(AutofacHostFactory.Container);

                Console.WriteLine("Opening the host");
                serviceHost.Open();

                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();

                serviceHost.Close();
            }
        }
    }
}
