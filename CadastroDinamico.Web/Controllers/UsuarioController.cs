using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CadastroDinamico.Core;
using CadastroDinamico.Web.Models;
using System.Collections.Generic;
using CadastroDinamico.Web.Extension;
using CadastroDinamico.Dominio;

namespace CadastroDinamico.Web.Controllers
{
    public class UsuarioController : Controller
    {
        public async Task<IActionResult> Index()
        {
            if (this.ValidarLogin())
            {
                var usuarios = await new Repositorio.SQLite.Repositorio().RetornarUsuariosAsync();
                var usuariosViewModel = new List<UsuarioViewModel>();

                foreach (var usuario in usuarios)
                {
                    usuariosViewModel.Add(new UsuarioViewModel()
                    {
                        IdUsuario = usuario.IdUsuario,
                        Nome = usuario.Nome,
                        Login = usuario.Login,
                        Senha = usuario.Senha
                    });
                }


                return View(usuariosViewModel);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (this.ValidarLogin())
            {
                var usuario = await new Repositorio.SQLite.Repositorio().RetornarUsuarioAsync(id);
                var usuarioViewModel = new UsuarioViewModel();

                usuarioViewModel.IdUsuario = usuario.IdUsuario;
                usuarioViewModel.Nome = usuario.Nome;
                usuarioViewModel.Login = usuario.Login;
                usuarioViewModel.Senha = usuario.Senha;


                return View("EdicaoUsuario", usuarioViewModel);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public async Task<IActionResult> Gravar(UsuarioViewModel usuarioViewModel)
        {
            if (this.ValidarLogin())
            {
                var sqliteCore = new SQLiteCore();
                var usuario = new Usuario()
                {
                    IdUsuario = usuarioViewModel.IdUsuario.HasValue ? usuarioViewModel.IdUsuario.Value : 0,
                    Nome = usuarioViewModel.Nome,
                    Login = usuarioViewModel.Login,
                    Senha = usuarioViewModel.Senha
                };

                await sqliteCore.GravarUsuarioAsync(usuario);

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public IActionResult Novo(int id)
        {
            if (this.ValidarLogin())
            {
                var usuarioViewModel = new UsuarioViewModel();

                usuarioViewModel.IdUsuario = 0;

                return View("EdicaoUsuario", usuarioViewModel);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public async Task<IActionResult> Deletar(int id)
        {
            if (this.ValidarLogin())
            {
                await new Repositorio.SQLite.Repositorio().DeletarUsuarioAsync(id);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}