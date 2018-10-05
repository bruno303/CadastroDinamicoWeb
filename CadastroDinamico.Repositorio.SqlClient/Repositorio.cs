using CadastroDinamico.Dominio;
using CadastroDinamico.Repositorio.Interface;
using System;
using System.Collections.Generic;
using System.Data;

namespace CadastroDinamico.Repositorio.SqlClient
{
    public class Repositorio : IRepositorio
    {
        public void CriarDatabaseAplicacao()
        {
            //
        }

        public string RetornarConnectionString()
        {
            ConfiguradorBancoDados configurador = new ConfiguradorBancoDados();
            BancoDados configuracao = configurador.RetornarConfiguracaoBancoDados();
            return configuracao.ToString();
        }

        public List<Database> RetornarDatabases()
        {
            DataTable dados = null;
            List<Database> databases = new List<Database>();
            Conexao conexao = new Conexao(RetornarConnectionString());

            try
            {
                string query = "SELECT NAME FROM MASTER.SYS.DATABASES";
                dados = conexao.RetornarDados(query);
                if (dados.Rows.Count > 0)
                {
                    for (int contador = 0; contador < dados.Rows.Count; contador++)
                    {
                        databases.Add(new Database() { Nome = dados.Rows[contador]["NAME"].ToString() });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return databases;
        }

        public List<Schema> RetornarSchemas(string database)
        {
            DataTable dados = null;
            List<Schema> schemas = new List<Schema>();
            Conexao conexao = new Conexao(RetornarConnectionString());

            try
            {
                string query = " EXEC CAD_DINAMICO.DBO.PRC_SEL_SCHEMAS '" + database + "'";
                dados = conexao.RetornarDados(query);
                if (dados.Rows.Count > 0)
                {
                    for (int contador = 0; contador < dados.Rows.Count; contador++)
                    {
                        schemas.Add(new Schema() { Nome = dados.Rows[contador]["NAME"].ToString() });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return schemas;
        }

        public List<Tabela> RetornarTabelas(string database, string schema)
        {
            DataTable dados = null;
            List<Tabela> tabelas = new List<Tabela>();
            Conexao conexao = new Conexao(RetornarConnectionString());

            try
            {
                string query = string.Format(" EXEC CAD_DINAMICO.DBO.PRC_SEL_TABELAS '{0}', '{1}' ", database, schema);
                dados = conexao.RetornarDados(query);
                if (dados.Rows.Count > 0)
                {
                    for (int contador = 0; contador < dados.Rows.Count; contador++)
                    {
                        tabelas.Add(new Tabela() { Nome = dados.Rows[contador]["TABLE_NAME"].ToString() });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tabelas;
        }

        public List<Coluna> RetornarColunas(string database, string schema, string tabela)
        {
            DataTable dados = null;
            List<Coluna> colunas = new List<Coluna>();
            Conexao conexao = new Conexao(RetornarConnectionString());

            try
            {
                string query = string.Format(" EXEC CAD_DINAMICO.DBO.PRC_SEL_DADOS_TABELA '{0}', '{1}', '{2}' ", tabela, schema, database);
                dados = conexao.RetornarDados(query);
                if (dados.Rows.Count > 0)
                {
                    for (int contador = 0; contador < dados.Rows.Count; contador++)
                    {
                        colunas.Add(new Coluna()
                        {
                            AceitaNull = (dados.Rows[contador]["IS_NULLABLE"].ToString().ToUpper().Equals("YES")),
                            Nome = dados.Rows[contador]["COLUMN_NAME"].ToString(),
                            Posicao = Convert.ToInt32(dados.Rows[contador]["ORDINAL_POSITION"].ToString()),
                            ExpressaoValorDefault = dados.Rows[contador]["COLUMN_DEFAULT"].ToString(),
                            PrecisaoNumerica = dados.Rows[contador]["NUMERIC_PRECISION"].ToString() == string.Empty ? -1 : Convert.ToInt32(dados.Rows[contador]["NUMERIC_PRECISION"].ToString()),
                            Tipo = dados.Rows[contador]["DATA_TYPE"].ToString(),
                            TamanhoMaximo = dados.Rows[contador]["CHARACTER_MAXIMUM_LENGTH"].ToString() != string.Empty ? Convert.ToInt32(dados.Rows[contador]["CHARACTER_MAXIMUM_LENGTH"].ToString()) : 0,
                            PrecisaoNumericaRadix = dados.Rows[contador]["NUMERIC_PRECISION_RADIX"].ToString() != string.Empty ? Convert.ToInt32(dados.Rows[contador]["NUMERIC_PRECISION_RADIX"].ToString()) : 0,
                            IsChavePrimaria = dados.Rows[contador]["IS_PK"].ToString().Equals("1"),
                            IsChaveEstrangeira = dados.Rows[contador]["IS_FK"].ToString().Equals("1"),
                            TabelaReferenciada = dados.Rows[contador]["TABELA_REFERENCIADA"].ToString(),
                            ColunaReferenciada = dados.Rows[contador]["COLUNA_REFERENCIADA"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return colunas;
        }
    }
}
