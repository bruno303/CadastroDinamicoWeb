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

namespace CadastroDinamico.Web.Controllers
{
    [CadDinamicoAuth]
    public class CadDinamicoController : Controller
    {
        public async Task<IActionResult> Index(string database, string schema, string tabela)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            ViewBag.Title = tabela;

            var repositorio = new Repo.Repositorio(idServidor);
            TabelaCore dadosTabela = null;

            try
            {
                dadosTabela = new TabelaCore(tabela, schema, database, idServidor);
                await dadosTabela.CarregarAsync();
                await dadosTabela.CarregarValoresAsync(true);
            }
            catch (Exception)
            {
                throw;
            }
            return View(dadosTabela);
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

            var tabela = new TabelaCore(valores["Tabela"], valores["Schema"], valores["Database"], idServidor);
            await tabela.CarregarAsync();

            await tabela.PesquisarRegistrosAsync(valores);
            ViewBag.Title = valores["Tabela"];

            return View("Index", tabela);
        }

        public async Task<IActionResult> TelaDinamicaAlteracao(string database, string schema, string tabela, string pk)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            var repositorio = new Repo.Repositorio(idServidor);
            TabelaCore dadosTabela = null;

            try
            {
                dadosTabela = new TabelaCore(tabela, schema, database, idServidor);
                await dadosTabela.CarregarAsync();
                await dadosTabela.CarregarValoresAsync(false, pk);
                ViewBag.Valores = dadosTabela.Valores;
                ViewBag.Alterar = true;

                ViewBag.UrlBack = $"/CadDinamico/Index?database={database}&schema={schema}&tabela={tabela}";
            }
            catch (Exception)
            {
                throw;
            }
            return View("TelaDinamica", dadosTabela);
        }

        public async Task<IActionResult> Novo(string database, string schema, string tabela)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            var repositorio = new Repo.Repositorio(idServidor);
            TabelaCore dadosTabela = null;

            try
            {
                dadosTabela = new TabelaCore(tabela, schema, database, idServidor);
                await dadosTabela.CarregarAsync();
                ViewBag.Alterar = false;

                ViewBag.UrlBack = $"/CadDinamico/Index?database={database}&schema={schema}&tabela={tabela}";
            }
            catch (Exception)
            {
                throw;
            }
            return View("TelaDinamicaInclusao", dadosTabela);
        }

        #region Downloads
        public async Task<FileResult> DownloadHtmlIndex(string database, string schema, string tabela)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            var tabelaCore = new TabelaCore(tabela, schema, database, idServidor);
            await tabelaCore.CarregarAsync();
            await tabelaCore.CarregarValoresAsync(true);

            ViewBag.Title = tabela;
            var html = await this.RenderViewAsync("Index", tabelaCore);

            var filename = $"{database}_{schema}_{tabela}_index";

            var bytes = await new DownloadArquivoCore().RetornarArquivoDownload(tabela, schema, database, html, filename);

            return File(bytes, "multipart/form-data", $"{filename}.zip");
        }

        public async Task<FileResult> DownloadHtmlTelaDinamicaAlteracao(string database, string schema, string tabela, string pk)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;

            var tabelaCore = new TabelaCore(tabela, schema, database, idServidor);
            await tabelaCore.CarregarAsync();
            await tabelaCore.CarregarValoresAsync(false, pk);
            ViewBag.Valores = tabelaCore.Valores;
            ViewBag.Alterar = true;

            ViewBag.UrlBack = $"/CadDinamico/Index?database={database}&schema={schema}&tabela={tabela}";

            var html = await this.RenderViewAsync("TelaDinamica", tabelaCore);
            var filename = $"{database}_{schema}_{tabela}_dinamic";

            var bytes = await new DownloadArquivoCore().RetornarArquivoDownload(tabela, schema, database, html, filename);

            return File(bytes, "multipart/form-data", $"{filename}.zip");
        }

        public async Task<FileResult> DownloadHtmlTelaDinamicaInclusao(string database, string schema, string tabela)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;

            var tabelaCore = new TabelaCore(tabela, schema, database, idServidor);
            await tabelaCore.CarregarAsync();
            ViewBag.Alterar = false;

            ViewBag.UrlBack = $"/CadDinamico/Index?database={database}&schema={schema}&tabela={tabela}";

            var html = await this.RenderViewAsync("TelaDinamicaInclusao", tabelaCore);
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
            var tabela = new TabelaCore(valores["Tabela"], valores["Schema"], valores["Database"], idServidor);
            await tabela.CarregarAsync();
            await tabela.AlterarRegistroAsync(valores);

            return RedirectToAction("Index", new { database = valores["Database"], schema = valores["Schema"], tabela = valores["Tabela"] });
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
            var tabela = new TabelaCore(valores["Tabela"], valores["Schema"], valores["Database"], idServidor);
            await tabela.CarregarAsync();
            await tabela.InserirRegistroAsync(valores);

            return RedirectToAction("Index", new { database = valores["Database"], schema = valores["Schema"], tabela = valores["Tabela"] });
        }
        #endregion
    }
}