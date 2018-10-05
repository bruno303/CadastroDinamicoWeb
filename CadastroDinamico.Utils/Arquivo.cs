using System;
using System.IO;

namespace CadastroDinamico.Utils
{
    public class Arquivo
    {
        public string EscreverEmArquivo(string path, string texto, bool append)
        {
            string retorno = string.Empty;

            try
            {
                using (StreamWriter writer = new StreamWriter(path, append))
                {
                    writer.Write(texto);
                }
            }
            catch (Exception ex)
            {
                retorno = "Erro em EscreverEmArquivo. Mensage: " + ex.Message;
            }

            return retorno;
        }

        public string LerArquivo(string path)
        {
            string texto = string.Empty;

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    texto = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                texto = string.Empty;
            }

            return texto;
        }
    }
}
