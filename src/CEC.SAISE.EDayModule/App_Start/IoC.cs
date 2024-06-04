using Amdaris.NHibernateProvider;
using Microsoft.Practices.Unity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.App_Start
{
    public sealed class IoC
    {
        private static readonly object LockObj = new object();

        private static IUnityContainer container;

        private static IoC instance = new IoC();

        private IoC()
        {

        }

        public static IUnityContainer Container
        {
            get { return UnityConfig.GetConfiguredContainer(); }

            set
            {
                lock (LockObj)
                {
                    container = UnityConfig.GetConfiguredContainer();
                }
            }
        }


        public static IoC Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (LockObj)
                    {
                        if (instance == null)
                        {
                            instance = new IoC();
                        }
                    }
                }

                return instance;
            }
        }

        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        public static T Resolve<T>(string key)
        {
            return Container.Resolve<T>(key);
        }

        public static object Resolve(Type type)
        {
            return Container.Resolve(type);
        }

        public static ISessionFactory GetSessionFactory()
        {
            var sessionFactory = Resolve<ISessionFactory>();
            Lazy<ISession> session = new Lazy<ISession>(() => sessionFactory.OpenSession());
            LazySessionContext.Bind(session, sessionFactory);
            return sessionFactory;
        }
    }
}