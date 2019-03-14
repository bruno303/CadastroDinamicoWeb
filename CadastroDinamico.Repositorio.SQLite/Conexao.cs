using System;
using System.IO;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Data;

namespace CadastroDinamico.Repositorio.SQLite
{
    internal class Conexao
    {
        private readonly string _connectionString;
        private readonly string _databaseFileName;

        public Conexao()
        {
            _databaseFileName = AppDomain.CurrentDomain.BaseDirectory + "CadDinamicoDb.sqlite";
            _connectionString = $"DataSource={_databaseFileName};";
        }

        public bool RetornaBaseExistente()
        {
            return File.Exists(_databaseFileName);
        }

        public void CriarBancoDados()
        {
            SQLiteConnection.CreateFile(_databaseFileName);
        }

        public async Task<DataTable> RetornarDadosAsync(string query)
        {
            DataTable retorno = new DataTable();
            try
            {
                using (var conexao = new SQLiteConnection(_connectionString))
                {
                    await conexao.OpenAsync();
                    using (var adapter = new SQLiteDataAdapter(query, conexao))
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

        public async Task<int> ExecutarQueryAsync(string query)
        {
            int linhasAfetadas = 0;
            try
            {
                using (var conexao = new SQLiteConnection(_connectionString))
                {
                    await conexao.OpenAsync();
                    using (var command = new SQLiteCommand(query, conexao))
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
    }
}
