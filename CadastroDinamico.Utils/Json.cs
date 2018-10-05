using Newtonsoft.Json;
using System;

namespace CadastroDinamico.Utils
{
    public class Json<Tipo>
    {
        public string ConverterParaJson(Tipo objeto)
        {
            string retorno = string.Empty;

            try
            {
                JsonSerializer serializer = new JsonSerializer();

                retorno = JsonConvert.SerializeObject(objeto);
            }
            catch (Exception ex)
            {
                retorno = "Erro ao ConverterParaJson. Mensagem: " + ex.Message;
            }

            return retorno;
        }

        public Tipo ConverterParaObjeto(string json)
        {
            Tipo retorno = default(Tipo);
            try
            {
                retorno = JsonConvert.DeserializeObject<Tipo>(json);
            }
            catch (Exception) { }

            return retorno;
        }
    }
}
