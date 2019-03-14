using CadastroDinamico.Dominio;
using System.Collections.Generic;

namespace CadastroDinamico.Core
{
    internal class AlteracaoRegistroCore
    {
        private List<Coluna> colunas;
        private Dictionary<string, string> valores;
        private Dictionary<string, object> valoresTratados;

        public AlteracaoRegistroCore(List<Coluna> colunas, Dictionary<string, string> valores, bool filtro = true)
        {
            this.colunas = colunas;
            this.valores = valores;
            valoresTratados = new Dictionary<string, object>();
            TratarValores(filtro);
        }

        public Dictionary<string, object> GetValoresTratados()
        {
            return valoresTratados;
        }

        private void TratarValores(bool filtro)
        {
            foreach (var item in colunas)
            {
                if (!filtro)
                {
                    if (!item.IsChavePrimaria)
                    {
                        if (valores.ContainsKey(item.NomeInput))
                        {
                            if (item.IsChaveEstrangeira)
                            {
                                if (valores[item.NomeInput] == "0")
                                {
                                    valoresTratados.Add(item.Nome, "NULL");
                                }
                                else
                                {
                                    valoresTratados.Add(item.Nome, valores[item.NomeInput]);
                                }
                            }
                            else
                            {
                                valoresTratados.Add(item.Nome, valores[item.NomeInput]);
                            }
                        }
                        else if (item.Tipo.ToUpper() == "BIT")
                        {
                            valoresTratados.Add(item.Nome, false);
                        }
                    }
                    else if (item.IsChaveEstrangeira)
                    {
                        if (valores[item.NomeInput] == "0")
                        {
                            valoresTratados.Add(item.Nome, "NULL");
                        }
                        else
                        {
                            valoresTratados.Add(item.Nome, valores[item.NomeInput]);
                        }
                    }
                }
                else // Tratar filtros
                {
                    if (valores.ContainsKey(item.NomeInput))
                    {
                        if (item.IsChaveEstrangeira)
                        {
                            if (valores[item.NomeInput] != "0")
                            {
                                valoresTratados.Add(item.Nome, valores[item.NomeInput]);
                            }
                        }
                        else
                        {
                            valoresTratados.Add(item.Nome, valores[item.NomeInput]);
                        }
                    }
                }
            }
        }
    }
}
