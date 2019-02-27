using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Repo = CadastroDinamico.Repositorio.SqlClient;
using System.Linq;
using CadastroDinamico.Core;
using System;
using Microsoft.AspNetCore.Http;
using CadastroDinamico.Web.Extension;
using System.Threading.Tasks;
using CadastroDinamico.Web.Filters;
using CadastroDinamico.Web.Models;

namespace CadastroDinamico.Web.Controllers
{
    [CadDinamicoAuth]
    public class CadDinamicoController : Controller
    {
        public readonly ITabelaCore _tabelaCore;

        public CadDinamicoController(ITabelaCore core)
        {
            _tabelaCore = core;
        }

        public async Task<IActionResult> Index(string database, string schema, string tabela)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            ViewBag.Title = tabela;

            var repositorio = new Repo.Repositorio(idServidor);

            try
            {
                await _tabelaCore.CarregarAsync(tabela, schema, database, idServidor);
                await _tabelaCore.CarregarValoresAsync(true);
            }
            catch (Exception)
            {
                throw;
            }
            return View(_tabelaCore);
        }

        public async Task<IActionResult> ConsultarDadosIndex(string database, string schema, string tabela)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            var repositorio = new Repo.Repositorio(idServidor);

            try
            {
                ViewBag.Title = tabela;
                await _tabelaCore.CarregarAsync(tabela, schema, database, idServidor);
                await _tabelaCore.CarregarValoresAsync(true);
            }
            catch (Exception)
            {
                throw;
            }
            return Json(_tabelaCore);
        }

        public async Task<IActionResult> Filtrar(IFormCollection formCollection)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            var lista = formCollection.ToList();
            var valores = new Dictionary<string, string>();

            foreach (var item in lista)
            {
                valores.Add(item.Key, item.Value[0]);
            }

            await _tabelaCore.CarregarAsync(valores["Tabela"], valores["Schema"], valores["Database"], idServidor);

            await _tabelaCore.PesquisarRegistrosAsync(valores);
            ViewBag.Title = valores["Tabela"];

            return Json(_tabelaCore);
        }

        public async Task<IActionResult> TelaDinamicaAlteracao(string database, string schema, string tabela, string pk)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            var repositorio = new Repo.Repositorio(idServidor);

            try
            {
                await _tabelaCore.CarregarAsync(tabela, schema, database, idServidor);
                await _tabelaCore.CarregarValoresAsync(false, pk);
                ViewBag.Valores = _tabelaCore.Valores;
                ViewBag.Alterar = true;

                ViewBag.UrlBack = $"/CadDinamico/Index?database={database}&schema={schema}&tabela={tabela}";
            }
            catch (Exception)
            {
                throw;
            }
            return View("TelaDinamica", _tabelaCore);
        }

        public async Task<IActionResult> Novo(string database, string schema, string tabela)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            var repositorio = new Repo.Repositorio(idServidor);

            try
            {
                await _tabelaCore.CarregarAsync(tabela, schema, database, idServidor);
                ViewBag.Alterar = false;

                ViewBag.UrlBack = $"/CadDinamico/Index?database={database}&schema={schema}&tabela={tabela}";
            }
            catch (Exception)
            {
                throw;
            }
            return View("TelaDinamicaInclusao", _tabelaCore);
        }

        #region Downloads
        public async Task<FileResult> DownloadHtmlIndex(string database, string schema, string tabela)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            await _tabelaCore.CarregarAsync(tabela, schema, database, idServidor);
            await _tabelaCore.CarregarValoresAsync(true);

            ViewBag.Title = tabela;
            var html = await this.RenderViewAsync("Index", _tabelaCore);

            var filename = $"{database}_{schema}_{tabela}_index";

            var bytes = await new DownloadArquivoCore().RetornarArquivoDownload(tabela, schema, database, html, filename);

            return File(bytes, "multipart/form-data", $"{filename}.zip");
        }

        public async Task<FileResult> DownloadHtmlTelaDinamicaAlteracao(string database, string schema, string tabela, string pk)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            await _tabelaCore.CarregarAsync(tabela, schema, database, idServidor);
            await _tabelaCore.CarregarValoresAsync(false, pk);
            ViewBag.Valores = _tabelaCore.Valores;
            ViewBag.Alterar = true;

            ViewBag.UrlBack = $"/CadDinamico/Index?database={database}&schema={schema}&tabela={tabela}";

            var html = await this.RenderViewAsync("TelaDinamica", _tabelaCore);
            var filename = $"{database}_{schema}_{tabela}_dinamic";

            var bytes = await new DownloadArquivoCore().RetornarArquivoDownload(tabela, schema, database, html, filename);

            return File(bytes, "multipart/form-data", $"{filename}.zip");
        }

        public async Task<FileResult> DownloadHtmlTelaDinamicaInclusao(string database, string schema, string tabela)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            await _tabelaCore.CarregarAsync(tabela, schema, database, idServidor);
            ViewBag.Alterar = false;

            ViewBag.UrlBack = $"/CadDinamico/Index?database={database}&schema={schema}&tabela={tabela}";

            var html = await this.RenderViewAsync("TelaDinamicaInclusao", _tabelaCore);
            var filename = $"{database}_{schema}_{tabela}_dinamic";

            var bytes = await new DownloadArquivoCore().RetornarArquivoDownload(tabela, schema, database, html, filename);

            return File(bytes, "multipart/form-data", $"{filename}.zip");
        }
        #endregion

        #region Salvar Informacoes
        public async Task<IActionResult> GravarItem(IFormCollection formCollection)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            var lista = formCollection.ToList();
            var valores = new Dictionary<string, string>();

            foreach (var item in lista)
            {
                valores.Add(item.Key, item.Value[0]);
            }
            await _tabelaCore.CarregarAsync(valores["Tabela"], valores["Schema"], valores["Database"], idServidor);
            await _tabelaCore.AlterarRegistroAsync(valores);

            return RedirectToAction("Index", new { database = valores["Database"], schema = valores["Schema"], tabela = valores["Tabela"] });
        }

        public async Task<IActionResult> DeletarItem(string database, string schema, string tabela, string pk)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;

            await _tabelaCore.CarregarAsync(tabela, schema, database, idServidor);
            var result = await _tabelaCore.DeletarRegistroAsync(idServidor, pk);
            if ((!string.IsNullOrEmpty(result[0])) || (!string.IsNullOrEmpty(result[1])) || (!string.IsNullOrEmpty(result[2])))
            {
                return RedirectToAction("CustomError", "Home", new CustomErrorViewModel()
                {
                    IdErro = 0, Titulo = result[0], Mensagem = result[1], ComandoExecutado = result[2]
                });
            }

            return RedirectToAction("Index", new { database, schema, tabela });
        }

        public async Task<IActionResult> GravarNovoItem(IFormCollection formCollection)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            var lista = formCollection.ToList();
            var valores = new Dictionary<string, string>();

            foreach (var item in lista)
            {
                valores.Add(item.Key, item.Value[0]);
            }
            await _tabelaCore.CarregarAsync(valores["Tabela"], valores["Schema"], valores["Database"], idServidor);
            await _tabelaCore.InserirRegistroAsync(valores);

            return RedirectToAction("Index", new { database = valores["Database"], schema = valores["Schema"], tabela = valores["Tabela"] });
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            _tabelaCore.Dispose();
            base.Dispose(disposing);
        }
    }
}