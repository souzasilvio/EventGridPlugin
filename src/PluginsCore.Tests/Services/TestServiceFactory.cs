using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Client;
using System.Configuration;
using Microsoft.Xrm.Tooling.Connector;
using System.Diagnostics;

namespace PluginsCore.Tests
{
    public class TestServiceFactory : IOrganizationServiceFactory
    {
        private static OrganizationServiceProxy _serviceProxy;
        
        public IOrganizationService CreateOrganizationService(Guid? userId)
        {
            if (_serviceProxy == null)
            {
                var ct = ConfigurationManager.ConnectionStrings["Crm365"].ConnectionString;
                var conn = new CrmServiceClient(ct);
                if (!conn.IsReady)
                {
                    throw new ApplicationException(conn.LastCrmError);
                }
                _serviceProxy = conn.OrganizationServiceProxy;
                _serviceProxy.EnableProxyTypes();
            }
            return _serviceProxy;
        }

    }


}
