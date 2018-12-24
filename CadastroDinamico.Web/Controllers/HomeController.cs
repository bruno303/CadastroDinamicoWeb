using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CadastroDinamico.Web.Models;
using CadastroDinamico.Repositorio.SqlClient;

namespace CadastroDinamico.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ConfiguracaoBanco()
        {
            ConfiguradorBancoDados configurador = new ConfiguradorBancoDados();
            var configuracao = configurador.RetornarConfiguracaoBancoDados();
            ConfiguracaoBancoDadosViewModel viewModel = new ConfiguracaoBancoDadosViewModel();
            if (configuracao != null)
            {
                viewModel.Servidor = configuracao.Servidor;
                viewModel.Instancia = configuracao.Instancia;
                viewModel.Usuario = configuracao.Usuario;
                viewModel.Senha = configuracao.Senha;
                viewModel.RegistrarLog = configuracao.RegistrarLog;
            }
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
