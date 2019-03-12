using System.Threading.Tasks;
using CadastroDinamico.Core;
using CadastroDinamico.Web.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CadastroDinamico.Web.Models;
using System.Collections.Generic;

namespace CadastroDinamico.Web.Controllers
{
    [CadDinamicoAuth]
    public class LogController : Controller
    {
        public readonly ITabelaCore _tabelaCore;

        public LogController(ITabelaCore core)
        {
            _tabelaCore = core;
        }

        public async Task<IActionResult> Index()
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            var viewModel = new List<LogViewModel>();

            var repositorio = new Repositorio.SqlClient.Repositorio(idServidor);
            var dadosLog = await repositorio.GetLogs();
            foreach (var item in dadosLog)
            {
                viewModel.Add(new LogViewModel()
                {
                    IdLog = item.IdLog,
                    Database = item.Database,
                    Schema = item.Schema,
                    Tabela = item.Tabela,
                    Metodo = item.Metodo,
                    Query = item.Query,
                    Usuario = item.Usuario,
                    DataHora = item.DataHora
                });
            }

            return View(viewModel);
        }
    }
}