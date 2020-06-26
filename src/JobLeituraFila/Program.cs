using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLeituraFila
{
    class Program
    {
        /// <summary>
        /// Use this to test reading messages from service bus. Use a azure event grid subscriber of type Queue to test your steps
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Conexao de storage account
            var storageConection = "DefaultEndpointsProtocol=https;AccountName=***;AccountKey=***;EndpointSuffix=core.windows.net";
            //Conexao de services bus
            var sbConection = "Endpoint=sb://****.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=+2h62lFuFg4Z6H9VQn4Sa++QKM3u1dPECYFpoT7R34s=";
            var config = new JobHostConfiguration(storageConection);
            var serviceBusConfig = new ServiceBusConfiguration
            {
                ConnectionString = sbConection,
                MessageOptions = new OnMessageOptions
                {
                    AutoComplete = false,
                    MaxConcurrentCalls = 1
                }
            };
            config.UseServiceBus(serviceBusConfig);
            var host = new JobHost(config);
            
            host.RunAndBlock();
        }
    }
}
