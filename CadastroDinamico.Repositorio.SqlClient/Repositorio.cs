using CadastroDinamico.Dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dir = System.IO.Directory;

namespace CadastroDinamico.Repositorio.SqlClient
{
    public class Repositorio
    {
        private const string DATABASE_NAME = "CAD_DINAMICO";

        public async void CriarObjetosAplicacao(string database, string ansiNullCmd, string quotedIdentCmd)
        {
            var arquivo = new Utils.Arquivo();
            var conexao = new Conexao(RetornarConnectionString(database));

            var files = arquivo.RetornarArquivosDiretorio(Dir.GetParent(Dir.GetCurrentDirectory()) + "\\" + "CadastroDinamico.Dominio\\Scripts");
            foreach (var file in files)
            {
                string texto = arquivo.LerArquivo(file.FullName);
                if (!string.IsNullOrEmpty(texto))
                {
                    await conexao.ExecutarQueryAsync(texto);
                }
            }
        }

        public bool ExisteDatabase(string database)
        {
            var conexao = new Conexao(RetornarConnectionString());
            try
            {
                return conexao.RetornarDados("SELECT database_id FROM sys.databases WITH(NOLOCK) WHERE name = '" + database + "'").Rows.Count > 0;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public void CriarDatabase(string database)
        {
            var conexao = new Conexao(RetornarConnectionString());
            try
            {
                conexao.ExecutarQuery("CREATE DATABASE " + database);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string RetornarConnectionString()
        {
            ConfiguradorBancoDados configurador = new ConfiguradorBancoDados();
            BancoDados configuracao = configurador.RetornarConfiguracaoBancoDados();
            return configuracao.ToString();
        }

        public string RetornarConnectionString(string database)
        {
            ConfiguradorBancoDados configurador = new ConfiguradorBancoDados();
            BancoDados configuracao = configurador.RetornarConfiguracaoBancoDados();
            configuracao.Database = database;
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
                dados = conexao.RetornarDados(query, 7);
                if (dados.Rows.Count > 0)
                {
                    for (int contador = 0; contador < dados.Rows.Count; contador++)
                    {
                        databases.Add(new Database() { Nome = dados.Rows[contador]["NAME"].ToString() });
                    }
                }
            }
            catch (Exception)
            {
                throw;
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
                string query = " EXEC " + DATABASE_NAME + ".DBO.PRC_SEL_SCHEMAS '" + database + "'";
                dados = conexao.RetornarDados(query);
                if (dados.Rows.Count > 0)
                {
                    for (int contador = 0; contador < dados.Rows.Count; contador++)
                    {
                        schemas.Add(new Schema() { Nome = dados.Rows[contador]["NAME"].ToString() });
                    }
                }
            }
            catch (Exception)
            {
                throw;
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
                string query = string.Format(" EXEC " + DATABASE_NAME + ".DBO.PRC_SEL_TABELAS '{0}', '{1}' ", database, schema);
                dados = conexao.RetornarDados(query);
                if (dados.Rows.Count > 0)
                {
                    for (int contador = 0; contador < dados.Rows.Count; contador++)
                    {
                        tabelas.Add(new Tabela() { Nome = dados.Rows[contador]["TABLE_NAME"].ToString() });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return tabelas;
        }

        public string SelecionarColunasFiltro(string database, string schema, string tabela)
        {
            DataTable dados = null;
            string colFiltro = string.Empty;
            int id = 0;
            Conexao conexao = new Conexao(RetornarConnectionString());

            try
            {
                string query = string.Format(" EXEC " + DATABASE_NAME + ".DBO.PRC_SEL_ID_CONFIGURACAO_TABELA '{0}', '{1}', '{2}' ", database, schema, tabela);
                dados = conexao.RetornarDados(query);
                if (dados.Rows.Count > 0)
                {
                    id = Convert.ToInt32(dados.Rows[0][0].ToString());
                }
                colFiltro = SelecionarColunasVisiveis(id);
            }
            catch (Exception)
            {
                throw;
            }
            return colFiltro;
        }

        public string SelecionarColunasFiltro(int id)
        {
            DataTable dados = null;
            string colFiltro = string.Empty;
            Conexao conexao = new Conexao(RetornarConnectionString());

            try
            {
                string query = string.Format(" EXEC {0}.DBO.PRC_SEL_CONFIGURACAO_TABELA {1} ", DATABASE_NAME, id);
                dados = conexao.RetornarDados(query);
                if (dados.Rows.Count > 0)
                {
                    colFiltro = dados.Rows[0][6].ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return colFiltro;
        }

        public List<Coluna> RetornarColunas(string database, string schema, string tabela)
        {
            DataTable dados = null;
            List<Coluna> colunas = new List<Coluna>();
            Conexao conexao = new Conexao(RetornarConnectionString());

            try
            {
                string query = string.Format(" EXEC " + DATABASE_NAME + ".DBO.PRC_SEL_DADOS_TABELA '{0}', '{1}', '{2}' ", tabela, schema, database);
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
            catch (Exception)
            {
                throw;
            }
            return colunas;
        }

        public int SelecionarIdConfiguracaoTabela(string database, string schema, string tabela)
        {
            DataTable dados = null;
            int codigo = 0;
            Conexao conexao = new Conexao(RetornarConnectionString());

            try
            {
                string query = string.Format(" EXEC " + DATABASE_NAME + ".DBO.PRC_SEL_ID_CONFIGURACAO_TABELA '{0}', '{1}', '{2}' ", database, schema, tabela);
                dados = conexao.RetornarDados(query);
                if (dados.Rows.Count > 0)
                {
                    codigo = Convert.ToInt32(dados.Rows[0][0].ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }
            return codigo;
        }

        public string SelecionarColunasVisiveis(string database, string schema, string tabela)
        {
            DataTable dados = null;
            string colVisiveis = string.Empty;
            int id = 0;
            Conexao conexao = new Conexao(RetornarConnectionString());

            try
            {
                string query = string.Format(" EXEC " + DATABASE_NAME + ".DBO.PRC_SEL_ID_CONFIGURACAO_TABELA '{0}', '{1}', '{2}' ", database, schema, tabela);
                dados = conexao.RetornarDados(query);
                if (dados.Rows.Count > 0)
                {
                    id = Convert.ToInt32(dados.Rows[0][0].ToString());
                }
                colVisiveis = SelecionarColunasVisiveis(id);
            }
            catch (Exception)
            {
                throw;
            }
            return colVisiveis;
        }

        public string SelecionarColunasVisiveis(int id)
        {
            DataTable dados = null;
            string colVisiveis = string.Empty;
            Conexao conexao = new Conexao(RetornarConnectionString());

            try
            {
                string query = string.Format(" EXEC {0}.DBO.PRC_SEL_CONFIGURACAO_TABELA {1} ", DATABASE_NAME, id);
                dados = conexao.RetornarDados(query);
                if (dados.Rows.Count > 0)
                {
                    colVisiveis = dados.Rows[0][4].ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return colVisiveis;
        }

        public string SelecionarColunasChaveEstrangeira(int id)
        {
            DataTable dados = null;
            string colVisiveis = string.Empty;
            Conexao conexao = new Conexao(RetornarConnectionString());

            try
            {
                string query = string.Format(" EXEC " + DATABASE_NAME + ".DBO.PRC_SEL_CONFIGURACAO_TABELA {0} ", id);
                dados = conexao.RetornarDados(query);
                if (dados.Rows.Count > 0)
                {
                    colVisiveis = dados.Rows[0][5].ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return colVisiveis;
        }

        public string SalvarConfiguracoesTabela(int id, string database, string schema, string table, string colVisiveis, string colChave, string colFiltros)
        {
            var retorno = string.Empty;
            Conexao conexao = new Conexao(RetornarConnectionString());

            try
            {
                string query = string.Format(" EXEC " + DATABASE_NAME + ".DBO.PRC_IU_CONFIGURACAO_TABELA {0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}' ",
                    id, database, schema, table, colVisiveis, colChave, colFiltros ?? string.Empty);
                conexao.ExecutarQuery(query);
            }
            catch (Exception ex)
            {
                retorno = ex.Message;
            }
            return retorno;
        }

        public List<TabelaEstrangeira> SelectTabela(string chavePrimaria, string descricao, string tabela, string schema, string database)
        {
            var retorno = new List<TabelaEstrangeira>();
            Conexao conexao = new Conexao(RetornarConnectionString());

            try
            {
                var consulta = conexao.RetornarDados(string.Format("SELECT {0}, {1} FROM {2}.{3}.{4}", chavePrimaria, descricao, database, schema, tabela));
                if ((consulta?.Rows?.Count ?? 0) > 0)
                {
                    for (int cont = 0; cont < consulta.Rows.Count; cont++)
                    {
                        retorno.Add(new TabelaEstrangeira()
                        {
                            ChavePrimaria = Convert.ToInt32(consulta.Rows[cont][0].ToString()),
                            Descricao = consulta.Rows[cont][1].ToString()
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return retorno;
        }

        public List<Coluna> RetornarColunasChavePrimariaTabela(string tabela, string schema, string database)
        {
            List<Coluna> colChavePrimaria = null;
            colChavePrimaria = RetornarColunas(database, schema, tabela).Where(p => p.IsChavePrimaria).ToList();
            return colChavePrimaria;
        }

        public string SelecionarDescricaoChaveEstrangeiraConfiguracaoTabela(string database, string schema, string tabela, string chavePrimaria)
        {
            DataTable dados = null;
            string colunas = string.Empty;
            string retorno = string.Empty;
            Conexao conexao = new Conexao(RetornarConnectionString());

            try
            {
                int id = SelecionarIdConfiguracaoTabela(database, schema, tabela);
                string query = string.Format(" EXEC {0}.DBO.PRC_SEL_CONFIGURACAO_TABELA {1} ", DATABASE_NAME, id);
                dados = conexao.RetornarDados(query);
                if (dados.Rows.Count > 0)
                {
                    colunas = dados.Rows[0][5].ToString();
                    var colunasSplit = colunas.Split(";");
                    for (int cont = 0; cont < colunasSplit.Length && retorno == string.Empty; cont++)
                    {
                        if (colunasSplit[cont].Split(":")[0].Equals(chavePrimaria))
                        {
                            retorno = colunasSplit[cont].Split(":")[1];
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return retorno;
        }
    }
}
