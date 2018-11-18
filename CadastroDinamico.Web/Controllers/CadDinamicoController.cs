using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Repo = CadastroDinamico.Repositorio.SqlClient;
using System.Linq;
using CadastroDinamico.Core;
using System;
using Microsoft.AspNetCore.Http;

namespace CadastroDinamico.Web.Controllers
{
    public class CadDinamicoController : Controller
    {
        public IActionResult Index(string database, string schema, string tabela)
        {
            ViewBag.Title = string.Format("{0}.{1}.{2}", database, schema, tabela).ToUpper();
            return View();
        }

        #region Teste
        public IActionResult TelaDinamica(string database, string schema, string tabela)
        {
            var repositorio = new Repo.Repositorio();
            var dadosTabela = new TabelaCore(tabela, schema, database);
            
            try
            {
                var result = dadosTabela.Carregar();
                if (!string.IsNullOrEmpty(result))
                {
                    return View("Error", "Home");
                }
                var query = dadosTabela.RetornarSelect("", true);
                var valoresColunas = new Repo.Conexao(new Repo.Repositorio().RetornarConnectionString()).RetornarDados(query);
                for (int cont = 0; cont < valoresColunas.Columns.Count; cont++)
                {
                    dadosTabela.Valores.Add(valoresColunas.Rows[0][cont]);
                }
                ViewBag.Valores = dadosTabela.Valores;
            }
            catch (Exception)
            {
                throw;
            }
            return View(dadosTabela);
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<FileResult> DownloadHtml(string database, string schema, string tabela)
        {
            var path = string.Format("DownloadFiles\\{0}\\", DateTime.Now.ToString("yyyy-MM-dd_HHmmss"));
            var fileName = tabela + ".html";

            System.IO.Directory.CreateDirectory(path);

            await new Utils.Arquivo().EscreverEmArquivoAsync(path + fileName, "teste", false);

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
            return View();
        }
        #endregion
    }
}