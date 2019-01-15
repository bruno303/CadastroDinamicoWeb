using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CadastroDinamico.Dominio;

namespace CadastroDinamico.Repositorio.SQLite
{
    public class Repositorio
    {
        public async Task CriaBaseSQLite()
        {
            var conexao = new Conexao();

            if (!conexao.RetornaBaseExistente())
            {
                conexao.CriarBancoDados();
                await CriarTabelaAsync();
                await CriarUsuarioAdminAsync();
            }
        }

        private async Task CriarTabelaAsync()
        {
            var conexao = new Conexao();
            await conexao.ExecutarQueryAsync("CREATE TABLE USUARIO (ID_USUARIO INTEGER PRIMARY KEY AUTOINCREMENT, NOME TEXT, LOGIN TEXT, SENHA TEXT);");
        }

        private async Task CriarUsuarioAdminAsync()
        {
            await CriarUsuarioAsync("ADMINISTRADOR", "ADMIN", "admin!@#123");
        }

        public async Task CriarUsuarioAsync(string nome, string login, string senha)
        {
            var conexao = new Conexao();
            await conexao.ExecutarQueryAsync($"INSERT INTO USUARIO (NOME, LOGIN, SENHA) VALUES ('{nome.ToUpper()}', '{login.ToUpper()}', '{senha}');");
        }

        public async Task<int> ValidarUsuarioSenhaAsync(string usuario, string senha)
        {
            var conexao = new Conexao();
            var retorno = 0;

            var dados = await conexao.RetornarDadosAsync($"SELECT ID_USUARIO FROM USUARIO WHERE UPPER(LOGIN) = '{usuario.ToUpper()}' AND SENHA = '{senha}'; ");

            if (dados.Rows.Count > 0)
            {
                retorno = Convert.ToInt32(dados.Rows[0]["ID_USUARIO"].ToString());
            }

            return retorno;
        }

        public async Task AlterarUsuarioAsync(Usuario usuario)
        {
            var conexao = new Conexao();
            var query = $"UPDATE USUARIO SET NOME = '{usuario.Nome.ToUpper()}', " +
                $"LOGIN = '{usuario.Login.ToUpper()}', " +
                $"SENHA = '{usuario.Senha}' " +
                $"WHERE ID_USUARIO = {usuario.IdUsuario.ToString()}; ";

            await conexao.ExecutarQueryAsync(query);
        }

        public async Task DeletarUsuarioAsync(int idUsuario)
        {
            var conexao = new Conexao();
            var query = $"DELETE FROM USUARIO WHERE ID_USUARIO = {idUsuario.ToString()}; ";

            await conexao.ExecutarQueryAsync(query);
        }

        public async Task<List<Usuario>> RetornarUsuarios()
        {
            var conexao = new Conexao();
            var query = " SELECT ID_USUARIO, NOME, LOGIN, SENHA FROM USUARIO; ";
            var retorno = new List<Usuario>();

            var dados = await conexao.RetornarDadosAsync(query);

            if (dados.Rows.Count > 0)
            {
                for (int i = 0; i < dados.Rows.Count; i++)
                {
                    retorno.Add(new Usuario()
                    {
                        IdUsuario = Convert.ToInt32(dados.Rows[i][0].ToString()),
                        Nome = dados.Rows[i][1].ToString(),
                        Login = dados.Rows[i][2].ToString(),
                        Senha = dados.Rows[i][3].ToString()
                    });
                }
            }

            return retorno;
        }

        public async Task<Usuario> RetornarUsuario(int idUsuario)
        {
            var conexao = new Conexao();
            var query = $" SELECT ID_USUARIO, NOME, LOGIN, SENHA FROM USUARIO WHERE ID_USUARIO = {idUsuario.ToString()}; ";
            var retorno = new Usuario();

            var dados = await conexao.RetornarDadosAsync(query);

            if (dados.Rows.Count > 0)
            {
                retorno.IdUsuario = Convert.ToInt32(dados.Rows[0][0].ToString());
                retorno.Nome = dados.Rows[0][1].ToString();
                retorno.Login = dados.Rows[0][2].ToString();
                retorno.Senha = dados.Rows[0][3].ToString();
            }

            return retorno;
        }
    }
}