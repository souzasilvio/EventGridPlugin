using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using PluginsCore.Model;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace PluginsCore
{
    /// <summary>
    /// Plugin base. Use as a base class to all plugins. Check some samples to know how to use.
    /// Note: This classe is based on code published on communit. Thanks to orignal autor.
    /// </summary>
    public abstract class PluginBase : IPlugin
    {

        public PluginBase()
        { }


        /// <summary>
        /// Constructor criado para usar em teste unitario do base
        /// </summary>
        /// <param name="serviceProvider"></param>
        public PluginBase(IServiceProvider serviceProvider)
        {
            Contexto = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            ServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            Service = ServiceFactory.CreateOrganizationService(this.Contexto.UserId);
        }

        public const string NomeImagePre = "IMAGEPRE";
        public const string NomeImagePos = "IMAGEPOS";
        public enum Mensagem { Create, Update, Delete, Win, Lose, Close, Assign };
        public enum Stagio { PreEvento = 10, PosEvento = 40 };


        public IOrganizationServiceFactory ServiceFactory { get; set; }
        public IPluginExecutionContext Contexto { get; set; }
        public ITracingService TracingService { get; set; }
        public IOrganizationService Service { get; set; }

        /// <summary>
        /// Valida contexto de entity, pre, para create ou update
        /// </summary>
        public bool IsPreEventForCreateUpdate
        {
            get {
                return (TargetIsEntity &&
                     Contexto.Stage == (int)Stagio.PreEvento &&
                    (Contexto.MessageName == Mensagem.Create.ToString() || Contexto.MessageName == Mensagem.Update.ToString()));
            }
        }

        /// <summary>
        /// Verifica se o InputParameters contem um Entity na propriedade Target
        /// </summary>
        public bool TargetIsEntity
        {
            get
            {
                return Contexto.InputParameters.Contains("Target") && Contexto.InputParameters["Target"] is Entity;
            }
        }
        /// <summary>
        /// Valida contexto de entity, pre, para create ou update
        /// </summary>
        public bool IsPreEventForCreate
        {
            get
            {
                return (TargetIsEntity &&
                     Contexto.Stage == (int)Stagio.PreEvento &&
                     Contexto.MessageName == Mensagem.Create.ToString());
            }
        }


        /// <summary>
        /// Valida contexto de entity, pre, para update
        /// </summary>
        public bool IsPreEventForUpdate
        {
            get
            {
                return (TargetIsEntity &&
                     Contexto.Stage == (int)Stagio.PreEvento &&
                     Contexto.MessageName == Mensagem.Update.ToString());
            }
        }

        public bool IsPostEventForUpdate
        {
            get
            {
                return (TargetIsEntity &&
                     Contexto.Stage == (int)Stagio.PosEvento &&
                     Contexto.MessageName == Mensagem.Update.ToString());
            }
        }

        /// <summary>
        /// Retorna entidade objeto do plugin
        /// </summary>
        public Entity EntidadeContexto
        {
            get { return (Entity)Contexto.InputParameters["Target"]; }
        }

        public Object ObterAtributo(string campo)
        {
            return EntidadeContexto.Contains(campo) ? this.EntidadeContexto[campo] : null;
        }

       
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                Contexto = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                ServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                Service = ServiceFactory.CreateOrganizationService(this.Contexto.UserId);
                OnExecute();
            }
            catch (Exception ex)
            {
                try
                {
                    OnError(ex);
                }
                catch {
                }

                throw;
            }
            finally
            {
                OnCleanup();
            }
        }

        // Todos as classes herdadas devem fazer o override deste metodo
        protected abstract void OnExecute();

        // method is virtual so if an inheriting class wishes to do something different, they can
        protected virtual void OnError(Exception ex)
        {
            // Perform logging how ever you log:
            //Logger.Write(ex);
        }

        /// <summary>
        /// Cleanup resources.
        /// </summary>
        protected virtual void OnCleanup()
        {
            // Allows inheriting class to perform any cleaup after the plugin has executed and any exceptions have been handled
        }

        public void Trace(string mensagem, params object[] args)
        {
            if (this.TracingService != null)
                this.TracingService.Trace(mensagem, args);
        }
       

        /// <summary>
        /// Retrieve a record on the fly based on a simple filter
        /// </summary>
        /// <param name="entityName">Entity to Query</param>
        /// <param name="fieldToFilter">Field to query</param>
        /// <param name="valueToFilter">Value to query exact match</param>
        /// <param name="atributos">Fields to return</param>
        /// <returns></returns>
        public Entity ObterRegistroPorFiltro(string entityName, string fieldToFilter, object valueToFilter, params string[] atributos)
        {
            var colecao = ObterColecaoPorFiltro(entityName, fieldToFilter, valueToFilter, atributos);
            if (colecao.Entities.Count > 0)
                return colecao.Entities[0];
            else
                return null;
        }

        /// <summary>
        /// Return a collection by exact filter
        /// </summary>
        /// <param name="entityName">Entity to query</param>
        /// <param name="fieldToFielter">Field to filter</param>
        /// <param name="valueToFilter">Value to filter on field. Exact Match</param>
        /// <param name="atributos">Fields to return</param>
        /// <returns></returns>
        public EntityCollection ObterColecaoPorFiltro(string entityName, string fieldToFilter, object valueToFilter, params string[] atributos)
        {
            var query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(atributos);
            query.Criteria.AddCondition(fieldToFilter, ConditionOperator.Equal, valueToFilter);
            return Service.RetrieveMultiple(query);
        }

        /// <summary>
        /// Retorna um registro pelo seu id
        /// </summary>
        /// <param name="entityName">Entidade</param>
        /// <param name="id">Id</param>
        /// <param name="atributos">Colunas</param>
        /// <returns>Entity</returns>
        public Entity ObterPorId(string entityName, Guid id, params string[] atributos)
        {
            return Service.Retrieve(entityName, id, new ColumnSet(atributos));
        }

        /// <summary>
        /// Retorna o valor de uma chave na entidade new_configuracao_crm
        /// </summary>
        /// <param name="chave"></param>
        /// <returns></returns>
        public string ObterConfiguracaoCrm(string chave)
        {
            var query = new QueryExpression("new_configuracao_crm");
            query.ColumnSet.AddColumns("new_valor", "new_criptografado");
            query.Criteria.AddCondition("new_chave", ConditionOperator.Equal, chave);
            var colecao = Service.RetrieveMultiple(query);
            if(colecao.Entities.Count > 0)
            {
                var criptografado = (Boolean)colecao[0]["new_criptografado"];
                if (criptografado)
                {
                    return Util.DecryptString(colecao[0]["new_valor"].ToString());
                }
                else
                {
                    return colecao[0]["new_valor"].ToString();
                }
            }
            throw new InvalidPluginExecutionException($"Chave {chave} não cadastrada na entidade Configuracao Crm.");
        }

     

        public bool ValidaIntegracaoTemporaria()
        {
            if (TargetIsEntity)
            {
                Entity entidadeContexto = (Entity)Contexto.InputParameters["Target"];
                if (entidadeContexto.Contains("mrv_integracao_temporaria") &&
                    !string.IsNullOrEmpty(entidadeContexto["mrv_integracao_temporaria"].ToString()))
                {
                    return true;
                }

                if (Contexto.ParentContext != null &&
                    Contexto.ParentContext.InputParameters.Contains("Target") &&
                    Contexto.ParentContext.InputParameters["Target"] is Entity)
                {
                    var contextoPai = (Entity)Contexto.ParentContext.InputParameters["Target"];
                    if (contextoPai.Contains("mrv_integracao_temporaria") &&
                    !string.IsNullOrEmpty(contextoPai["mrv_integracao_temporaria"].ToString()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
