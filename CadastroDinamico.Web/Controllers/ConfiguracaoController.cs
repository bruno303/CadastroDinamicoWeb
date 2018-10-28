using CadastroDinamico.Web.Models;
using Microsoft.AspNetCore.Mvc;
using CadastroDinamico.Dominio;
using CadastroDinamico.Repositorio.SqlClient;
using UJson = CadastroDinamico.Utils.Json<CadastroDinamico.Dominio.BancoDados>;

namespace CadastroDinamico.Web.Controllers
{
    public class ConfiguracaoController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SalvarConfiguracoes(ConfiguracaoBancoDadosViewModel viewModel)
        {
            ConfiguradorBancoDados configurador = new ConfiguradorBancoDados();

            BancoDados configuracaoBancoDados = new BancoDados()
            {
                Servidor = viewModel.Servidor,
                Database = viewModel.Database,
                Usuario = viewModel.Usuario,
                Senha = viewModel.Senha,
                RegistrarLog = viewModel.RegistrarLog
            };
            configurador.AlterarConfiguracaoBancoDados(configuracaoBancoDados);
            return RedirectToAction("ConfiguracaoBanco", "Home");
        }

        [HttpGet]
        public JsonResult SalvarConfiguracoes(string values)
        {
            var configurador = new ConfiguradorBancoDados();
            var json = new UJson();

            try
            {
                BancoDados configuracaoBancoDados = json.ConverterParaObjeto(values);
                configurador.AlterarConfiguracaoBancoDados(configuracaoBancoDados);
            }
            catch(System.Exception ex)
            {
                return Json(new { result = false, error = ex.Message });
            }

            return Json(new { result = true, error = string.Empty });
        }
    }
}