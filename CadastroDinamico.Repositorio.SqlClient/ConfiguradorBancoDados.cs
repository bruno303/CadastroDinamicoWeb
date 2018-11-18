using CadastroDinamico.Dominio;
using CadastroDinamico.Utils;
using System;
using System.Threading.Tasks;

namespace CadastroDinamico.Repositorio.SqlClient
{
    public class ConfiguradorBancoDados
    {
        private const string FILE_NAME = "config.ini";
        private readonly string Path;

        public ConfiguradorBancoDados()
        {
            Path = AppDomain.CurrentDomain.BaseDirectory;
            if (!Path.EndsWith("\\"))
            {
                Path += "\\";
            }
            Path += FILE_NAME;
        }

        public string AlterarConfiguracaoBancoDados(BancoDados configuracao)
        {
            string retorno = string.Empty;
            string json = string.Empty;

            try
            {
                if (configuracao != null)
                {
                    json = new Json<BancoDados>().ConverterParaJson(configuracao);
                    json = new Criptografia().Criptografar(json);
                    new Arquivo().EscreverEmArquivo(Path, json, false);
                }
                else
                {
                    new Arquivo().EscreverEmArquivo(Path, string.Empty, false);
                }
            }
            catch (Exception ex)
            {
                retorno = "Erro ao salvar as configurações. Mensagem: " + ex.Message;
            }

            return retorno;
        }

        public async Task<BancoDados> RetornarConfiguracaoBancoDadosAsync()
        {
            BancoDados configuracao = null;
            Criptografia criptografia = new Criptografia();
            string json = string.Empty;

            try
            {
                Arquivo arquivo = new Arquivo();
                json = await arquivo.LerArquivoAsync(Path);
                json = criptografia.Descriptografar(json);
                configuracao = new Json<BancoDados>().ConverterParaObjeto(json);
            }
            catch (Exception)
            {
                configuracao = null;
            }

            return configuracao;
        }

        public BancoDados RetornarConfiguracaoBancoDados()
        {
            BancoDados configuracao = null;
            Criptografia criptografia = new Criptografia();
            string json = string.Empty;

            try
            {
                Arquivo arquivo = new Arquivo();
                json = arquivo.LerArquivo(Path);
                json = criptografia.Descriptografar(json);
                configuracao = new Json<BancoDados>().ConverterParaObjeto(json);
            }
            catch (Exception)
            {
                configuracao = null;
            }

            return configuracao;
        }

        public async Task<bool> ConfiguracaoValidaAsync()
        {
            return (await RetornarConfiguracaoBancoDadosAsync()) != null;
        }
    }
}
