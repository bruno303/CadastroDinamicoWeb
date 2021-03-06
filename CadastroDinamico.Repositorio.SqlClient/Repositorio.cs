﻿using CadastroDinamico.Dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dir = System.IO.Directory;

namespace CadastroDinamico.Repositorio.SqlClient
{
    public class Repositorio
    {
        public int IdServidor { get; set; }

        private const string DATABASE_NAME = "CAD_DINAMICO";

        public Repositorio(int idServidor)
        {
            IdServidor = idServidor;
        }

        public async Task CriarObjetosAplicacaoAsync(string database, string ansiNullCmd, string quotedIdentCmd)
        {
            var arquivo = new Utils.Arquivo();
            var conexao = new Conexao(await RetornarConnectionStringAsync(DATABASE_NAME));

            var files = await arquivo.RetornarArquivosDiretorioAsync(Dir.GetParent(Dir.GetCurrentDirectory()) + "\\" + "CadastroDinamico.Dominio\\Scripts");
            foreach (var file in files)
            {
                string texto = await arquivo.LerArquivoAsync(file.FullName);
                if (!string.IsNullOrEmpty(texto))
                {
                    await conexao.ExecutarQueryAsync(texto);
                }
            }
        }

        public async Task<bool> ExisteDatabaseAsync(string database)
        {
            var conexao = new Conexao(await RetornarConnectionStringAsync());
            try
            {
                return (await conexao.RetornarDadosAsync("SELECT database_id FROM sys.databases WITH(NOLOCK) WHERE name = '" + database + "'"))?.Rows?.Count > 0;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public async Task CriarDatabaseAsync(string database)
        {
            var conexao = new Conexao(await RetornarConnectionStringAsync());
            try
            {
                await conexao.ExecutarQueryAsync("CREATE DATABASE " + database);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> RetornarConnectionStringAsync(string database = null)
        {
            return await ConfiguradorBancoDados.RetornarConnectionStringAsync(IdServidor, database);
        }

        public async Task<List<Database>> RetornarDatabasesAsync()
        {
            DataTable dados = null;
            List<Database> databases = new List<Database>();
            Conexao conexao = new Conexao(await RetornarConnectionStringAsync());

            try
            {
                string query = " EXEC " + DATABASE_NAME + ".DBO.PRC_SEL_DATABASES";
                dados = await conexao.RetornarDadosAsync(query, 7);
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

        public async Task<List<Schema>> RetornarSchemasAsync(string database)
        {
            DataTable dados = null;
            List<Schema> schemas = new List<Schema>();
            Conexao conexao = new Conexao(await RetornarConnectionStringAsync());

            try
            {
                string query = " EXEC " + DATABASE_NAME + ".DBO.PRC_SEL_SCHEMAS '" + database + "'";
                dados = await conexao.RetornarDadosAsync(query);
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

        public async Task<List<Tabela>> RetornarTabelasAsync(string database, string schema)
        {
            DataTable dados = null;
            List<Tabela> tabelas = new List<Tabela>();
            Conexao conexao = new Conexao(await RetornarConnectionStringAsync());

            try
            {
                string query = string.Format(" EXEC " + DATABASE_NAME + ".DBO.PRC_SEL_TABELAS '{0}', '{1}' ", database, schema);
                dados = await conexao.RetornarDadosAsync(query);
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

        public async Task<List<string>> SelecionarColunasFiltroAsync(string database, string schema, string tabela)
        {
            DataTable dados = null;
            string colFiltro = string.Empty;
            int id = 0;
            List<string> colunas = new List<string>();
            Conexao conexao = new Conexao(await RetornarConnectionStringAsync());

            try
            {
                string query = string.Format(" EXEC " + DATABASE_NAME + ".DBO.PRC_SEL_ID_CONFIGURACAO_TABELA '{0}', '{1}', '{2}' ", database, schema, tabela);
                dados = await conexao.RetornarDadosAsync(query);
                if (dados.Rows.Count > 0)
                {
                    id = Convert.ToInt32(dados.Rows[0][0].ToString());
                }
                colunas = await SelecionarColunasFiltroAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
            return colunas;
        }

        public async Task<List<string>> SelecionarColunasFiltroAsync(int id)
        {
            DataTable dados = null;
            List<string> colunas = new List<string>();
            Conexao conexao = new Conexao(await RetornarConnectionStringAsync());

            try
            {
                string query = string.Format(" EXEC {0}.DBO.PRC_SEL_COLUNAS_FILTRO {1} ", DATABASE_NAME, id);
                dados = await conexao.RetornarDadosAsync(query);
                if (dados.Rows.Count > 0)
                {
                    for (int i = 0; i < dados.Rows.Count; i++)
                    {
                        colunas.Add(dados.Rows[i][0].ToString());
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return colunas;
        }

        public async Task<List<Coluna>> RetornarColunasAsync(string database, string schema, string tabela)
        {
            DataTable dados = null;
            List<Coluna> colunas = new List<Coluna>();
            Conexao conexao = new Conexao(await RetornarConnectionStringAsync());

            try
            {
                string query = string.Format(" EXEC " + DATABASE_NAME + ".DBO.PRC_SEL_DADOS_TABELA '{0}', '{1}', '{2}' ", tabela, schema, database);
                dados = await conexao.RetornarDadosAsync(query);
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
                            ColunaReferenciada = dados.Rows[contador]["COLUNA_REFERENCIADA"].ToString(),
                            IsIdentity = Convert.ToInt32(dados.Rows[contador]["IS_IDENTITY"].ToString()).Equals(1)
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

        public async Task<int> SelecionarIdConfiguracaoTabelaAsync(string database, string schema, string tabela)
        {
            DataTable dados = null;
            int codigo = 0;
            Conexao conexao = new Conexao(await RetornarConnectionStringAsync());

            try
            {
                string query = string.Format(" EXEC " + DATABASE_NAME + ".DBO.PRC_SEL_ID_CONFIGURACAO_TABELA '{0}', '{1}', '{2}' ", database, schema, tabela);
                dados = await conexao.RetornarDadosAsync(query);
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

        public async Task<int> SelecionarIdConfiguracaoTabelaColunaAsync(string database, string schema, string tabela, string coluna)
        {
            DataTable dados = null;
            int codigo = 0;
            Conexao conexao = new Conexao(await RetornarConnectionStringAsync());

            try
            {
                string query = string.Format(" EXEC " + DATABASE_NAME + ".DBO.PRC_SEL_ID_CONFIGURACAO_TABELA_COLUNA '{0}', '{1}', '{2}', '{3}' ",
                    database, schema, tabela, coluna);
                dados = await conexao.RetornarDadosAsync(query);
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

        public async Task<List<string>> SelecionarColunasVisiveisAsync(string database, string schema, string tabela)
        {
            DataTable dados = null;
            List<string> colunas = new List<string>();
            int id = 0;
            Conexao conexao = new Conexao(await RetornarConnectionStringAsync());

            try
            {
                string query = string.Format(" EXEC " + DATABASE_NAME + ".DBO.PRC_SEL_ID_CONFIGURACAO_TABELA '{0}', '{1}', '{2}' ", database, schema, tabela);
                dados = await conexao.RetornarDadosAsync(query);
                if (dados.Rows.Count > 0)
                {
                    id = Convert.ToInt32(dados.Rows[0][0].ToString());
                    colunas = await SelecionarColunasVisiveisAsync(id);
                }
                else
                {
                    colunas = (await RetornarColunasAsync(database, schema, tabela)).Select(c => c.Nome).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return colunas;
        }

        public async Task<List<string>> SelecionarColunasVisiveisAsync(int id)
        {
            DataTable dados = null;
            List<string> colunas = new List<string>();
            Conexao conexao = new Conexao(await RetornarConnectionStringAsync());

            try
            {
                string query = string.Format(" EXEC {0}.DBO.PRC_SEL_COLUNAS_VISIVEIS {1} ", DATABASE_NAME, id);
                dados = await conexao.RetornarDadosAsync(query);
                if (dados.Rows.Count > 0)
                {
                    for (int i = 0; i < dados.Rows.Count; i++)
                    {
                        colunas.Add(dados.Rows[i][0].ToString());
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return colunas;
        }

        public async Task<List<string>> SelecionarColunasChaveEstrangeiraAsync(int id)
        {
            DataTable dados = null;
            List<string> colunas = new List<string>();
            Conexao conexao = new Conexao(await RetornarConnectionStringAsync());

            try
            {
                string query = string.Format(" EXEC " + DATABASE_NAME + ".DBO.PRC_SEL_COLUNAS_CHAVE {0} ", id);
                dados = await conexao.RetornarDadosAsync(query);
                if (dados.Rows.Count > 0)
                {
                    for (int i = 0; i < dados.Rows.Count; i++)
                    {
                        colunas.Add(dados.Rows[i][0].ToString() + ":" + dados.Rows[i][1].ToString());
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return colunas;
        }

        public async Task<string> SalvarConfiguracoesTabelaAsync(int id, string database, string schema, string table)
        {
            var retorno = string.Empty;
            Conexao conexao = new Conexao(await RetornarConnectionStringAsync())
            {
                UsarTransacao = await ConfiguradorBancoDados.RetornarUsaTransacao(IdServidor)
            };

            try
            {
                string query = string.Format(" EXEC " + DATABASE_NAME + ".DBO.PRC_IU_CONFIGURACAO_TABELA {0}, '{1}', '{2}', '{3}' ",
                    id, database, schema, table);
                await conexao.ExecutarQueryAsync(query);
            }
            catch (Exception ex)
            {
                retorno = ex.Message;
            }
            return retorno;
        }

        public async Task SalvarConfiguracaoColunaAsync(int idConfiguracaoTabelaColuna, int idConfiguracaoTabela, string coluna,
            bool visivel, string colunaDescricao, bool filtro)
        {
            Conexao conexao = new Conexao(await RetornarConnectionStringAsync())
            {
                UsarTransacao = await ConfiguradorBancoDados.RetornarUsaTransacao(IdServidor)
            };

            try
            {
                var query = string.Format("EXEC {0}.DBO.PRC_IU_CONFIGURACAO_TABELA_COLUNA {1}, {2}, '{3}', {4}, '{5}', {6} ", 
                    DATABASE_NAME, idConfiguracaoTabelaColuna, idConfiguracaoTabela, coluna, visivel ? 1 : 0, colunaDescricao,
                    filtro ? 1 : 0);
                await conexao.ExecutarQueryAsync(query);
            }
             catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<TabelaEstrangeira>> SelectTabelaAsync(string chavePrimaria, string descricao, string tabela, string schema, string database)
        {
            var retorno = new List<TabelaEstrangeira>();
            Conexao conexao = new Conexao(await RetornarConnectionStringAsync());

            try
            {
                var consulta = await conexao.RetornarDadosAsync(string.Format("SELECT {0}, {1} FROM {2}.{3}.{4}", chavePrimaria, descricao, database, schema, tabela));
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

        public async Task<List<Coluna>> RetornarColunasChavePrimariaTabelaAsync(string tabela, string schema, string database)
        {
            List<Coluna> colChavePrimaria = null;
            colChavePrimaria = (await RetornarColunasAsync(database, schema, tabela)).Where(p => p.IsChavePrimaria && !p.IsChaveEstrangeira).ToList();
            return colChavePrimaria;
        }

        public async Task<string> SelecionarDescricaoChaveEstrangeiraConfiguracaoTabelaAsync(string database, string schema, string tabela, string chavePrimaria)
        {
            DataTable dados = null;
            List<string> colunas = new List<string>();
            string retorno = string.Empty;
            Conexao conexao = new Conexao(await RetornarConnectionStringAsync());

            try
            {
                int id = await SelecionarIdConfiguracaoTabelaAsync(database, schema, tabela);
                string query = string.Format(" EXEC {0}.DBO.PRC_SEL_COLUNAS_CHAVE {1} ", DATABASE_NAME, id);
                dados = await conexao.RetornarDadosAsync(query);
                if (dados.Rows.Count > 0)
                {
                    for (int i = 0; i < dados.Rows.Count; i++)
                    {
                        colunas.Add(dados.Rows[i][0].ToString() + ":" + dados.Rows[i][1].ToString());
                    }
                    for (int cont = 0; cont < colunas.Count && retorno == string.Empty; cont++)
                    {
                        if (colunas[cont].Split(":")[0].Equals(chavePrimaria))
                        {
                            retorno = colunas[cont].Split(":")[1];
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

        public async Task<MatrizValores> RetornarValoresQueryAsync(string query)
        {
            DataTable dados = null;
            var conexao = new Conexao(await RetornarConnectionStringAsync());

            dados = await conexao.RetornarDadosAsync(query);

            var retorno = new MatrizValores(dados.Rows.Count, dados.Columns.Count - 1);

            if (dados?.Rows?.Count > 0)
            {
                
                for (int lin = 0; lin < dados.Rows.Count; lin++)
                {
                    for (int col = 1; col < dados.Columns.Count; col++)
                    {
                        retorno.SetValor(lin, col, dados.Rows[lin][col].ToString(), dados.Rows[lin][0].ToString());
                    }
                }
            }

            return retorno;
        }

        public async Task<List<object>> RetornarValoresAsync(string query)
        {
            DataTable dados = null;
            var conexao = new Conexao(await RetornarConnectionStringAsync());

            dados = await conexao.RetornarDadosAsync(query);

            var retorno = new List<object>();

            if (dados?.Rows?.Count > 0)
            {
                for (int cont = 1; cont < dados.Columns.Count; cont++)
                {
                    retorno.Add(dados.Rows[0][cont].ToString());
                }
            }

            return retorno;
        }

        #region Alteração / Deleção
        public async Task<string> AlterarIncluirValoresAsync(string query, DadosLog dadosLog)
        {
            try
            {
                var conexao = new Conexao(await RetornarConnectionStringAsync())
                {
                    UsarTransacao = await ConfiguradorBancoDados.RetornarUsaTransacao(IdServidor)
                };

                await GravarLogExecucao(dadosLog);
                var result = await conexao.ExecutarQueryAsync(query);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return string.Empty;
        }

        public async Task<string[]> DeletarRegistroAsync(string query, DadosLog dadosLog)
        {
            string[] retorno = { string.Empty, string.Empty, string.Empty };
            try
            {
                var conexao = new Conexao(await RetornarConnectionStringAsync())
                {
                    UsarTransacao = await ConfiguradorBancoDados.RetornarUsaTransacao(IdServidor),
                };
                await GravarLogExecucao(dadosLog);
                await conexao.ExecutarQueryAsync(query);
            }
            catch (Exception ex)
            {
                retorno[0] = "Erro ao deletar o registro.";
                retorno[1] = ex.Message;
                retorno[2] = query;
            }
            return retorno;
        }
        #endregion

        public async Task<int> RetornarProximaChavePrimaria(string database, string schema, string tabela, string colPk)
        {
            var retorno = 1;

            var query = $"SELECT COALESCE(MAX({colPk}), 0) + 1 AS PROXIMO FROM {database}.{schema}.{tabela} WITH(NOLOCK)";

            DataTable dados = null;
            var conexao = new Conexao(await RetornarConnectionStringAsync());

            dados = await conexao.RetornarDadosAsync(query);
            if (dados.Rows.Count > 0)
            {
                retorno = Convert.ToInt32(dados.Rows[0][0].ToString());
            }
            return retorno;
        }

        public async Task<object[,]> RetornarValoresMultilinha(string query)
        {
            DataTable dados = null;
            var conexao = new Conexao(await RetornarConnectionStringAsync());

            dados = await conexao.RetornarDadosAsync(query);

            var retorno = new object[dados.Rows.Count, dados.Columns.Count];

            if (dados?.Rows?.Count > 0)
            {
                for (int lin = 0; lin < dados.Rows.Count; lin++)
                {
                    for (int col = 0; col < dados.Columns.Count; col++)
                    {
                        retorno[lin, col] = dados.Rows[lin][col].ToString();
                    }
                }
            }

            return retorno;
        }

        private async Task GravarLogExecucao(DadosLog dados)
        {
            if (await ConfiguradorBancoDados.RetornarGravaLog(IdServidor))
            {
                var queryLog = new StringBuilder();
                var conexao = new Conexao(await RetornarConnectionStringAsync(DATABASE_NAME));

                queryLog.Append("EXEC PRC_INS_LOG ");
                queryLog.Append($" @STR_BANCO_DADOS = '{dados.Database}', ");
                queryLog.Append($" @STR_ESQUEMA = '{dados.Schema}', ");
                queryLog.Append($" @STR_TABELA = '{dados.Tabela}', ");
                queryLog.Append($" @STR_USUARIO = '{dados.Usuario}', ");
                queryLog.Append($" @STR_METODO = '{dados.Metodo}', ");
                queryLog.Append($" @STR_QUERY = '{dados.Query.Replace("'", "''")}'");
                await conexao.ExecutarQueryAsyncNoTransaction(queryLog.ToString());
            }
        }

        public async Task<List<DadosLog>> GetLogs()
        {
            var logData = new List<DadosLog>();
            var query = new StringBuilder();
            var conexao = new Conexao(await RetornarConnectionStringAsync(DATABASE_NAME));

            query.AppendLine(" SELECT L.ID_LOG, B.NOME AS BANCO_DADOS, E.NOME AS ESQUEMA, T.NOME AS TABELA, ");
            query.AppendLine("     L.USUARIO, L.DATA_HORA, L.METODO, L.QUERY_EXECUTADA ");
            query.AppendLine($" FROM LOG L WITH(NOLOCK)");
            query.AppendLine($" INNER JOIN BANCO_DADOS B WITH(NOLOCK) ON L.ID_BANCO_DADOS = B.ID_BANCO_DADOS ");
            query.AppendLine($" INNER JOIN ESQUEMA E WITH(NOLOCK) ON L.ID_ESQUEMA = E.ID_ESQUEMA ");
            query.AppendLine($" INNER JOIN TABELA T WITH(NOLOCK) ON L.ID_TABELA = T.ID_TABELA ");

            var result = await conexao.RetornarDadosAsync(query.ToString());
            if (result.Rows.Count > 0)
            {
                for (int i = 0; i < result.Rows.Count; i++)
                {
                    logData.Add(new DadosLog()
                    {
                        IdLog = Convert.ToInt32(result.Rows[i]["ID_LOG"].ToString()),
                        Database = result.Rows[i]["BANCO_DADOS"].ToString(),
                        Schema = result.Rows[i]["ESQUEMA"].ToString(),
                        Tabela = result.Rows[i]["TABELA"].ToString(),
                        Usuario = result.Rows[i]["USUARIO"].ToString(),
                        DataHora = Convert.ToDateTime(result.Rows[i]["DATA_HORA"].ToString()),
                        Metodo = result.Rows[i]["METODO"].ToString(),
                        Query = result.Rows[i]["QUERY_EXECUTADA"].ToString()
                    });
                }
            }

            return logData;
        }
    }
}
