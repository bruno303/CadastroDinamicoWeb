using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CadastroDinamico.Web.Models;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult CustomError(string mensagem)
        {
            return View(new CustomErrorViewModel { IdErro = 0, Mensagem = mensagem });
        }
    }
}
