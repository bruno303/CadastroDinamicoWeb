using System;
using System.Threading.Tasks;
using Repo = CadastroDinamico.Repositorio.SqlClient.Repositorio;

namespace CadastroDinamico.Core
{
    public class ConfiguradorCadastroDinamico
    {
        private const string DATABASE_NAME = "CAD_DINAMICO";
        private const string ANSI_NULL_COMMAND = "SET ANSI_NULL ON";
        private const string QUOTED_IDENTIFIER_COMMAND = "SET QUOTED_IDENTIFIER ON";

        public async Task<string> CriarDatabaseAplicacaoAsync(int idServidor)
        {
            var retorno = string.Empty;
            var repositorio = new Repo(idServidor);

            try
            {
                if (!await repositorio.ExisteDatabaseAsync(DATABASE_NAME))
                {
                    await repositorio.CriarDatabaseAsync(DATABASE_NAME);
                    await repositorio.CriarObjetosAplicacaoAsync(DATABASE_NAME, ANSI_NULL_COMMAND, QUOTED_IDENTIFIER_COMMAND);
                }
            }
            catch(Exception ex)
            {
                retorno = ex.Message;
            }
            return retorno;
        }
    }
}
