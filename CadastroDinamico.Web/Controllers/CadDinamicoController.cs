using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Repo = CadastroDinamico.Repositorio.SqlClient;
using System.Linq;
using CadastroDinamico.Core;
using System;
using Microsoft.AspNetCore.Http;
using CadastroDinamico.Web.Extension;
using System.Threading.Tasks;

namespace CadastroDinamico.Web.Controllers
{
    public class CadDinamicoController : Controller
    {
        public async Task<IActionResult> Index(string database, string schema, string tabela)
        {
            if (this.ValidarLogin())
            {
                var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
                ViewBag.Title = string.Format("{0}.{1}.{2}", database, schema, tabela).ToUpper();

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
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        #region Teste
        public async Task<IActionResult> TelaDinamicaAlteracao(string database, string schema, string tabela, string pk)
        {
            if (this.ValidarLogin())
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
                }
                catch (Exception)
                {
                    throw;
                }
                return View("TelaDinamica", dadosTabela);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpGet]
        public async Task<FileResult> DownloadHtml(string database, string schema, string tabela)
        {
            if (this.ValidarLogin())
            {
                var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
                var tabelaCore = new TabelaCore(tabela, schema, database, idServidor);
                await tabelaCore.CarregarAsync();
                await tabelaCore.CarregarValoresAsync(true);
                var html = await this.RenderViewAsync("Index", tabelaCore);
                var path = string.Format("DownloadFiles\\{0}\\", DateTime.Now.ToString("yyyy-MM-dd_HHmmss"));
                var fileName = $"{database}_{schema}_{tabela}.html";

                System.IO.Directory.CreateDirectory(path);

                await new Utils.Arquivo().EscreverEmArquivoAsync(path + fileName, html, false);

                var bytes = await System.IO.File.ReadAllBytesAsync(path + fileName);

                System.IO.File.Delete(path + fileName);
                System.IO.Directory.Delete(path, true);

                return File(bytes, "multipart/form-data", fileName);
            }
            else
            {
                throw new Exception("É necessário fazer login na aplicação");
            }
        }

        public async Task<IActionResult> GravarItem(IFormCollection formCollection)
        {
            if (this.ValidarLogin())
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
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        #endregion
    }
}