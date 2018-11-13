using CadastroDinamico.Dominio;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
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
        public List<object> Valores { get; set; }
        public List<Coluna> ChavesPrimarias { get; set; }

        private List<string> camposExibir;

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
                Colunas = repositorio.RetornarColunas(Database, Schema, Nome);
                TemChavePrimaria = Colunas.Where(p => p.IsChavePrimaria).FirstOrDefault() != null;
                TemChaveEstrangeira = Colunas.Where(p => p.IsChaveEstrangeira).FirstOrDefault() != null;
                camposExibir = repositorio.SelecionarColunasVisiveis(Database, Schema, Nome).Split(";").ToList();
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
                    if (!camposExibir.Contains(Colunas[cont].Nome))
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

        private SelectList RetornarListaTabelaEstrangeira(string tabela, int itemSelecionado)
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
            catch(Exception)
            {
                throw;
            }
            return new SelectList(itens, chavePrimaria, descricao, itemSelecionado);
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

        private DataTable RetornarAmostraDados(List<string> camposNaoExibir)
        {
            return new DataTable();
            //IConfigurador configurador = new Configurador();
            //ConfiguracaoBancoDados configuracaoBancoDados = configurador.RetornarConfiguracaoBancoDados();
            //Conexao conexao = new Conexao(configuracaoBancoDados);
            //return new Conexao(new Configurador().RetornarConfiguracaoBancoDados()).RetornarDados(RetornarSelect(true));
        }

        public string RetornarSelect(string where, bool amostra = false)
        {
            string query = string.Empty;

            query = "SELECT " + (amostra ? "TOP(1) " : "");
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
    }
}
