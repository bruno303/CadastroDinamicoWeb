using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Repo = CadastroDinamico.Repositorio.SqlClient;
using System.Linq;
using CadastroDinamico.Core;
using System;
using Microsoft.AspNetCore.Http;
using CadastroDinamico.Web.Extension;

namespace CadastroDinamico.Web.Controllers
{
    public class CadDinamicoController : Controller
    {
        public IActionResult Index(string database, string schema, string tabela)
        {
            ViewBag.Title = string.Format("{0}.{1}.{2}", database, schema, tabela).ToUpper();
            var repositorio = new Repo.Repositorio();
            var dadosTabela = new TabelaCore(tabela, schema, database);

            try
            {
                var result = dadosTabela.Carregar();
                dadosTabela.CarregarValores(true);
                if (!string.IsNullOrEmpty(result))
                {
                    return View("Error", "Home");
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View(dadosTabela);
        }

        #region Teste
        public IActionResult TelaDinamicaAlteracao(string database, string schema, string tabela, string pk)
        {
            var repositorio = new Repo.Repositorio();
            var dadosTabela = new TabelaCore(tabela, schema, database);
            
            try
            {
                var result = dadosTabela.Carregar();
                dadosTabela.CarregarValores(false, pk);
                if (!string.IsNullOrEmpty(result))
                {
                    return View("Error", "Home");
                }
                ViewBag.Valores = dadosTabela.Valores;
                ViewBag.Alterar = true;
            }
            catch (Exception)
            {
                throw;
            }
            return View("TelaDinamica", dadosTabela);
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<FileResult> DownloadHtml(string database, string schema, string tabela)
        {
            var tabelaCore = new TabelaCore(tabela, schema, database);
            tabelaCore.Carregar();
            tabelaCore.CarregarValores(true);
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

        public IActionResult GravarItem(IFormCollection formCollection)
        {
            var lista = formCollection.ToList();
            var valores = new Dictionary<string, string>();

            foreach (var item in lista)
            {
                valores.Add(item.Key, item.Value[0]);
            }
            var tabela = new TabelaCore(valores["Tabela"], valores["Schema"], valores["Database"]);
            tabela.Carregar();
            tabela.AlterarRegistro(valores);

            return RedirectToAction("Index", new { database = valores["Database"], schema = valores["Schema"], tabela = valores["Tabela"] });
        }
        #endregion
    }
}