using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CadastroDinamico.Repositorio.SqlClient
{
    internal class Conexao
    {
        private readonly string _connectionString;

        public Conexao()
        {
            _connectionString = string.Empty;
        }

        public Conexao(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Retornar dados
        public async Task<DataTable> RetornarDadosAsync(string query)
        {
            DataTable retorno = new DataTable();
            try
            {
                using (SqlConnection conexao = new SqlConnection(RetornarStringConexao()))
                {
                    await conexao.OpenAsync();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conexao))
                    {
                        await Task.Factory.StartNew(() =>
                        {
                            adapter.Fill(retorno);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }

        public async Task<DataTable> RetornarDadosAsync(string query, int timeOut)
        {
            DataTable retorno = new DataTable();
            try
            {
                var strConnection = RetornarStringConexao() + "Connection Timeout=" + timeOut.ToString() + ";";
                using (SqlConnection conexao = new SqlConnection(strConnection))
                {
                    await conexao.OpenAsync();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conexao))
                    {
                        await Task.Factory.StartNew(() =>
                        {
                            adapter.Fill(retorno);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }
        #endregion

        #region Executar query
        public async Task<int> ExecutarQueryAsync(string query, bool usarTransacao = false)
        {
            int linhasAfetadas = 0;
            try
            {
                using (SqlConnection conexao = new SqlConnection(RetornarStringConexao()))
                {
                    await conexao.OpenAsync();
                    if (usarTransacao)
                    {
                        using (var transacao = conexao.BeginTransaction(IsolationLevel.ReadCommitted))
                        {
                            try
                            {
                                using (SqlCommand command = new SqlCommand(query, conexao))
                                {
                                    command.CommandType = CommandType.Text;
                                    command.Transaction = transacao;
                                    linhasAfetadas = await command.ExecuteNonQueryAsync();
                                }
                                transacao.Commit();
                            }
                            catch (Exception ex)
                            {
                                transacao.Rollback();
                                throw ex;
                            }
                        }
                    }
                    else
                    {
                        using (SqlCommand command = new SqlCommand(query, conexao))
                        {
                            command.CommandType = CommandType.Text;
                            linhasAfetadas = await command.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return linhasAfetadas;
        }
        #endregion

        #region Utils
        private string RetornarStringConexao()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("Erro em Conexao. Mensagem: Não foram carregadas configurações válidas para o banco de dados.");
            }
            else
            {
                return _connectionString;
            }
        }
        #endregion


    }
}
