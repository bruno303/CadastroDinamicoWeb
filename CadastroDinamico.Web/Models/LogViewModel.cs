using System;

namespace CadastroDinamico.Web.Models
{
    public class LogViewModel
    {
        public int IdLog { get; set; }
        public string Database { get; set; }
        public string Schema { get; set; }
        public string Tabela { get; set; }
        public string Metodo { get; set; }
        public string Query { get; set; }
        public string Usuario { get; set; }
        public DateTime DataHora { get; set; }
    }
}