using System;
using System.IO;
using System.Threading.Tasks;

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
                retorno = "Erro em EscreverEmArquivo. Mensagem: " + ex.Message;
            }

            return retorno;
        }

        public async Task<string> EscreverEmArquivoAsync(string path, string texto, bool append)
        {
            string retorno = string.Empty;

            try
            {
                using (StreamWriter writer = new StreamWriter(path, append))
                {
                    await writer.WriteAsync(texto);
                }
            }
            catch (Exception ex)
            {
                retorno = "Erro em EscreverEmArquivo. Mensagem: " + ex.Message;
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

        public async Task<string> LerArquivoAsync(string path)
        {
            string texto = string.Empty;

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    texto = await reader.ReadToEndAsync();
                }
            }
            catch (Exception)
            {
                texto = string.Empty;
            }

            return texto;
        }

        public async Task<FileInfo[]> RetornarArquivosDiretorioAsync(string path)
        {
            if (Directory.Exists(path))
            {
                var task = Task.Run(() => new DirectoryInfo(path).GetFiles());
                return await task;
            }
            else
            {
                throw new ArgumentException("Diretório informado não existe!", "path");
            }
        }
    }
}
