using CadastroDinamico.Dominio;
using System;
using System.Threading.Tasks;

namespace CadastroDinamico.Repositorio.SqlClient
{
    public class ConfiguradorBancoDados
    {
        public static async Task<BancoDados> RetornarConfiguracaoBancoDados(int idServidor, string database = null)
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
                configuracao.GravarLog = servidor.GravarLog;
                configuracao.UsarTransacao = servidor.UsarTransacao;

                return configuracao;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<bool> RetornarGravaLog(int idServidor)
        {
            try
            {
                BancoDados configuracao = await RetornarConfiguracaoBancoDados(idServidor);
                return configuracao.GravarLog;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<bool> RetornarUsaTransacao(int idServidor)
        {
            try
            {
                BancoDados configuracao = await RetornarConfiguracaoBancoDados(idServidor);
                return configuracao.UsarTransacao;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<string> RetornarConnectionStringAsync(int idServidor, string database = null)
        {
            try
            {
                BancoDados configuracao = await RetornarConfiguracaoBancoDados(idServidor, database);
                return configuracao.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
