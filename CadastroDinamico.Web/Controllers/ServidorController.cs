using System.Collections.Generic;
using System.Threading.Tasks;
using CadastroDinamico.Core;
using CadastroDinamico.Dominio;
using CadastroDinamico.Web.Filters;
using CadastroDinamico.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CadastroDinamico.Web.Controllers
{
    [CadDinamicoAuth]
    public class ServidorController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var servidores = await new Repositorio.SQLite.Repositorio().RetornarServidoresAsync();
            var servidoresViewModel = new List<ServidorViewModel>();

            foreach (var servidor in servidores)
            {
                servidoresViewModel.Add(new ServidorViewModel()
                {
                    IdServidor = servidor.IdServidor,
                    Hostname = servidor.Hostname,
                    Instancia = servidor.Instancia,
                    Usuario = servidor.Usuario,
                    Senha = servidor.Senha,
                    GravarLog = servidor.GravarLog,
                    UsarTransacao = servidor.UsarTransacao
                });
            }


            return View(servidoresViewModel);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var servidor = await new Repositorio.SQLite.Repositorio().RetornarServidorAsync(id);
            var servidorViewModel = new ServidorViewModel
            {
                IdServidor = servidor.IdServidor,
                Hostname = servidor.Hostname,
                Instancia = servidor.Instancia,
                Usuario = servidor.Usuario,
                Senha = servidor.Senha,
                GravarLog = servidor.GravarLog,
                UsarTransacao = servidor.UsarTransacao
            };

            return View(servidorViewModel);
        }

        public async Task<IActionResult> Gravar(ServidorViewModel servidorViewModel)
        {
            var sqliteCore = new SQLiteCore();
            var servidor = new Servidor()
            {
                IdServidor = servidorViewModel.IdServidor.HasValue ? servidorViewModel.IdServidor.Value : 0,
                Hostname = servidorViewModel.Hostname,
                Instancia = servidorViewModel.Instancia,
                Usuario = servidorViewModel.Usuario,
                Senha = servidorViewModel.Senha,
                GravarLog = servidorViewModel.GravarLog,
                UsarTransacao = servidorViewModel.UsarTransacao
            };

            await sqliteCore.GravarServidorAsync(servidor);
            if (await sqliteCore.DeveGravarSessaoServidor())
            {
                HttpContext.Session.SetInt32("idServidor", await sqliteCore.RetornaIdUnicoServidor());
            }

            return RedirectToAction("Index");
        }

        public IActionResult Novo(int id)
        {
            var servidorViewModel = new ServidorViewModel();

            servidorViewModel.IdServidor = 0;
            servidorViewModel.GravarLog = true;
            servidorViewModel.UsarTransacao = true;

            return View("Editar", servidorViewModel);
        }

        public async Task<IActionResult> Deletar(int id)
        {
            if (id == HttpContext.Session.GetInt32("idServidor").Value)
            {
                HttpContext.Session.SetInt32("idServidor", 0);
            }
            await new SQLiteCore().DeletarServidorAsync(id);
            return RedirectToAction("Index");
        }

        public IActionResult Usar(int id)
        {
            HttpContext.Session.SetInt32("idServidor", id);
            return RedirectToAction("Index");
        }
    }
}