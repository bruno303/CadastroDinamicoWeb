﻿using Microsoft.AspNetCore.Mvc;
using CadastroDinamico.Dominio;
using CadastroDinamico.Repositorio.SqlClient;
using UJson = CadastroDinamico.Utils.Json<CadastroDinamico.Dominio.BancoDados>;
using Repo = CadastroDinamico.Repositorio.SqlClient;
using CadastroDinamico.Core;
using System.Collections.Generic;
using CadastroDinamico.Web.Models;
using System.Linq;
using System;
using CadastroDinamico.Web.Extension;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CadastroDinamico.Web.Controllers
{
    public class ConfiguracaoController : Controller
    {
        public IActionResult Index()
        {
            if (this.ValidarLogin())
            {
                var idServidor = HttpContext.Session.GetInt32("idServidor");
                if ((!idServidor.HasValue) || idServidor.Value == 0)
                {
                    return RedirectToAction("CustomError", "Home", new { mensagem = "O ID do servidor não foi definido!" });
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public async Task<JsonResult> SelecionarDatabases()
        {
            if (this.ValidarLogin())
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
            else
            {
                return Json(new { result = false, error = "É necessário fazer login na aplicação" }); ;
            }
        }

        public async Task<JsonResult> SelecionarSchemas(string database)
        {
            if (this.ValidarLogin())
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
            else
            {
                return Json(new { result = false, error = "É necessário fazer login na aplicação" }); ;
            }
        }

        public async Task<JsonResult> SelecionarTabelas(string database, string schema)
        {
            if (this.ValidarLogin())
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
            else
            {
                return Json(new { result = false, error = "É necessário fazer login na aplicação" }); ;
            }
        }

        public async Task<JsonResult> SelecionarColunas(string database, string schema, string tabela)
        {
            if (this.ValidarLogin())
            {
                var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
                List<ColunaNomeViewModel> colunas = new List<ColunaNomeViewModel>();
                var tabelaCore = new TabelaCore(tabela, schema, database, idServidor);
                await tabelaCore.CarregarAsync();

                var colVisiveisList = await tabelaCore.RetornarColunasVisiveisAsync();
                var colFiltroList = await tabelaCore.RetornarColunasFiltroAsync();
                var id = await tabelaCore.RetornarIdConfiguracaoTabelaAsync();

                for (int contador = 0; contador < tabelaCore.TodasColunas.Count; contador++)
                {
                    colunas.Add(new ColunaNomeViewModel()
                    {
                        Name = tabelaCore.TodasColunas[contador].Nome,
                        Visivel = (id <= 0) || (colVisiveisList?.Contains(tabelaCore.TodasColunas[contador].Nome.ToUpper()) ?? false),
                        PodeOcultar = tabelaCore.TodasColunas[contador].AceitaNull,
                        Filtro = (id <= 0) || (colFiltroList?.Contains(tabelaCore.TodasColunas[contador].Nome.ToUpper()) ?? false)
                    });
                }

                return Json(colunas);
            }
            else
            {
                return Json(new { result = false, error = "É necessário fazer login na aplicação" }); ;
            }
        }

        [HttpPost]
        public async Task<JsonResult> GravarConfiguracoesTabela(string dados, string dadosfk, string dadosfiltro)
        {
            if (this.ValidarLogin())
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

                        var tabelaCore = new TabelaCore(tabela, schema, database, idServidor);
                        await tabelaCore.CarregarAsync();

                        await tabelaCore.SalvarConfiguracoesColunasAsync(colunas.Split(";").ToList(), dadosfk?.Split(";").ToList(), dadosfiltro?.Split(";").ToList());
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
            else
            {
                return Json(new { result = false, message = "É necessário fazer login na aplicação" }); ;
            }
        }

        public async Task<JsonResult> SelecionarColunasChaveEstrangeira(string database, string schema, string tabela)
        {
            if (this.ValidarLogin())
            {
                var idServidor = HttpContext.Session.GetInt32("idServidor").Value;
                Repo.Repositorio repositorio = new Repo.Repositorio(idServidor);
                var consulta = (await repositorio.RetornarColunasAsync(database, schema, tabela)).Where(p => p.IsChaveEstrangeira).ToList();
                List<ColunaChaveEstrangeiraViewModel> colunas = new List<ColunaChaveEstrangeiraViewModel>();

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
                        var colunasTabelaReferenciada = (await repositorio.RetornarColunasAsync(database, schema, consulta[contador].TabelaReferenciada)).Select(p => p.Nome).ToList();
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
            else
            {
                return Json(new { result = false, error = "É necessário fazer login na aplicação" }); ;
            }
        }
    }
}