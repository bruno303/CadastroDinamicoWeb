using CadastroDinamico.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CadastroDinamico.Core
{
    public class SQLiteCore
    {
        public int IdUsuario { get; set; }

        public async Task<bool> ValidarLoginUsuarioAsync(Usuario usuario)
        {
            IdUsuario = 0;
            bool retorno = false;
            var repositorioSQLite = new Repositorio.SQLite.Repositorio();

            try
            {
                await repositorioSQLite.CriarBaseSQLiteAsync();
                IdUsuario = await repositorioSQLite.ValidarUsuarioSenhaAsync(usuario.Login, usuario.Senha);
                return IdUsuario > 0;
            }
            catch (Exception)
            {
                retorno = false;
            }

            return retorno;
        }

        public async Task<bool> GravarUsuarioAsync(Usuario usuario)
        {
            var repositorioSQLite = new Repositorio.SQLite.Repositorio();
            var retorno = false;

            try
            {
                if (usuario.IdUsuario != 0)
                {
                    await repositorioSQLite.AlterarUsuarioAsync(usuario);
                }
                else
                {
                    await repositorioSQLite.CriarUsuarioAsync(usuario.Nome, usuario.Login, usuario.Senha);
                }
                retorno = true;
            }
            catch (Exception)
            {
                retorno = false;
            }

            return retorno;
        }

        public async Task<bool> GravarServidorAsync(Servidor servidor)
        {
            var repositorioSQLite = new Repositorio.SQLite.Repositorio();
            var retorno = false;

            try
            {
                if (servidor.IdServidor != 0)
                {
                    await repositorioSQLite.AlterarServidorAsync(servidor);
                }
                else
                {
                    await repositorioSQLite.CriarServidorAsync(servidor);
                }
                retorno = true;
            }
            catch (Exception)
            {
                retorno = false;
            }

            return retorno;
        }

        public async Task DeletarServidorAsync(int idServidor)
        {
            try
            {
                await new Repositorio.SQLite.Repositorio().DeletarServidorAsync(idServidor);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeveGravarSessaoServidor()
        {
            var repositorioSQLite = new Repositorio.SQLite.Repositorio();
            return (await repositorioSQLite.RetornarServidoresAsync()).Count == 1;
        }

        public async Task<int> RetornaIdUnicoServidor()
        {
            var repositorioSQLite = new Repositorio.SQLite.Repositorio();
            var servidor = await repositorioSQLite.RetornarServidoresAsync();
            return servidor.FirstOrDefault().IdServidor;
        }

        public async Task<string> RetornarNomeUsuario(int idUsuario)
        {
            var repositorioSQLite = new Repositorio.SQLite.Repositorio();
            var usuario = await repositorioSQLite.RetornarUsuarioAsync(idUsuario);
            return usuario.Nome;
        }
    }
}
