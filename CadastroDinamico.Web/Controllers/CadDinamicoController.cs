using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CadastroDinamico.Core;
using CadastroDinamico.Repositorio.Interface;
using CadastroDinamico.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CadastroDinamico.Web.Controllers
{
    public class CadDinamicoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult SelecionarDatabases()
        {
            IRepositorio repositorio = new Repositorio.SqlClient.Repositorio();
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
            IRepositorio repositorio = new Repositorio.SqlClient.Repositorio();
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
            IRepositorio repositorio = new Repositorio.SqlClient.Repositorio();
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
            IRepositorio repositorio = new Repositorio.SqlClient.Repositorio();
            var consulta = repositorio.RetornarColunas(database, schema, tabela);
            List<ColunaNomeViewModel> colunas = new List<ColunaNomeViewModel>();
            if (consulta.Count > 0)
            {
                for (int contador = 0; contador < consulta.Count; contador++)
                {
                    colunas.Add(new ColunaNomeViewModel() { Name = consulta[contador].Nome });
                }
            }

            return new JsonResult(colunas);
        }
    }
}