using CadastroDinamico.Dominio;
using System;
using System.Threading.Tasks;

namespace CadastroDinamico.Repositorio.SqlClient
{
    public class ConfiguradorBancoDados
    {
        public static async Task<string> RetornarConnectionStringAsync(int idServidor, string database = null)
        {
            BancoDados configuracao = new BancoDados();
            var repositorioSQLite = new SQLite.Repositorio();

            try
            {
                var servidor = await repositorioSQLite.RetornarServidorAsync(idServidor);

                configuracao.Servidor = servidor.Hostname;
                configuracao.Instancia = servidor.Instancia;
                configuracao.Database = database;
                configuracao.Usuario = servidor.Usuario;
                configuracao.Senha = servidor.Senha;

                return configuracao.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
