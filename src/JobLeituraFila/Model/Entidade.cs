using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLeituraFila.Model
{
    public class Entidade
    {
        public Entidade()
        {
            Atributos = new List<KeyValuePair<string, object>>();
        }

        public Entidade(Guid id, string nomeLogico)
        {
            Id = id;
            NomeLogico = nomeLogico;
            Atributos = new List<KeyValuePair<string, object>>();
        }

        public Guid Id { get; set; }
        public string NomeLogico { get; set; }

        public List<KeyValuePair<string, object>> Atributos { get; set; }

    }
}
