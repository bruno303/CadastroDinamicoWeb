using System.Collections.Generic;

namespace CadastroDinamico.Web.Models
{
    public class ColunaChaveEstrangeiraViewModel
    {
        public string Nome { get; set; }
        public string TabelaReferenciada { get; set; }
        public string ColunaReferenciada { get; set; }
        public int IndiceColTabelaReferenciada { get; set; }

        public List<string> ColunasTabelaReferenciada { get; set; }
    }
}
