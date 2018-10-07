using CadastroDinamico.Dominio;
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

        private readonly List<string> CamposNaoExibir;

        #region Construtores
        public TabelaCore(string tabela, string schema, string database)
        {
            Nome = tabela;
            Schema = schema;
            Database = database;
        }

        public TabelaCore(string tabela, string schema)
        {
            Nome = tabela;
            Schema = schema;
            CamposNaoExibir = new List<string>();
        }

        public TabelaCore(string tabela)
        {
            Nome = tabela;
            CamposNaoExibir = new List<string>();
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
                QuantidadeCampos = Colunas.Count;
                TemChavePrimaria = (Colunas.Where(p => p.IsChavePrimaria).FirstOrDefault() != null);
                TemChaveEstrangeira = (Colunas.Where(p => p.IsChaveEstrangeira).FirstOrDefault() != null);
            }
            catch (Exception ex)
            {
                retorno = ex.Message;
            }

            return retorno;
        }

        public DataTable RetornarAmostraDados(List<string> camposNaoExibir)
        {
            return new DataTable();
            //IConfigurador configurador = new Configurador();
            //ConfiguracaoBancoDados configuracaoBancoDados = configurador.RetornarConfiguracaoBancoDados();
            //Conexao conexao = new Conexao(configuracaoBancoDados);
            //return new Conexao(new Configurador().RetornarConfiguracaoBancoDados()).RetornarDados(RetornarSelect(true));
        }

        public string RetornarSelect(bool amostra = false)
        {
            string query = string.Empty;

            query = "SELECT " + (amostra ? "TOP(10) " : "");
            foreach (var coluna in Colunas)
            {
                query += coluna.Nome + ", ";
            }
            query = query.Remove(query.Length - 2, 2);
            query += string.Format(" FROM {0} WITH(NOLOCK)", Nome);

            return query;
        }
    }
}
