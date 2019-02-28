using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CadastroDinamico.Core;
using CadastroDinamico.Web.Models;
using System.Collections.Generic;
using CadastroDinamico.Dominio;
using CadastroDinamico.Web.Filters;
using Microsoft.AspNetCore.Http;

namespace CadastroDinamico.Web.Controllers
{
    [CadDinamicoAuth]
    public class UsuarioController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            var usuarios = await new Repositorio.SQLite.Repositorio().RetornarUsuariosAsync(idUsuario.Value);
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

        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await new Repositorio.SQLite.Repositorio().RetornarUsuarioAsync(id);
            var usuarioViewModel = new UsuarioViewModel();

            usuarioViewModel.IdUsuario = usuario.IdUsuario;
            usuarioViewModel.Nome = usuario.Nome;
            usuarioViewModel.Login = usuario.Login;
            usuarioViewModel.Senha = usuario.Senha;


            return View("EdicaoUsuario", usuarioViewModel);
        }

        public async Task<IActionResult> Gravar(UsuarioViewModel usuarioViewModel)
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

        public IActionResult Novo(int id)
        {
            var usuarioViewModel = new UsuarioViewModel();

            usuarioViewModel.IdUsuario = 0;

            return View("EdicaoUsuario", usuarioViewModel);
        }

        public async Task<IActionResult> Deletar(int id)
        {
            await new Repositorio.SQLite.Repositorio().DeletarUsuarioAsync(id);
            return RedirectToAction("Index");
        }
    }
}