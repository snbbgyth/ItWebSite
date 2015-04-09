using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ItWebSite.Core.BLL;

namespace ItWebSite.Crawler.Help
{
    public class Helper
    {
        private static IContainer _container;

        static Helper()
        {
          _container=  BuildContainer();
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            // FunnelWeb Database
            builder.RegisterModule(new CoreModule());
            return builder.Build();
        }


        public static T Resolve<T>()
        {
            try
            {
                return _container.Resolve<T>();
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }
}
