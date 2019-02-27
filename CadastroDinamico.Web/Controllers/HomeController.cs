using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CadastroDinamico.Web.Models;
using CadastroDinamico.Web.Filters;

namespace CadastroDinamico.Web.Controllers
{
    public class HomeController : Controller
    {
        [CadDinamicoAuth]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult CustomError(CustomErrorViewModel error)
        {
            return View(error);
        }
    }
}
