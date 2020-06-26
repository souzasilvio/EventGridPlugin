using JobLeituraFila.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace JobLeituraFila
{
    /// <summary>
    /// Listerner principal - fila cliente
    /// </summary>
    public class ListenerCliente : ListenerBase
    {
        /// <summary>
        /// Listener para processar as mensagens enviadas pelo EventGrid para a fila de cliente de escuta do CRM
        /// A fila do CRM cliente deve ser congigurada como endpoint no EventGrid para o topic cliente
        /// </summary>
        /// <param name="message">Mensagen do event grid para o topic cliente</param>
        /// <param name="log">Log</param>
        [Disable("DesabilitaListnerCliente")]
        public static void ProcessaFilaCliente([ServiceBusTrigger("cliente")] BrokeredMessage message, TextWriter log)
        {
            var msgEvento = GetFromBrokeredMsg(message);
            var cliente = new Entidade();
            try
            {
                //Sincronizar apenas clientes com cpf/cnpj e codigoclientesap ou algum outro campo indicado a existencia
                //no CRM                
                if (msgEvento.EventType.ToLower() == "account-update" || msgEvento.EventType.ToLower() == "account-create")
                {
                    cliente = JsonConvert.DeserializeObject<Entidade>(msgEvento.Data.ToString());
                    foreach (KeyValuePair<string, object> campo in cliente.Atributos)
                    {
                        Console.WriteLine($"Campo {campo.Key} Valor {campo.Value}");
                    }
                    message.CompleteAsync();
                  
                }
                else
                {
                    message.CompleteAsync();
                }
            }            
            catch (Exception ex)
            {
               message.DeadLetterAsync("Erro", ex.Message);
               
            }
        }
    }
}