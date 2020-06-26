using Microsoft.Xrm.Sdk;
using PluginsCore.Model;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace PluginsCore
{
    public class EventGridPublish : PluginBase
    {
        private readonly string _topicHostName;
        private readonly string _domainName;
        private readonly string _domainKey;
        public EventGridPublish(string topicHostNameAndDomain, string domainKey)
        {
            try
            {
                _domainKey = domainKey;
                var config = topicHostNameAndDomain.Split(new[] { Environment.NewLine },
                                   StringSplitOptions.RemoveEmptyEntries);
                _topicHostName = config[0];
                _domainName = config[1];
            }
            catch
            {
                _topicHostName = string.Empty;
                _domainName = string.Empty;
            }

        }
        public EventGridPublish()
        {
            
        }

        protected override void OnExecute()
        {
            if (Contexto.Stage == (int)Stagio.PosEvento)
            {
                if (string.IsNullOrEmpty(_topicHostName) || string.IsNullOrEmpty(_domainName))
                {
                    TracingService.Trace("Plugin não foi configurado corretamente.");
                    throw new InvalidPluginExecutionException("Plugin nao foi configurado corretamente");
                }

                try
                {
                    TracingService.Trace($"Iniciando post no eventgrid");
                    TracingService.Trace($"Eventgrid: {_topicHostName}");
                    TracingService.Trace($"Domain: {_domainName}");

                    var eventGridClient = new Service.EventGridClient(_domainKey);
                    var egEvent = MontarEvento(_domainName);
                    var result = eventGridClient.PublishEvents(_topicHostName, egEvent).GetAwaiter().GetResult();
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        TracingService.Trace("Mensagem publicada com sucesso");
                    }
                    else
                    {
                        string responseText = result.Content.ReadAsStringAsync().Result;
                        throw new InvalidPluginExecutionException($"Ocorreu um erro ao publicar no event grid. Status {result.StatusCode} - {responseText}");
                    }
                }
                catch (InvalidPluginExecutionException ex)
                {
                    TracingService.Trace("InvalidPluginExecutionException: {0}", ex.ToString());
                    throw;
                }
                catch (Exception ex)
                {
                    TracingService.Trace("Exception: {0}", ex.ToString());
                    throw new InvalidPluginExecutionException($"Ocorreu um erro ao publicar no event grid. {ex.Message}", ex);
                }
            }
        }

        private IList<EventGridEvent> MontarEvento(string domain)
        {
            var eventsList = new List<EventGridEvent>();
            eventsList.Add(new EventGridEvent()
            {
                Id = Guid.NewGuid().ToString(),
                EventType = $"{EntidadeContexto.LogicalName}-{Contexto.MessageName}",
                Data = MontarEntidadeEnvio(),
                EventTime = DateTime.Now,
                Subject = $"CRM 365-{EntidadeContexto.LogicalName}",
                DataVersion = "1.0",
                Topic = domain
            });

            return eventsList;
        }

        /// <summary>
        /// Serialize entity using CRM Context and imagePre to get a complet entity body
        /// </summary>
        /// <returns></returns>
        private Model.Entidade MontarEntidadeEnvio()
        {
            var registro = new Model.Entidade(EntidadeContexto.Id, EntidadeContexto.LogicalName);
            TracingService.Trace("Montando entidade de envio com o contexto");
            PreencheEntidade(registro, EntidadeContexto);

            if (Contexto.PreEntityImages.Contains("IMAGEPRE") )
            {
                TracingService.Trace("Montando entidade de envio com o imagepre");
                var image = Contexto.PreEntityImages["IMAGEPRE"];
                PreencheEntidade(registro, image);
            }
            return registro;
        }

        
        private void PreencheEntidade(Entidade registro, Entity entidadeCrm)
        {
            foreach (KeyValuePair<string, object> campo in entidadeCrm.Attributes)
            {
                
                if (registro.Atributos.ContainsKey(campo.Key)) continue;
                if (campo.Value == null) continue;
                switch (campo.Value.GetType().ToString())
                {
                        case "Microsoft.Xrm.Sdk.OptionSetValue":
                            TracingService.Trace($"Lendo atributo OptionSetValue {campo.Key}");
                            registro.Atributos.Add(campo.Key, ((OptionSetValue)campo.Value).Value);
                        if (entidadeCrm.FormattedValues.Contains(campo.Key))
                            registro.Atributos.Add($"{campo.Key}_descricao", entidadeCrm.FormattedValues[campo.Key]);
                            break;
                        case "Microsoft.Xrm.Sdk.EntityReference":
                            TracingService.Trace($"Lendo atributo EntityReference {campo.Key}");
                            var campoReferencia = (EntityReference)campo.Value;
                            registro.Atributos.Add(campo.Key, campoReferencia.Id);                            
                            registro.Atributos.Add($"{campo.Key}_nome", campoReferencia.Name);
                            break;

                        case "System.String":
                        case "System.Int32":
                        case "System.Double":
                        case "System.Decimal":
                        case "System.Boolean":
                        case "System.DateTime":
                        case "System.Guid":
                            TracingService.Trace($"Lendo atributo primitivo {campo.Key}");
                            registro.Atributos.Add(campo.Key, campo.Value);
                            break;
                        default:
                            TracingService.Trace($"Lendo atributo ignorado {campo.Key}");
                            registro.Atributos.Add(campo.Key, $"tipo ignorado {campo.Value.GetType()}");
                            break;
                }
            }
            
        }
    }
}
