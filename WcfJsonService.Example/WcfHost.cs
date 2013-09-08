using System;
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
            host.Run();
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
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service.svc";

            using (ServiceHost serviceHost = new ServiceHost(typeof(SalesService), new Uri(baseAddress)))
            {
                WebHttpBinding webBinding = new WebHttpBinding
                    {
                        ContentTypeMapper = new RawContentMapper(),
                        MaxReceivedMessageSize = 4194304,
                        MaxBufferSize = 4194304
                    };

                //WSHttpBinding wsHttpBinding = new WSHttpBinding();

                serviceHost.AddServiceEndpoint(typeof(ISalesService), new BasicHttpBinding(), baseAddress);

                serviceHost.AddServiceEndpoint(typeof(ISalesService), webBinding, "json")
                    .Behaviors.Add(new WebHttpJsonBehavior());
                
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
