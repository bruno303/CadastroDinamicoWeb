using CadastroDinamico.Core;
using CadastroDinamico.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CadastroDinamico.Web.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var sqliteCore = new SQLiteCore();
                if (await sqliteCore.ValidarLoginUsuario(loginViewModel.Usuario, loginViewModel.Senha))
                {
                    HttpContext.Session.SetInt32("idUsuario", sqliteCore.IdUsuario);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Usuário e/ou senha não encontrados.");
                    return View("Index", loginViewModel);
                }
            }
            else
            {
                ModelState.AddModelError("", "Ocorreu um erro ao tentar realizar o login.");
                return View("Index", loginViewModel);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}