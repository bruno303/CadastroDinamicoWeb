using CadastroDinamico.Web.Models;
using Microsoft.AspNetCore.Mvc;
using CadastroDinamico.Dominio;
using CadastroDinamico.Repositorio.SqlClient;

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
    }
}