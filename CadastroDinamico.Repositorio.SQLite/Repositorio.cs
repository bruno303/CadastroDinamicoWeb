using System;
using System.Threading.Tasks;

namespace CadastroDinamico.Repositorio.SQLite
{
    public class Repositorio
    {
        public async void CriaBaseSQLite()
        {
            var conexao = new Conexao();

            if (!conexao.RetornaBaseExistente())
            {
                conexao.CriarBancoDados();
                await CriarTabela();
                await CriarUsuarioAdmin();
            }
        }

        private async Task CriarTabela()
        {
            var conexao = new Conexao();
            await conexao.ExecutarQueryAsync("CREATE TABLE USUARIO (ID_USUARIO INTEGER AUTOINCREMENT, NOME TEXT, LOGON TEXT, SENHA TEXT);");
        }

        private async Task CriarUsuarioAdmin()
        {
            await CriarUsuario("ADMINISTRADOR", "ADMIN", "ADMIN!@#123");
        }

        public async Task CriarUsuario(string nome, string logon, string senha)
        {
            var conexao = new Conexao();
            await conexao.ExecutarQueryAsync($"INSERT INTO USUARIO (NOME, LOGON, SENHA) VALUES ('{nome}', '{logon}', '{senha}');");
        }

        public async Task<int> ValidarUsuarioSenha(string usuario, string senha)
        {
            var conexao = new Conexao();
            var retorno = 0;

            var dados = await conexao.RetornarDadosAsync($"SELECT ID_USUARIO FROM USUARIO WHERE LOGON = '{usuario}' AND SENHA = '{senha}'; ");

            if (dados.Rows.Count > 0)
            {
                retorno = Convert.ToInt32(dados.Rows[0]["ID_USUARIO"].ToString());
            }

            return retorno;
        }
    }
}