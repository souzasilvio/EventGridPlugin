using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using PluginsCore.Tests.Services;

namespace PluginsCore.Tests
{
    public class TesteServiceProvider : IServiceProvider
    {
        public enum Stagio { PreEvento = 10, PosEvento = 40 };

        #region IServiceProvider Members
        private TestPluginContext _pluginContext;
        public TesteServiceProvider(TestPluginContext pluginContext)
        {
            _pluginContext = pluginContext;
        }

       public object GetService(Type serviceType)
       {
            if (serviceType == typeof(IPluginExecutionContext))
            {
                return _pluginContext;
            }
            if (serviceType == typeof(IOrganizationServiceFactory))
            {
                return new TestServiceFactory();
            }

            if (serviceType == typeof(ITracingService))
            {
                return new TraceFake();
            }

            return null;
        }

     

        #endregion
    }
}
