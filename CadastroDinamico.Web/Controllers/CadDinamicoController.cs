using System.Collections.Generic;
using Repo = CadastroDinamico.Repositorio.SqlClient;
using CadastroDinamico.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
            Repo.Repositorio repositorio = new Repo.Repositorio();
            List<ColunaNomeViewModel> colunas = new List<ColunaNomeViewModel>();
            var id = repositorio.SelecionarIdConfiguracaoTabela(database, schema, tabela);
            string colVisiveis = string.Empty;
            List<string> colVisiveisList = null;

            if (id > 0)
            {
                colVisiveis = repositorio.SelecionarColunasVisiveis(id);
                if (!string.IsNullOrEmpty(colVisiveis))
                {
                    colVisiveisList = colVisiveis.Split(";").ToList();
                }
            }

            var consulta = repositorio.RetornarColunas(database, schema, tabela);
            if (consulta.Count > 0)
            {
                for (int contador = 0; contador < consulta.Count; contador++)
                {
                    colunas.Add(new ColunaNomeViewModel()
                    {
                        Name = consulta[contador].Nome,
                        Visivel = (colVisiveisList?.Contains(consulta[contador].Nome) ?? false)
                    });
                }
            }

            return Json(colunas);
        }

        public JsonResult GravarConfiguracoesTabela(string dados, string dadosfk)
        {
            Repo.Repositorio repositorio = new Repo.Repositorio();
            if (!string.IsNullOrEmpty(dados))
            {
                var dadosBanco = dados.Split("|")[0];
                var colunas = dados.Split("|")[1];
                var database = dadosBanco.Split(";")[0];
                var schema = dadosBanco.Split(";")[1];
                var tabela = dadosBanco.Split(";")[2];

                var id = repositorio.SelecionarIdConfiguracaoTabela(database, schema, tabela);
                repositorio.SalvarConfiguracoesTabela(id, database, schema, tabela, colunas, dadosfk);
            }

            return Json(null);
        }

        public JsonResult SelecionarColunasChaveEstrangeira(string database, string schema, string tabela)
        {
            Repo.Repositorio repositorio = new Repo.Repositorio();
            var consulta = repositorio.RetornarColunas(database, schema, tabela).Where(p => p.IsChaveEstrangeira).ToList();
            List<ColunaChaveEstrangeiraViewModel> colunas = new List<ColunaChaveEstrangeiraViewModel>();

            var id = repositorio.SelecionarIdConfiguracaoTabela(database, schema, tabela);
            string cols = string.Empty;
            string colDescricao = string.Empty;

            List<string> colsList = null;

            if (id > 0)
            {
                cols = repositorio.SelecionarColunasChaveEstrangeira(id);
                if (!string.IsNullOrEmpty(cols))
                {
                    colsList = cols.Split(";").ToList();
                }
            }

            if (consulta.Count > 0)
            {
                for (int contador = 0; contador < consulta.Count; contador++)
                {
                    var colunasTabelaReferenciada = repositorio.RetornarColunas(database, schema, consulta[contador].TabelaReferenciada).Select(p => p.Nome).ToList();
                    if (colsList != null)
                    {
                        colDescricao = colsList.Where(p => p.Split(":")[0].Equals(consulta[contador].Nome)).FirstOrDefault().Split(":")[1];
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