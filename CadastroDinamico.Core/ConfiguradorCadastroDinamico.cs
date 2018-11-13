using System;
using Repo = CadastroDinamico.Repositorio.SqlClient.Repositorio;

namespace CadastroDinamico.Core
{
    public class ConfiguradorCadastroDinamico
    {
        private const string DATABASE_NAME = "CAD_DINAMICO";
        private const string ANSI_NULL_COMMAND = "SET ANSI_NULL ON";
        private const string QUOTED_IDENTIFIER_COMMAND = "SET QUOTED_IDENTIFIER ON";

        public string CriarDatabaseAplicacao()
        {
            var retorno = string.Empty;
            var repositorio = new Repo();

            try
            {
                if (!repositorio.ExisteDatabase(DATABASE_NAME))
                {
                    repositorio.CriarDatabase(DATABASE_NAME);
                    repositorio.CriarObjetosAplicacao(DATABASE_NAME, ANSI_NULL_COMMAND, QUOTED_IDENTIFIER_COMMAND);
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
