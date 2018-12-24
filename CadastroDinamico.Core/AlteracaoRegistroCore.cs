using CadastroDinamico.Dominio;
using System.Collections.Generic;

namespace CadastroDinamico.Core
{
    internal class AlteracaoRegistroCore
    {
        private List<Coluna> colunas;
        private Dictionary<string, string> valores;
        private Dictionary<string, object> valoresTratados;

        public AlteracaoRegistroCore(List<Coluna> colunas, Dictionary<string, string> valores)
        {
            this.colunas = colunas;
            this.valores = valores;
            valoresTratados = new Dictionary<string, object>();
            TratarValores();
        }

        public Dictionary<string, object> GetValoresTratados()
        {
            return valoresTratados;
        }

        private void TratarValores()
        {
            foreach (var item in colunas)
            {
                if (!item.IsChavePrimaria)
                {
                    if (valores.ContainsKey(item.NomeInput))
                    {
                        valoresTratados.Add(item.Nome, valores[item.NomeInput]);
                    }
                    else if (item.Tipo.ToUpper() == "BIT")
                    {
                        valoresTratados.Add(item.Nome, false);
                    }
                }
            }
        }
    }
}
