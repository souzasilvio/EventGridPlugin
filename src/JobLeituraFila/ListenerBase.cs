using Microsoft.Azure.EventGrid.Models;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace JobLeituraFila
{
    public class ListenerBase
    {
        
        /// <summary>
        /// Tempo de pausa em caso de erro
        /// </summary>
        protected static double tempoPausa = 5;
      
        private static object GetMessagePropertie(BrokeredMessage msg, string propriedade)
        {
            if (msg.Properties.ContainsKey(propriedade))
            {
                return msg.Properties[propriedade];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Faz uma pausa no listner em caso de erro de comunicação com algum serviço (CRM, Azure, etc)
        /// </summary>
        /// <param name="log"></param>
        /// <param name="ex"></param>
        protected static void EfetuaPausa(TextWriter log, Exception ex)
        {
            log.WriteLine(ex.Message);
            log.WriteLine($"Job pausado por {tempoPausa} minutos");
            Task.Delay(TimeSpan.FromMinutes(tempoPausa)).Wait();
        }


        public static EventGridEvent GetFromBrokeredMsg(BrokeredMessage message)
        {
            Stream stream = message.GetBody<Stream>();
            StreamReader reader = new StreamReader(stream);
            string payload = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<EventGridEvent>(payload);
        }


    }
}