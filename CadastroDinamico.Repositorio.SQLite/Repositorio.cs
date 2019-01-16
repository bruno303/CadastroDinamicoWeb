using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CadastroDinamico.Dominio;

namespace CadastroDinamico.Repositorio.SQLite
{
    public class Repositorio
    {
        public async Task CriarBaseSQLiteAsync()
        {
            var conexao = new Conexao();

            if (!conexao.RetornaBaseExistente())
            {
                conexao.CriarBancoDados();
                await CriarTabelaUsuarioAsync();
                await CriarUsuarioAdminAsync();
                await CriarTabelaServidorAsync();
            }
        }

        private async Task CriarTabelaUsuarioAsync()
        {
            var conexao = new Conexao();
            await conexao.ExecutarQueryAsync("CREATE TABLE USUARIO (ID_USUARIO INTEGER PRIMARY KEY AUTOINCREMENT, NOME TEXT, LOGIN TEXT, SENHA TEXT);");
        }

        private async Task CriarTabelaServidorAsync()
        {
            var conexao = new Conexao();
            var query = new System.Text.StringBuilder();
            query.AppendLine("CREATE TABLE SERVIDOR");
            query.AppendLine("(");
            query.AppendLine("    ID_SERVIDOR INTEGER PRIMARY KEY AUTOINCREMENT,");
            query.AppendLine("    HOSTNAME TEXT,");
            query.AppendLine("    INSTANCIA TEXT,");
            query.AppendLine("    USUARIO TEXT,");
            query.AppendLine("    SENHA TEXT");
            query.AppendLine(");");

            await conexao.ExecutarQueryAsync(query.ToString());
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

        public async Task<List<Usuario>> RetornarUsuariosAsync()
        {
            var conexao = new Conexao();
            var query = " SELECT ID_USUARIO, NOME, LOGIN, SENHA FROM USUARIO; ";
            var retorno = new List<Usuario>();

            await CriarBaseSQLiteAsync();
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

        public async Task<Usuario> RetornarUsuarioAsync(int idUsuario)
        {
            var conexao = new Conexao();
            var query = $" SELECT ID_USUARIO, NOME, LOGIN, SENHA FROM USUARIO WHERE ID_USUARIO = {idUsuario.ToString()}; ";
            var retorno = new Usuario();

            await CriarBaseSQLiteAsync();
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

        public async Task<List<Servidor>> RetornarServidoresAsync()
        {
            var conexao = new Conexao();
            var query = " SELECT ID_SERVIDOR, HOSTNAME, INSTANCIA, USUARIO, SENHA FROM SERVIDOR; ";
            var retorno = new List<Servidor>();

            await CriarBaseSQLiteAsync();
            var dados = await conexao.RetornarDadosAsync(query);

            if (dados.Rows.Count > 0)
            {
                for (int i = 0; i < dados.Rows.Count; i++)
                {
                    retorno.Add(new Servidor()
                    {
                        IdServidor = Convert.ToInt32(dados.Rows[i][0].ToString()),
                        Hostname = dados.Rows[i][1].ToString(),
                        Instancia = dados.Rows[i][2].ToString(),
                        Usuario = dados.Rows[i][3].ToString(),
                        Senha = dados.Rows[i][4].ToString()
                    });
                }
            }

            return retorno;
        }

        public async Task<Servidor> RetornarServidorAsync(int idServidor)
        {
            var conexao = new Conexao();
            var query = $" SELECT ID_SERVIDOR, HOSTNAME, INSTANCIA, USUARIO, SENHA FROM SERVIDOR WHERE ID_SERVIDOR = {idServidor.ToString()}; ";
            var retorno = new Servidor();

            await CriarBaseSQLiteAsync();
            var dados = await conexao.RetornarDadosAsync(query);

            if (dados.Rows.Count > 0)
            {
                retorno.IdServidor = Convert.ToInt32(dados.Rows[0][0].ToString());
                retorno.Hostname = dados.Rows[0][1].ToString();
                retorno.Instancia = dados.Rows[0][2].ToString();
                retorno.Usuario = dados.Rows[0][3].ToString();
                retorno.Senha = dados.Rows[0][4].ToString();
            }

            return retorno;
        }

        public async Task CriarServidorAsync(Servidor servidor)
        {
            var conexao = new Conexao();
            var query = "INSERT INTO SERVIDOR(HOSTNAME, INSTANCIA, USUARIO, SENHA) VALUES ";
            query += $"  ('{servidor.Hostname}', '{servidor.Instancia}', '{servidor.Usuario}', '{servidor.Senha}');";

            await conexao.ExecutarQueryAsync(query);
            
        }

        public async Task AlterarServidorAsync(Servidor servidor)
        {
            var conexao = new Conexao();
            var query = $"UPDATE SERVIDOR SET HOSTNAME = '{servidor.Hostname}', " +
                $"INSTANCIA = '{servidor.Instancia}', " +
                $"USUARIO = '{servidor.Usuario}', " +
                $"SENHA = '{servidor.Senha}' " +
                $"WHERE ID_SERVIDOR = {servidor.IdServidor.ToString()}; ";

            await conexao.ExecutarQueryAsync(query);
        }

        public async Task DeletarServidorAsync(int idServidor)
        {
            var conexao = new Conexao();
            var query = $"DELETE FROM SERVIDOR WHERE ID_SERVIDOR = {idServidor.ToString()}; ";

            await conexao.ExecutarQueryAsync(query);
        }
    }
}