using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginsCore
{
    public sealed class Constantes
    {

        /// <summary>
        /// prefixo padrão das entidades
        /// </summary>
        public const string prefixoEntidades = "mrv";

        public struct EntidadeTeste
        {
            public const string nome = "mrv_teste";
        }

        public struct Cliente
        {
            public const string nomeLogico = "account";
            public const string campoAutoNumeracao = "new_cod_cliente";
        }

    }
}
