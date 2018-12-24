using CadastroDinamico.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using SqlClient = CadastroDinamico.Repositorio.SqlClient;

namespace CadastroDinamico.Core
{
    public class TabelaCore
    {
        public string Nome { get; set; }
        public string Database { get; set; }
        public string Schema { get; set; }
        public bool TemChavePrimaria { get; set; }
        public bool TemChaveEstrangeira { get; set; }
        public int QuantidadeCampos { get; set; }
        public List<Coluna> Colunas { get; set; }
        public List<Coluna> TodasColunas { get; set; }
        public List<object> Valores { get; set; }
        public MatrizValores ValoresMultilinha { get; set; }
        public List<Coluna> ChavesPrimarias { get; set; }
        public int QuantidadeColunas { get; set; }
        public int QuantidadeLinhas { get; set; }

        private List<string> camposExibir;
        private string pkAlteracao;

        #region Construtores
        public TabelaCore(string tabela, string schema, string database)
        {
            Nome = tabela;
            Schema = schema;
            Database = database;
            Valores = new List<object>();
        }

        public TabelaCore(string tabela, string schema)
        {
            Nome = tabela;
            Schema = schema;
            Valores = new List<object>();
        }

        public TabelaCore(string tabela)
        {
            Nome = tabela;
            Valores = new List<object>();
        }

        public TabelaCore() { }
        #endregion

        public string Carregar()
        {
            string retorno = string.Empty;
            SqlClient.Repositorio repositorio = new SqlClient.Repositorio();

            if (string.IsNullOrEmpty(Database))
            {
                throw new ArgumentException("Propriedade Database é obrigatória.");
            }

            if (string.IsNullOrEmpty(Nome))
            {
                throw new ArgumentException("Propriedade Nome da tabela é obrigatória.");
            }

            if (string.IsNullOrEmpty(Schema))
            {
                Schema = "DBO";
            }

            try
            {
                pkAlteracao = string.Empty;
                Colunas = repositorio.RetornarColunas(Database, Schema, Nome);
                TodasColunas = new List<Coluna>();
                TodasColunas.AddRange(Colunas);
                CarregarNomesInput();
                TemChavePrimaria = Colunas.Where(p => p.IsChavePrimaria).FirstOrDefault() != null;
                TemChaveEstrangeira = Colunas.Where(p => p.IsChaveEstrangeira).FirstOrDefault() != null;
                camposExibir = repositorio.SelecionarColunasVisiveis(Database, Schema, Nome);
                ChavesPrimarias = repositorio.RetornarColunasChavePrimariaTabela(Nome, Schema, Database);
                RemoverColunasIgnoradas();
                QuantidadeCampos = Colunas.Count;
                CarregarColunasChaveEstrangeira();
            }
            catch (Exception ex)
            {
                retorno = ex.Message;
            }

            return retorno;
        }

        private void CarregarNomesInput()
        {
            for (int cont = 0; cont < Colunas.Count; cont++)
            {
                Colunas[cont].NomeInput = "inputColuna" + cont.ToString();
            }
        }

        private void CarregarColunasChaveEstrangeira()
        {
            /* Carrega SelectList para os campos de chave estrangeira */
            for (int cont = 0; cont < Colunas.Count; cont++)
            {
                if (Colunas[cont].IsChaveEstrangeira)
                {
                    Colunas[cont].ListaSelecao = RetornarListaTabelaEstrangeira(Colunas[cont].TabelaReferenciada);
                }
            }
        }

        private void RemoverColunasIgnoradas()
        {
            int cont = 0;
            /* Remove campos que não serão manipulados */
            while (cont < Colunas.Count)
            {
                if (!(camposExibir.Count == 1 && camposExibir[0].Equals(string.Empty)))
                {
                    if (!camposExibir.Contains(Colunas[cont].Nome.ToUpper()))
                    {
                        Colunas.RemoveAt(cont);
                    }
                    else
                    {
                        cont++;
                    }
                }
                else
                {
                    cont++;
                }
            }
        }

        private List<TabelaEstrangeira> RetornarListaTabelaEstrangeira(string tabela)
        {
            List<TabelaEstrangeira> itens = null;
            var repositorio = new SqlClient.Repositorio();
            string chavePrimaria = "";
            string descricao = "";

            try
            {
                chavePrimaria = repositorio.RetornarColunasChavePrimariaTabela(tabela, Schema, Database)[0].Nome;
                descricao = repositorio.SelecionarDescricaoChaveEstrangeiraConfiguracaoTabela(Database, Schema, Nome, chavePrimaria);
                itens = repositorio.SelectTabela(chavePrimaria, descricao, tabela, Schema, Database);
            }
            catch (Exception)
            {
                throw;
            }
            return itens;
        }

        private string MontarWhereChavesPrimarias(string pk)
        {
            var retorno = string.Empty;
            if (!string.IsNullOrEmpty(pk) && pk.IndexOf(":") > 0)
            {
                var listaPks = pk.Split(";");
                if (listaPks.Length > 0)
                {
                    foreach (var item in listaPks)
                    {
                        var chaveValor = item.Split(":");
                        if (retorno != string.Empty)
                        {
                            retorno += " AND ";
                        }
                        retorno += chaveValor[0] + " = " + chaveValor[1];
                    }
                }
            }

            return retorno;
        }

        public string MontarUrlChavesPrimarias(int linha)
        {
            var retorno = string.Empty;

            for (int cont = 0; cont < Colunas.Count; cont++)
            {
                if (Colunas[cont].IsChavePrimaria)
                {
                    retorno += Colunas[cont].Nome + ":" + ValoresMultilinha.GetValor(linha, cont) + ";";
                }
            }

            if (!string.IsNullOrEmpty(retorno))
            {
                retorno = retorno.Substring(0, retorno.Length - 1);
            }

            return retorno;
        }

        public void CarregarValores(bool amostra = false, string pk = "")
        {
            var repositorio = new SqlClient.Repositorio();
            var query = string.Empty;
            var where = string.Empty;

            try
            {
                where = MontarWhereChavesPrimarias(pk);
                query = RetornarSelect(where, amostra);

                if (amostra)
                {
                    ValoresMultilinha = repositorio.RetornarValoresAmostraDados(query);
                }
                else
                {
                    Valores = repositorio.RetornarValores(query);
                    pkAlteracao = pk;
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public string RetornarSelect(string where, bool amostra = false)
        {
            string query = string.Empty;

            query = "SELECT " + (amostra ? "TOP(10) " : "TOP(1) ");
            foreach (var coluna in Colunas)
            {
                query += coluna.Nome + ", ";
            }
            query = query.Remove(query.Length - 2, 2);
            query += string.Format(" FROM {0}.{1}.{2} WITH(NOLOCK)", Database, Schema, Nome);
            
            if (!string.IsNullOrWhiteSpace(where))
            {
                query += " WHERE " + where;
            }

            return query;
        }

        public string RetornarUpdate(string pk, Dictionary<string, object> valores)
        {
            string query = string.Empty;

            query = string.Format("UPDATE {0}.{1}.{2} SET ", Database, Schema, Nome);
            for (int cont = 0; cont < Colunas.Count; cont++)
            {
                if (!Colunas[cont].IsChavePrimaria)
                {
                    string valorParametro = valores[Colunas[cont].Nome].ToString();

                    if (Colunas[cont].Tipo.ToUpper() == "DATE" ||
                            Colunas[cont].Tipo.ToUpper() == "DATETIME" ||
                            Colunas[cont].Tipo.ToUpper() == "DATETIME2")
                    {
                        valorParametro = AjustarFormatoData(valorParametro);
                    }

                    if (Colunas[cont].Tipo.ToUpper() == "VARCHAR" ||
                        Colunas[cont].Tipo.ToUpper() == "CHAR" ||
                        Colunas[cont].Tipo.ToUpper() == "NVARCHAR" ||
                        Colunas[cont].Tipo.ToUpper() == "NCHAR" ||
                        Colunas[cont].Tipo.ToUpper() == "DATE" ||
                        Colunas[cont].Tipo.ToUpper() == "DATETIME" ||
                        Colunas[cont].Tipo.ToUpper() == "TIME" ||
                        Colunas[cont].Tipo.ToUpper() == "TIMESPAN" ||
                        Colunas[cont].Tipo.ToUpper() == "DATETIME2")
                    {
                        valorParametro = "'" + valorParametro + "'";
                    }
                    else if (Colunas[cont].Tipo.ToUpper() == "BIT")
                    {
                        if (valorParametro.ToUpper() == bool.TrueString.ToUpper() || valorParametro.ToUpper() == "ON")
                        {
                            valorParametro = "1";
                        }
                        else
                        {
                            valorParametro = "0";
                        }
                    }
                    query += string.Format("{0} = {1}, ", Colunas[cont].Nome, valorParametro);
                }
            }
            query = query.Substring(0, query.Length - 2);

            if (!string.IsNullOrWhiteSpace(pk))
            {
                query += " WHERE " + MontarWhereChavesPrimarias(pk);
            }

            return query;
        }

        public string GetPk()
        {
            return pkAlteracao;
        }

        private string AjustarFormatoData(string data)
        {
            string dataCorreta = string.Empty;

            if (!string.IsNullOrWhiteSpace(data))
            {
                dataCorreta = data.Substring(6, 4) + "-" + data.Substring(3, 2) + "-" + data.Substring(0, 2) + data.Substring(10, data.Length - 10);
            }

            return dataCorreta;
        }

        public string AlterarRegistro(Dictionary<string, string> valores)
        {
            string retorno = string.Empty;
            try
            {
                SqlClient.Repositorio repositorio = new SqlClient.Repositorio();
                var pk = string.Empty;
                AlteracaoRegistroCore alteracaoRegistro = new AlteracaoRegistroCore(Colunas, valores);

                var valoresTratados = alteracaoRegistro.GetValoresTratados();
                pk = valores["pk"];

                var query = RetornarUpdate(pk, valoresTratados);
                retorno = repositorio.AlterarValores(query);
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return retorno;
        }

        public void SalvarConfiguracoesColunas(List<string> colunasVisiveis, List<string> colunasChave, List<string> colunasFiltro)
        {
            try
            {
                var repositorio = new SqlClient.Repositorio();
                var idConfiguracaoTabela = repositorio.SelecionarIdConfiguracaoTabela(Database, Schema, Nome);
                var idConfiguracaoTabelaColuna = 0;

                repositorio.SalvarConfiguracoesTabela(idConfiguracaoTabela, Database, Schema, Nome);
                idConfiguracaoTabela = repositorio.SelecionarIdConfiguracaoTabela(Database, Schema, Nome);

                foreach (var coluna in TodasColunas)
                {
                    var visivel = colunasVisiveis.Contains(coluna.Nome);
                    var filtro = colunasFiltro?.Contains(coluna.Nome) ?? false;
                    var chave = string.Empty;
                    if (colunasChave != null)
                    {
                        foreach (var item in colunasChave)
                        {
                            if (chave == string.Empty)
                            {
                                if (item.Split(":")[0] == coluna.Nome)
                                {
                                    chave = item.Split(":")[1];
                                }
                            }
                        }   
                    }
                    idConfiguracaoTabelaColuna = repositorio.SelecionarIdConfiguracaoTabelaColuna(Database, Schema, Nome, coluna.Nome);
                    repositorio.SalvarConfiguracaoColuna(idConfiguracaoTabelaColuna, idConfiguracaoTabela, coluna.Nome, visivel, chave, filtro);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<string> RetornarColunasVisiveis()
        {
            var repositorio = new SqlClient.Repositorio();
            List<string> colVisiveisList = new List<string>();
            var id = repositorio.SelecionarIdConfiguracaoTabela(Database, Schema, Nome);

            if (id > 0)
            {
                colVisiveisList = repositorio.SelecionarColunasVisiveis(id);
            }


            return colVisiveisList;
        }

        public List<string> RetornarColunasFiltro()
        {
            var repositorio = new SqlClient.Repositorio();
            List<string> colFiltroList = new List<string>();
            var id = repositorio.SelecionarIdConfiguracaoTabela(Database, Schema, Nome);

            if (id > 0)
            {
                colFiltroList = repositorio.SelecionarColunasFiltro(id);
            }


            return colFiltroList;
        }

        public int RetornarIdConfiguracaoTabela()
        {
            var repositorio = new SqlClient.Repositorio();
            return repositorio.SelecionarIdConfiguracaoTabela(Database, Schema, Nome);
        }
    }
}
