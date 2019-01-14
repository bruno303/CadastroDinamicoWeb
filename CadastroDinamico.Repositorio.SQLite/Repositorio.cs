using System;
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

        public async Task CriarUsuarioAsync(string nome, string logon, string senha)
        {
            var conexao = new Conexao();
            await conexao.ExecutarQueryAsync($"INSERT INTO USUARIO (NOME, LOGIN, SENHA) VALUES ('{nome}', '{logon}', '{senha}');");
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
                $"WHERE ID_USUARIO = '{usuario.IdUsuario.ToString()}'; ";

            await conexao.ExecutarQueryAsync(query);
        }

        public async Task DeletarUsuarioAsync(Usuario usuario)
        {
            var conexao = new Conexao();
            var query = $"DELETE FROM USUARIO WHERE ID_USUARIO = '{usuario.IdUsuario.ToString()}'; ";

            await conexao.ExecutarQueryAsync(query);
        }
    }
}