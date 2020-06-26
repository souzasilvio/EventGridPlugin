using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace PluginsCore.Tests
{
    [TestClass]
    public class EventGridTest
    {
        [TestMethod]
        public void TemplateMethodSucess()
        {
            var entidade = new Entity();
            entidade.Id = Guid.NewGuid();
            entidade.LogicalName = "account";
            entidade["new_cpf"] = "366.628.938-03";
            entidade["new_email"] = "123";

            var entidadeImage = new Entity();
            entidadeImage.Id = entidade.Id;
            entidadeImage.LogicalName = "account";
            entidadeImage["new_nome"] = "Maria";
            entidadeImage["new_email"] = "123";
            entidadeImage["DataCriacao"] = DateTime.Now;
            entidadeImage["Tipo"] = new OptionSetValue(1);
            entidadeImage.FormattedValues["Tipo"] = "Op 1";
            var rel1 = new EntityReference("Cliente", Guid.NewGuid());
            rel1.Name = "Cliente relacionado";
            entidadeImage["Relacao1"] = rel1;


            var pluginContext = new TestPluginContext();
            var provider = new TesteServiceProvider(pluginContext);

            pluginContext.InputParameters["Target"] = entidade;
            pluginContext.PreEntityImages["IMAGEPRE"] = entidadeImage;
            pluginContext.PrimaryEntityName = entidade.LogicalName;
            pluginContext.MessageName = "Create";
            pluginContext.Stage = (int)TesteServiceProvider.Stagio.PosEvento;

            //Set eventgrid host and event grid domain on unsecure string plugin parameter
            //All CRM Posts to event grid will be to a single event grid domain
            //Subscribers can filter entitys by event-type
            string p1 = "egdsadinfo.brazilsouth-1.eventgrid.azure.net\r\ncliente";
            //Set event grid key on secure string plugin parameter
            string p2 = "vvNouwOKw7HqhzoWHFtH8QlWuXF+NfwzFUqK7BqLVMk=";
            var plugin = new PluginsCore.EventGridPublish(p1,p2);
            try
            {
                plugin.Execute(provider);
            }
            catch (InvalidPluginExecutionException ex)
            {
                Assert.IsTrue(true, "");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
