using System.Collections.Generic;

namespace CadastroDinamico.Dominio
{
    public class Coluna
    {
        public string Nome { get; set; }
        public int Posicao { get; set; }
        public string ExpressaoValorDefault { get; set; }
        public bool AceitaNull { get; set; }
        public string Tipo { get; set; }
        public int TamanhoMaximo { get; set; }
        public int PrecisaoNumerica { get; set; }
        public int PrecisaoNumericaRadix { get; set; }
        public bool IsChavePrimaria { get; set; }
        public bool IsChaveEstrangeira { get; set; }
        public string TabelaReferenciada { get; set; }
        public string TabelaReferenciadaChavePrimaria { get; set; }
        public string ColunaReferenciada { get; set; }
        public string ColunaReferenciadaChavePrimaria { get; set; }
        public List<TabelaEstrangeira> ListaSelecao { get; set; }
        public string NomeInput { get; set; }
        public bool IsIdentity { get; set; }
    }
}
