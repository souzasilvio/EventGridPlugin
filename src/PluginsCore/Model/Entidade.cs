using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginsCore.Model
{
    public class Entidade
    {
        public Entidade()
        {
            Atributos = new Dictionary<string, object>();
        }

        public Entidade(Guid id, string nomeLogico)
        {
            Id = id;
            NomeLogico = nomeLogico;
            Atributos = new Dictionary<string, object>();
        }

        public Guid Id { get; set; }
        public string NomeLogico { get; set; }

        public Dictionary<string, object> Atributos { get; set; }

    }
}
