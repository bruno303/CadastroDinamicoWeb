using Microsoft.AspNetCore.Mvc;
using CadastroDinamico.Dominio;
using CadastroDinamico.Repositorio.SqlClient;
using UJson = CadastroDinamico.Utils.Json<CadastroDinamico.Dominio.BancoDados>;
using CadastroDinamico.Core;

namespace CadastroDinamico.Web.Controllers
{
    public class ConfiguracaoController : Controller
    {
        [HttpPost]
        public JsonResult SalvarConfiguracoes(string values)
        {
            var configurador = new ConfiguradorBancoDados();
            var json = new UJson();

            try
            {
                BancoDados configuracaoBancoDados = json.ConverterParaObjeto(values);
                configurador.AlterarConfiguracaoBancoDados(configuracaoBancoDados);
                new ConfiguradorCadastroDinamico().CriarDatabaseAplicacao();
            }
            catch(System.Exception ex)
            {
                return Json(new { result = false, error = ex.Message });
            }

            return Json(new { result = true, error = string.Empty });
        }
    }
}