using CadastroDinamico.Dominio;
using System;
using System.Collections.Generic;
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
                await repositorioSQLite.CriaBaseSQLite();
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

        public async Task<List<Usuario>> RetornarUsuariosAsync()
        {
            try
            {
                return await new Repositorio.SQLite.Repositorio().RetornarUsuarios();
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<Usuario> RetornarUsuarioAsync(int idUsuario)
        {
            try
            {
                return await new Repositorio.SQLite.Repositorio().RetornarUsuario(idUsuario);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeletarUsuarioAsync(int idUsuario)
        {
            try
            {
                await new Repositorio.SQLite.Repositorio().DeletarUsuarioAsync(idUsuario);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
