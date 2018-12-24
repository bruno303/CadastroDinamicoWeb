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

namespace CadastroDinamico.Web.Controllers
{
    public class ConfiguracaoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SalvarConfiguracoesBancoDados(string values)
        {
            var configurador = new ConfiguradorBancoDados();
            var json = new UJson();

            try
            {
                BancoDados configuracaoBancoDados = json.ConverterParaObjeto(values);
                configurador.AlterarConfiguracaoBancoDados(configuracaoBancoDados);
                new ConfiguradorCadastroDinamico().CriarDatabaseAplicacao();
            }
            catch(System.Exception ex)
            {
                return Json(new { result = false, error = ex.Message });
            }

            return Json(new { result = true, error = string.Empty });
        }

        public JsonResult SelecionarDatabases()
        {
            Repo.Repositorio repositorio = new Repo.Repositorio();
            var consulta = repositorio.RetornarDatabases();
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

        public JsonResult SelecionarSchemas(string database)
        {
            Repo.Repositorio repositorio = new Repo.Repositorio();
            var consulta = repositorio.RetornarSchemas(database);
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

        public JsonResult SelecionarTabelas(string database, string schema)
        {
            Repo.Repositorio repositorio = new Repo.Repositorio();
            var consulta = repositorio.RetornarTabelas(database, schema);
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

        public JsonResult SelecionarColunas(string database, string schema, string tabela)
        {
            List<ColunaNomeViewModel> colunas = new List<ColunaNomeViewModel>();
            var tabelaCore = new TabelaCore(tabela, schema, database);
            tabelaCore.Carregar();

            var colVisiveisList = tabelaCore.RetornarColunasVisiveis();
            var colFiltroList = tabelaCore.RetornarColunasFiltro();
            var id = tabelaCore.RetornarIdConfiguracaoTabela();

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

        [HttpPost]
        public JsonResult GravarConfiguracoesTabela(string dados, string dadosfk, string dadosfiltro)
        {
            try
            {
                Repo.Repositorio repositorio = new Repo.Repositorio();
                if (!string.IsNullOrEmpty(dados))
                {
                    var dadosBanco = dados.Split("|")[0];
                    var colunas = dados.Split("|")[1];
                    var database = dadosBanco.Split(";")[0];
                    var schema = dadosBanco.Split(";")[1];
                    var tabela = dadosBanco.Split(";")[2];

                    var tabelaCore = new TabelaCore(tabela, schema, database);
                    tabelaCore.Carregar();

                    tabelaCore.SalvarConfiguracoesColunas(colunas.Split(";").ToList(), dadosfk?.Split(";").ToList(), dadosfiltro?.Split(";").ToList());
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

        public JsonResult SelecionarColunasChaveEstrangeira(string database, string schema, string tabela)
        {
            Repo.Repositorio repositorio = new Repo.Repositorio();
            var consulta = repositorio.RetornarColunas(database, schema, tabela).Where(p => p.IsChaveEstrangeira).ToList();
            List<ColunaChaveEstrangeiraViewModel> colunas = new List<ColunaChaveEstrangeiraViewModel>();

            var id = repositorio.SelecionarIdConfiguracaoTabela(database, schema, tabela);
            string colDescricao = string.Empty;

            List<string> colsList = null;

            if (id > 0)
            {
                colsList = repositorio.SelecionarColunasChaveEstrangeira(id);
            }

            if (consulta.Count > 0)
            {
                for (int contador = 0; contador < consulta.Count; contador++)
                {
                    var colunasTabelaReferenciada = repositorio.RetornarColunas(database, schema, consulta[contador].TabelaReferenciada).Select(p => p.Nome).ToList();
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
    }
}