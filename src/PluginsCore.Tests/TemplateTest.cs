using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace PluginsCore.Tests
{
    [TestClass]
    public class TemplateTest
    {
        [TestMethod]
        public void TemplateMethodSucess()
        {
            //var entidade = new Entity();
            //entidade.LogicalName = "account";
            //entidade["new_cpf"] = "366.628.938-03";

            //var pluginContext = new TestPluginContext();
            //var provider = new TesteServiceProvider(pluginContext);

            //pluginContext.InputParameters["Target"] = entidade;
            //pluginContext.PrimaryEntityName = entidade.LogicalName;
            //pluginContext.MessageName = "Create";
            //pluginContext.Stage = (int)TesteServiceProvider.Stagio.PreEvento;


            //var plugin = new ValidaCpfCnpjDuplicado();
            //try
            //{
            //    plugin.Execute(provider);
            //}
            //catch (InvalidPluginExecutionException ex)
            //{
            //    Assert.IsTrue(ex.Message == ValidaCpfCnpjDuplicado.MensagemErro);
            //}
            //catch (Exception ex)
            //{
            //    Assert.Fail(ex.Message);
            //}
        }
    }
}
