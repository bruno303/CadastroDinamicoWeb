using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CadastroDinamico.Core;
using CadastroDinamico.Dominio;
using CadastroDinamico.Web.Extension;
using CadastroDinamico.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CadastroDinamico.Web.Controllers
{
    public class ServidorController : Controller
    {
        public async Task<IActionResult> Index()
        {
            if (this.ValidarLogin())
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
                        Senha = servidor.Senha
                    });
                }


                return View(servidoresViewModel);
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
                var servidor = await new Repositorio.SQLite.Repositorio().RetornarServidorAsync(id);
                var servidorViewModel = new ServidorViewModel
                {
                    IdServidor = servidor.IdServidor,
                    Hostname = servidor.Hostname,
                    Instancia = servidor.Instancia,
                    Usuario = servidor.Usuario,
                    Senha = servidor.Senha
                };


                return View(servidorViewModel);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public async Task<IActionResult> Gravar(ServidorViewModel servidorViewModel)
        {
            if (this.ValidarLogin())
            {
                var sqliteCore = new SQLiteCore();
                var servidor = new Servidor()
                {
                    IdServidor = servidorViewModel.IdServidor.HasValue ? servidorViewModel.IdServidor.Value : 0,
                    Hostname = servidorViewModel.Hostname,
                    Instancia = servidorViewModel.Instancia,
                    Usuario = servidorViewModel.Usuario,
                    Senha = servidorViewModel.Senha
                };

                await sqliteCore.GravarServidorAsync(servidor);
                if (await sqliteCore.DeveGravarSessaoServidor())
                {
                    HttpContext.Session.SetInt32("idServidor", await sqliteCore.RetornaIdUnicoServidor());
                }

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
                var servidorViewModel = new ServidorViewModel();

                servidorViewModel.IdServidor = 0;

                return View("Editar", servidorViewModel);
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
                if (id == HttpContext.Session.GetInt32("idServidor").Value)
                {
                    HttpContext.Session.SetInt32("idServidor", 0);
                }
                await new SQLiteCore().DeletarServidorAsync(id);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public IActionResult Usar(int id)
        {
            if (this.ValidarLogin())
            {
                HttpContext.Session.SetInt32("idServidor", id);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}