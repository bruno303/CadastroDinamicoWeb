using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CadastroDinamico.Web.Models;
using CadastroDinamico.Repositorio.SqlClient;
using CadastroDinamico.Web.Extension;

namespace CadastroDinamico.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (this.ValidarLogin())
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public IActionResult ConfiguracaoBanco()
        {
            if (this.ValidarLogin())
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
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
