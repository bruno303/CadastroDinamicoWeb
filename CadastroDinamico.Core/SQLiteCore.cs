using CadastroDinamico.Dominio;
using System;
using System.Threading.Tasks;

namespace CadastroDinamico.Core
{
    public class SQLiteCore
    {
        public int IdUsuario { get; set; }

        public async Task<bool> ValidarLoginUsuario(string usuario, string senha)
        {
            IdUsuario = 0;
            bool retorno = false;
            var repositorioSQLite = new Repositorio.SQLite.Repositorio();

            try
            {
                await repositorioSQLite.CriaBaseSQLite();
                IdUsuario = await repositorioSQLite.ValidarUsuarioSenhaAsync(usuario, senha);
                return IdUsuario > 0;
            }
            catch (Exception)
            {
                retorno = false;
            }

            return retorno;
        }

        public async Task<bool> AlterarUsuario(Usuario usuario)
        {
            var repositorioSQLite = new Repositorio.SQLite.Repositorio();
            var retorno = false;

            try
            {
                await repositorioSQLite.AlterarUsuarioAsync(usuario);
                retorno = true;
            }
            catch (Exception)
            {
                retorno = false;
            }

            return retorno;
        }
    }
}
