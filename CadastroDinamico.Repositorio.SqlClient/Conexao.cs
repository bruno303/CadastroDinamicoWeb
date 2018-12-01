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
        public DataTable RetornarDados(string query)
        {
            DataTable retorno = new DataTable();
            try
            {
                using (SqlConnection conexao = new SqlConnection(RetornarStringConexao()))
                {
                    conexao.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conexao))
                    {
                        adapter.Fill(retorno);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }

        public DataTable RetornarDados(string query, int timeOut)
        {
            DataTable retorno = new DataTable();
            try
            {
                var strConnection = RetornarStringConexao() + "Connection Timeout=" + timeOut.ToString() + ";";
                using (SqlConnection conexao = new SqlConnection(strConnection))
                {
                    conexao.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conexao))
                    {
                        adapter.SelectCommand.CommandTimeout = timeOut;
                        adapter.Fill(retorno);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }

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
        public int ExecutarQuery(string query)
        {
            int linhasAfetadas = 0;
            try
            {
                using (SqlConnection conexao = new SqlConnection(RetornarStringConexao()))
                {
                    conexao.Open();
                    using (SqlCommand command = new SqlCommand(query, conexao))
                    {
                        command.CommandType = CommandType.Text;
                        linhasAfetadas = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return linhasAfetadas;
        }

        public async Task<int> ExecutarQueryAsync(string query)
        {
            int linhasAfetadas = 0;
            try
            {
                using (SqlConnection conexao = new SqlConnection(RetornarStringConexao()))
                {
                    await conexao.OpenAsync();
                    using (SqlCommand command = new SqlCommand(query, conexao))
                    {
                        command.CommandType = CommandType.Text;
                        linhasAfetadas = await command.ExecuteNonQueryAsync();
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
