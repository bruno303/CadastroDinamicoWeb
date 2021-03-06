﻿using CadastroDinamico.Core;
using CadastroDinamico.Dominio;
using CadastroDinamico.Web.Filters;
using CadastroDinamico.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CadastroDinamico.Web.Controllers
{
    public class LoginController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var loginViewModel = new LoginViewModel();
            await loginViewModel.CarregarServidoresAsync();
            return View(loginViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var sqliteCore = new SQLiteCore();
                var usuario = new Usuario()
                {
                    Login = loginViewModel.Usuario,
                    Senha = loginViewModel.Senha
                };

                if (await sqliteCore.ValidarLoginUsuarioAsync(usuario))
                {
                    HttpContext.Session.SetInt32("idUsuario", sqliteCore.IdUsuario);
                    HttpContext.Session.SetInt32("idServidor", loginViewModel.Servidor ?? 0);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Usuário e/ou senha não encontrados.");
                    await loginViewModel.CarregarServidoresAsync();
                    return View("Index", loginViewModel);
                }
            }
            else
            {
                ModelState.AddModelError("", "Ocorreu um erro ao tentar realizar o login.");
                await loginViewModel.CarregarServidoresAsync();
                return View("Index", loginViewModel);
            }
        }

        [CadDinamicoAuth]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}