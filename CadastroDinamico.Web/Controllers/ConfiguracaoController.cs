using Microsoft.AspNetCore.Mvc;
using Repo = CadastroDinamico.Repositorio.SqlClient;
using CadastroDinamico.Core;
using System.Collections.Generic;
using CadastroDinamico.Web.Models;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CadastroDinamico.Web.Filters;

namespace CadastroDinamico.Web.Controllers
{
    [CadDinamicoAuth]
    public class ConfiguracaoController : Controller
    {
        public readonly ITabelaCore _tabelaCore;

        public ConfiguracaoController(ITabelaCore core)
        {
            _tabelaCore = core;
        }

        public async Task<IActionResult> Index()
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor");
            if ((!idServidor.HasValue) || idServidor.Value == 0)
            {
                return RedirectToAction("CustomError", "Home", new CustomErrorViewModel()
                {
                    IdErro = 0,
                    Titulo = string.Empty,
                    Mensagem = "O ID do servidor não foi definido!",
                    ComandoExecutado = string.Empty
                });
            }
            else
            {
                await new ConfiguradorCadastroDinamico().CriarDatabaseAplicacaoAsync(idServidor.Value);
            }
            return View();
        }

        public async Task<JsonResult> SelecionarDatabases()
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            Repo.Repositorio repositorio = new Repo.Repositorio(idServidor);
            var consulta = await repositorio.RetornarDatabasesAsync();
            List<DatabaseViewModel> databases = new List<DatabaseViewModel>();
            if (consulta.Count > 0)
            {
                for (int contador = 0; contador < consulta.Count; contador++)
                {
                    databases.Add(new DatabaseViewModel() { Name = consulta[contador].Nome });
                }
            }

            return new JsonResult(databases);
        }

        public async Task<JsonResult> SelecionarSchemas(string database)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            Repo.Repositorio repositorio = new Repo.Repositorio(idServidor);
            var consulta = await repositorio.RetornarSchemasAsync(database);
            List<SchemaViewModel> schemas = new List<SchemaViewModel>();
            if (consulta.Count > 0)
            {
                for (int contador = 0; contador < consulta.Count; contador++)
                {
                    schemas.Add(new SchemaViewModel() { Name = consulta[contador].Nome });
                }
            }

            return new JsonResult(schemas);
        }

        public async Task<JsonResult> SelecionarTabelas(string database, string schema)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            Repo.Repositorio repositorio = new Repo.Repositorio(idServidor);
            var consulta = await repositorio.RetornarTabelasAsync(database, schema);
            List<SchemaViewModel> schemas = new List<SchemaViewModel>();
            if (consulta.Count > 0)
            {
                for (int contador = 0; contador < consulta.Count; contador++)
                {
                    schemas.Add(new SchemaViewModel() { Name = consulta[contador].Nome });
                }
            }
            return new JsonResult(schemas);
        }

        public async Task<JsonResult> SelecionarColunas(string database, string schema, string tabela)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            List<ColunaNomeViewModel> colunas = new List<ColunaNomeViewModel>();
            await _tabelaCore.CarregarAsync(tabela, schema, database, idServidor);

            var colVisiveisList = await _tabelaCore.RetornarColunasVisiveisAsync();
            var colFiltroList = await _tabelaCore.RetornarColunasFiltroAsync();
            var id = await _tabelaCore.RetornarIdConfiguracaoTabelaAsync();

            for (int contador = 0; contador < _tabelaCore.TodasColunas.Count; contador++)
            {
                colunas.Add(new ColunaNomeViewModel()
                {
                    Name = _tabelaCore.TodasColunas[contador].Nome,
                    Visivel = (id <= 0) || (colVisiveisList?.Contains(_tabelaCore.TodasColunas[contador].Nome.ToUpper()) ?? false),
                    PodeOcultar = _tabelaCore.TodasColunas[contador].AceitaNull,
                    Filtro = (id <= 0) || (colFiltroList?.Contains(_tabelaCore.TodasColunas[contador].Nome.ToUpper()) ?? false)
                });
            }

            return Json(colunas);
        }

        [HttpPost]
        public async Task<JsonResult> GravarConfiguracoesTabela(string dados, string dadosfk, string dadosfiltro)
        {
            try
            {
                var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
                Repo.Repositorio repositorio = new Repo.Repositorio(idServidor);
                if (!string.IsNullOrEmpty(dados))
                {
                    var dadosBanco = dados.Split("|")[0];
                    var colunas = dados.Split("|")[1];
                    var database = dadosBanco.Split(";")[0];
                    var schema = dadosBanco.Split(";")[1];
                    var tabela = dadosBanco.Split(";")[2];

                    await _tabelaCore.CarregarAsync(tabela, schema, database, idServidor);

                    await _tabelaCore.SalvarConfiguracoesColunasAsync(colunas.Split(";").ToList(), dadosfk?.Split(";").ToList(), dadosfiltro?.Split(";").ToList());
                }
                else
                {
                    return Json(new { result = false, message = "Dados vazios!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = ex.Message });
            }

            return Json(new { result = true, message = string.Empty });
        }

        public async Task<JsonResult> SelecionarColunasChaveEstrangeira(string database, string schema, string tabela)
        {
            var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
            Repo.Repositorio repositorio = new Repo.Repositorio(idServidor);
            var consulta = (await repositorio.RetornarColunasAsync(database, schema, tabela)).Where(p => p.IsChaveEstrangeira).ToList();
            List<ColunaChaveEstrangeiraViewModel> colunas = new List<ColunaChaveEstrangeiraViewModel>();

            await _tabelaCore.CarregarAsync(tabela, schema, database, idServidor);

            var id = await repositorio.SelecionarIdConfiguracaoTabelaAsync(database, schema, tabela);
            string colDescricao = string.Empty;

            List<string> colsList = null;

            if (id > 0)
            {
                colsList = await repositorio.SelecionarColunasChaveEstrangeiraAsync(id);
            }

            if (consulta.Count > 0)
            {
                for (int contador = 0; contador < consulta.Count; contador++)
                {
                    var tabelaReferenciadaChavePrimaria = _tabelaCore.Colunas.Where(c => c.Nome == consulta[contador].Nome).FirstOrDefault()?.TabelaReferenciadaChavePrimaria;
                    var colunasTabelaReferenciada = (await repositorio.RetornarColunasAsync(database, schema, tabelaReferenciadaChavePrimaria)).Select(p => p.Nome).ToList();
                    if (colsList != null)
                    {
                        if (colsList.Where(p => p.Split(":")[0].Equals(consulta[contador].Nome)).FirstOrDefault() != null)
                        {
                            colDescricao = colsList.Where(p => p.Split(":")[0].Equals(consulta[contador].Nome)).FirstOrDefault().Split(":")[1];
                        }
                        else
                        {
                            colDescricao = consulta[contador].Nome;
                        }
                    }
                    colunas.Add(new ColunaChaveEstrangeiraViewModel()
                    {
                        Nome = consulta[contador].Nome,
                        TabelaReferenciada = consulta[contador].TabelaReferenciada,
                        ColunaReferenciada = consulta[contador].ColunaReferenciada,
                        ColunasTabelaReferenciada = colunasTabelaReferenciada,
                        IndiceColTabelaReferenciada = colunasTabelaReferenciada.IndexOf(colDescricao) == -1 ? 0 : colunasTabelaReferenciada.IndexOf(colDescricao)
                    });
                }
            }

            return Json(colunas);
        }

        protected override void Dispose(bool disposing)
        {
            _tabelaCore.Dispose();
            base.Dispose(disposing);
        }
    }
}