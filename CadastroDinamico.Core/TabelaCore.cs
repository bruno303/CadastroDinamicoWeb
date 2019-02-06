using CadastroDinamico.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlClient = CadastroDinamico.Repositorio.SqlClient;

namespace CadastroDinamico.Core
{
    public class TabelaCore : ITabelaCore
    {
        public string Nome { get; set; }
        public string Database { get; set; }
        public string Schema { get; set; }
        public bool TemChavePrimaria { get; set; }
        public bool TemChaveEstrangeira { get; set; }
        public int QuantidadeCampos { get; set; }
        public List<Coluna> Colunas { get; set; }
        public List<Coluna> TodasColunas { get; set; }
        public List<object> Valores { get; set; }
        public MatrizValores ValoresMultilinha { get; set; }
        public List<Coluna> ChavesPrimarias { get; set; }
        public int QuantidadeColunas { get; set; }
        public int QuantidadeLinhas { get; set; }
        public int IdServidor { get; set; }
        public bool IsIdentity { get; set; }
        public List<Coluna> ColunasFiltro { get; set; }
        public object[,] ConsultaDados { get; set; }

        private List<string> camposExibir;
        private string pkAlteracao;

        public async Task<string> CarregarAsync(string tabela, string schema, string database, int idServidor)
        {
            Nome = tabela;
            Schema = schema;
            Database = database;
            IdServidor = idServidor;
            Valores = new List<object>();

            string retorno = string.Empty;
            SqlClient.Repositorio repositorio = new SqlClient.Repositorio(IdServidor);

            if (string.IsNullOrEmpty(Database))
            {
                throw new ArgumentException("Propriedade Database é obrigatória.");
            }

            if (string.IsNullOrEmpty(Nome))
            {
                throw new ArgumentException("Propriedade Nome da tabela é obrigatória.");
            }

            if (string.IsNullOrEmpty(Schema))
            {
                Schema = "DBO";
            }

            try
            {
                Valores = null;
                ValoresMultilinha = null;
                ConsultaDados = null;
                pkAlteracao = string.Empty;

                Colunas = await repositorio.RetornarColunasAsync(Database, Schema, Nome);
                TodasColunas = new List<Coluna>();
                TodasColunas.AddRange(Colunas);
                CarregarNomesInput();
                TemChavePrimaria = Colunas.Where(p => p.IsChavePrimaria).FirstOrDefault() != null;
                TemChaveEstrangeira = Colunas.Where(p => p.IsChaveEstrangeira).FirstOrDefault() != null;
                camposExibir = await repositorio.SelecionarColunasVisiveisAsync(Database, Schema, Nome);
                ChavesPrimarias = await repositorio.RetornarColunasChavePrimariaTabelaAsync(Nome, Schema, Database);
                RemoverColunasIgnoradas();
                QuantidadeCampos = Colunas.Count;
                await CarregarColunasChaveEstrangeiraAsync();
                IsIdentity = Colunas.FirstOrDefault().IsIdentity;
                await CarregarColunasFiltro();
            }
            catch (Exception ex)
            {
                retorno = ex.Message;
            }

            return retorno;
        }

        private void CarregarNomesInput()
        {
            for (int cont = 0; cont < Colunas.Count; cont++)
            {
                Colunas[cont].NomeInput = "inputColuna" + cont.ToString();
            }
        }

        private async Task CarregarColunasChaveEstrangeiraAsync()
        {
            /* Carrega SelectList para os campos de chave estrangeira */
            for (int cont = 0; cont < Colunas.Count; cont++)
            {
                if (Colunas[cont].IsChaveEstrangeira)
                {
                    Colunas[cont].ListaSelecao = await RetornarListaTabelaEstrangeira(Colunas[cont].TabelaReferenciada);
                }
            }
        }

        private void RemoverColunasIgnoradas()
        {
            int cont = 0;
            /* Remove campos que não serão manipulados */
            while (cont < Colunas.Count)
            {
                if (!(camposExibir.Count == 1 && camposExibir[0].Equals(string.Empty)))
                {
                    if (!camposExibir.Contains(Colunas[cont].Nome.ToUpper()))
                    {
                        Colunas.RemoveAt(cont);
                    }
                    else
                    {
                        cont++;
                    }
                }
                else
                {
                    cont++;
                }
            }
        }

        private async Task<List<TabelaEstrangeira>> RetornarListaTabelaEstrangeira(string tabela)
        {
            List<TabelaEstrangeira> itens = null;
            var repositorio = new SqlClient.Repositorio(IdServidor);
            string chavePrimaria = "";
            string descricao = "";

            try
            {
                chavePrimaria = (await repositorio.RetornarColunasChavePrimariaTabelaAsync(tabela, Schema, Database))[0].Nome;
                descricao = await repositorio.SelecionarDescricaoChaveEstrangeiraConfiguracaoTabelaAsync(Database, Schema, Nome, chavePrimaria);
                itens = await repositorio.SelectTabelaAsync(chavePrimaria, descricao, tabela, Schema, Database);
            }
            catch (Exception)
            {
                throw;
            }
            return itens;
        }

        private string MontarWhereChavesPrimarias(string pk, bool useAlias = true)
        {
            var retorno = string.Empty;
            if (!string.IsNullOrEmpty(pk) && pk.IndexOf(":") > 0)
            {
                var listaPks = pk.Split(";");
                if (listaPks.Length > 0)
                {
                    foreach (var item in listaPks)
                    {
                        var chaveValor = item.Split(":");
                        if (retorno != string.Empty)
                        {
                            retorno += " AND ";
                        }
                        if (useAlias)
                        {
                            retorno += $"AL0.{chaveValor[0]} = {chaveValor[1]}";
                        }
                        else
                        {
                            retorno += $"{chaveValor[0]} = {chaveValor[1]}";
                        }
                    }
                }
            }

            return retorno;
        }

        public string MontarUrlChavesPrimarias(int linha)
        {
            var retorno = string.Empty;

            for (int cont = 0; cont < Colunas.Count; cont++)
            {
                if (Colunas[cont].IsChavePrimaria)
                {
                    retorno += Colunas[cont].Nome + ":" + ValoresMultilinha.GetValor(linha, cont) + ";";
                }
            }

            if (!string.IsNullOrEmpty(retorno))
            {
                retorno = retorno.Substring(0, retorno.Length - 1);
            }

            return retorno;
        }

        public async Task CarregarValoresAsync(bool amostra = false, string pk = "")
        {
            var repositorio = new SqlClient.Repositorio(IdServidor);
            var query = string.Empty;
            var where = string.Empty;

            try
            {
                where = MontarWhereChavesPrimarias(pk);
                query = await RetornarSelectAsync(where, amostra, true);

                if (amostra)
                {
                    ValoresMultilinha = await repositorio.RetornarValoresQueryAsync(query);
                    ConsultaDados = await repositorio.RetornarValoresMultilinha(query);
                }
                else
                {
                    Valores = await repositorio.RetornarValoresAsync(query);
                    pkAlteracao = pk;
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task CarregarValoresAsync(string query)
        {
            var repositorio = new SqlClient.Repositorio(IdServidor);
            var where = string.Empty;

            try
            {
                ValoresMultilinha = await repositorio.RetornarValoresQueryAsync(query);
                ConsultaDados = await repositorio.RetornarValoresMultilinha(query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> RetornarSelectAsync(string where, bool amostra, bool limitarUm)
        {
            var repositorio = new SqlClient.Repositorio(IdServidor);
            string query = string.Empty;
            int countTabelasEstrangeiras = 0;
            const string prefixoAlias = "AL";

            var id = await repositorio.SelecionarIdConfiguracaoTabelaAsync(Database, Schema, Nome);
            string colDescricao = string.Empty;

            List<string> colsList = null;

            /* Dá TOP(10) quando não tiver where definido */
            if (!limitarUm && where == string.Empty)
            {
                amostra = true;
            }

            if (id > 0)
            {
                colsList = await repositorio.SelecionarColunasChaveEstrangeiraAsync(id);
            }
            query = "SELECT " + (amostra ? "TOP(10) " : (limitarUm ? "TOP(1) " : ""));
            /* Montar Select da PK */
            if (Colunas.Where(p => p.IsChavePrimaria).Count() > 0)
            {
                for (int i = 0; i < Colunas.Count; i++)
                {
                    if (Colunas[i].IsChavePrimaria)
                    {
                        query += $"'{Colunas[i].Nome}:' + CAST(AL0.{Colunas[i].Nome} AS VARCHAR) + ';' +";
                    }
                }
                query = query.Remove(query.Length - 8, 8);

                query += " AS PK, ";
            }
            else
            {
                query += "'' AS PK, ";
            }

            for (int i = 0; i < Colunas.Count; i++)
            {
                if (Colunas[i].IsChaveEstrangeira)
                {
                    countTabelasEstrangeiras++;
                    if (colsList != null)
                    {
                        if (colsList.Where(p => p.Split(":")[0].Equals(Colunas[i].Nome)).FirstOrDefault() != null)
                        {
                            colDescricao = colsList.Where(p => p.Split(":")[0].Equals(Colunas[i].Nome)).FirstOrDefault().Split(":")[1];
                        }
                        else
                        {
                            colDescricao = Colunas[i].Nome;
                        }
                    }

                    query += $"{prefixoAlias + countTabelasEstrangeiras.ToString()}.{colDescricao}, ";
                }
                else
                {
                    query += prefixoAlias + "0." + Colunas[i].Nome + ", ";
                }
            }
            query = query.Remove(query.Length - 2, 2);
            query += string.Format(" FROM {0}.{1}.{2} {3} WITH(NOLOCK)", Database, Schema, Nome, prefixoAlias + "0");
            if (countTabelasEstrangeiras > 0)
            {
                countTabelasEstrangeiras = 0;
                for (int i = 0; i < Colunas.Count; i++)
                {
                    if (Colunas[i].IsChaveEstrangeira)
                    {
                        countTabelasEstrangeiras++;
                        query += $" LEFT JOIN {Database}.{Schema}.{Colunas[i].TabelaReferenciada} {prefixoAlias + countTabelasEstrangeiras.ToString()} ";
                        query += $"     WITH(NOLOCK) ON {prefixoAlias + countTabelasEstrangeiras.ToString()}.{Colunas[i].ColunaReferenciada} = {prefixoAlias + "0"}.{Colunas[i].Nome}";
                    }
                }
            }
            
            if (!string.IsNullOrWhiteSpace(where))
            {
                query += " WHERE " + where;
            }

            return query;
        }

        public string RetornarUpdate(string pk, Dictionary<string, object> valores)
        {
            string query = string.Empty;

            query = string.Format("UPDATE {0}.{1}.{2} SET ", Database, Schema, Nome);
            for (int cont = 0; cont < Colunas.Count; cont++)
            {
                if (!Colunas[cont].IsChavePrimaria)
                {
                    string valorParametro = valores[Colunas[cont].Nome].ToString();

                    if (valorParametro.ToUpper() != "NULL")
                    {
                        if (Colunas[cont].Tipo.ToUpper() == "DATE" ||
                                Colunas[cont].Tipo.ToUpper() == "DATETIME" ||
                                Colunas[cont].Tipo.ToUpper() == "DATETIME2")
                        {
                            valorParametro = AjustarFormatoData(valorParametro);
                        }

                        if (Colunas[cont].Tipo.ToUpper() == "VARCHAR" ||
                            Colunas[cont].Tipo.ToUpper() == "CHAR" ||
                            Colunas[cont].Tipo.ToUpper() == "NVARCHAR" ||
                            Colunas[cont].Tipo.ToUpper() == "NCHAR" ||
                            Colunas[cont].Tipo.ToUpper() == "DATE" ||
                            Colunas[cont].Tipo.ToUpper() == "DATETIME" ||
                            Colunas[cont].Tipo.ToUpper() == "TIME" ||
                            Colunas[cont].Tipo.ToUpper() == "TIMESPAN" ||
                            Colunas[cont].Tipo.ToUpper() == "DATETIME2")
                        {
                            valorParametro = "'" + valorParametro + "'";
                        }
                        else if (Colunas[cont].Tipo.ToUpper() == "BIT")
                        {
                            if (valorParametro.ToUpper() == bool.TrueString.ToUpper() || valorParametro.ToUpper() == "ON")
                            {
                                valorParametro = "1";
                            }
                            else
                            {
                                valorParametro = "0";
                            }
                        }
                    }
                    query += string.Format("{0} = {1}, ", Colunas[cont].Nome, valorParametro);
                }
                else  if (Colunas[cont].IsChaveEstrangeira)
                {
                    query += string.Format("{0} = {1}, ", Colunas[cont].Nome, valores[Colunas[cont].Nome].ToString());
                }
            }
            query = query.Substring(0, query.Length - 2);

            if (!string.IsNullOrWhiteSpace(pk))
            {
                query += " WHERE " + MontarWhereChavesPrimarias(pk, false);
            }

            return query;
        }

        public async Task<string> RetornarInsert(Dictionary<string, object> valores)
        {
            string query = string.Empty;
            var repositorio = new SqlClient.Repositorio(IdServidor);

            /* Monta cabeçalho */
            query = string.Format("INSERT INTO {0}.{1}.{2} ( ", Database, Schema, Nome);
            for (int cont = 0; cont < Colunas.Count; cont++)
            {
                if (!Colunas[cont].IsChavePrimaria)
                {
                    query += Colunas[cont].Nome + ", ";
                }
                else if (Colunas[cont].IsChaveEstrangeira)
                {
                    query += Colunas[cont].Nome + ", ";
                }
                else if (!IsIdentity)
                {
                    query += Colunas[cont].Nome + ", ";
                }
            }
            query = query.Substring(0, query.Length - 2);
            query += " ) VALUES ( ";

            /* Monta valores */
            for (int cont = 0; cont < Colunas.Count; cont++)
            {
                if (!Colunas[cont].IsChavePrimaria)
                {
                    string valorParametro = valores[Colunas[cont].Nome].ToString();

                    if (valorParametro.ToUpper() != "NULL")
                    {
                        if (Colunas[cont].Tipo.ToUpper() == "DATE" ||
                                Colunas[cont].Tipo.ToUpper() == "DATETIME" ||
                                Colunas[cont].Tipo.ToUpper() == "DATETIME2")
                        {
                            valorParametro = AjustarFormatoData(valorParametro);
                        }

                        if (Colunas[cont].Tipo.ToUpper() == "VARCHAR" ||
                            Colunas[cont].Tipo.ToUpper() == "CHAR" ||
                            Colunas[cont].Tipo.ToUpper() == "NVARCHAR" ||
                            Colunas[cont].Tipo.ToUpper() == "NCHAR" ||
                            Colunas[cont].Tipo.ToUpper() == "DATE" ||
                            Colunas[cont].Tipo.ToUpper() == "DATETIME" ||
                            Colunas[cont].Tipo.ToUpper() == "TIME" ||
                            Colunas[cont].Tipo.ToUpper() == "TIMESPAN" ||
                            Colunas[cont].Tipo.ToUpper() == "DATETIME2")
                        {
                            valorParametro = "'" + valorParametro + "'";
                        }
                        else if (Colunas[cont].Tipo.ToUpper() == "BIT")
                        {
                            if (valorParametro.ToUpper() == bool.TrueString.ToUpper() || valorParametro.ToUpper() == "ON")
                            {
                                valorParametro = "1";
                            }
                            else
                            {
                                valorParametro = "0";
                            }
                        }
                    }
                    query += valorParametro + ", ";
                }
                else // Tratar chave primária
                {
                    if (Colunas[cont].IsChaveEstrangeira)
                    {
                        string valorParametro = valores[Colunas[cont].Nome].ToString();

                        query += valorParametro + ", ";
                    }
                    else if (!IsIdentity)
                    {
                        string valorParametro = (await repositorio.RetornarProximaChavePrimaria(Database, Schema, Nome,
                            Colunas[cont].Nome)).ToString();

                        query += valorParametro + ", ";
                    }
                }
            }
            query = query.Substring(0, query.Length - 2);
            query += " )";

            return query;
        }

        public string GetPk()
        {
            return pkAlteracao;
        }

        private string AjustarFormatoData(string data)
        {
            string dataCorreta = string.Empty;

            if (!string.IsNullOrWhiteSpace(data))
            {
                dataCorreta = data.Substring(6, 4) + "-" + data.Substring(3, 2) + "-" + data.Substring(0, 2) + data.Substring(10, data.Length - 10);
            }

            return dataCorreta;
        }

        public async Task<string> AlterarRegistroAsync(Dictionary<string, string> valores)
        {
            string retorno = string.Empty;
            try
            {
                SqlClient.Repositorio repositorio = new SqlClient.Repositorio(IdServidor);
                var pk = string.Empty;
                AlteracaoRegistroCore alteracaoRegistro = new AlteracaoRegistroCore(Colunas, valores, false);

                var valoresTratados = alteracaoRegistro.GetValoresTratados();
                pk = valores["pk"];

                var query = RetornarUpdate(pk, valoresTratados);
                retorno = await repositorio.AlterarValoresAsync(query);
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return retorno;
        }

        public async Task<string> InserirRegistroAsync(Dictionary<string, string> valores)
        {
            string retorno = string.Empty;
            try
            {
                SqlClient.Repositorio repositorio = new SqlClient.Repositorio(IdServidor);
                AlteracaoRegistroCore alteracaoRegistro = new AlteracaoRegistroCore(Colunas, valores, false);

                var valoresTratados = alteracaoRegistro.GetValoresTratados();

                var query = await RetornarInsert(valoresTratados);
                retorno = await repositorio.AlterarValoresAsync(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }

        public async Task<string> PesquisarRegistrosAsync(Dictionary<string, string> valores)
        {
            string retorno = string.Empty;
            try
            {
                SqlClient.Repositorio repositorio = new SqlClient.Repositorio(IdServidor);
                AlteracaoRegistroCore alteracaoRegistro = new AlteracaoRegistroCore(Colunas, valores, true);

                var valoresTratados = alteracaoRegistro.GetValoresTratados();

                var where = MontarWhere(valoresTratados);
                var query = await RetornarSelectAsync(where, false, false);

                await CarregarValoresAsync(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }

        public async Task SalvarConfiguracoesColunasAsync(List<string> colunasVisiveis, List<string> colunasChave, List<string> colunasFiltro)
        {
            try
            {
                var repositorio = new SqlClient.Repositorio(IdServidor);
                var idConfiguracaoTabela = await repositorio.SelecionarIdConfiguracaoTabelaAsync(Database, Schema, Nome);
                var idConfiguracaoTabelaColuna = 0;

                await repositorio.SalvarConfiguracoesTabelaAsync(idConfiguracaoTabela, Database, Schema, Nome);
                idConfiguracaoTabela = await repositorio.SelecionarIdConfiguracaoTabelaAsync(Database, Schema, Nome);

                foreach (var coluna in TodasColunas)
                {
                    var visivel = colunasVisiveis.Contains(coluna.Nome);
                    var filtro = colunasFiltro?.Contains(coluna.Nome) ?? false;
                    var chave = string.Empty;
                    if (colunasChave != null)
                    {
                        foreach (var item in colunasChave)
                        {
                            if (chave == string.Empty)
                            {
                                if (item.Split(":")[0] == coluna.Nome)
                                {
                                    chave = item.Split(":")[1];
                                }
                            }
                        }   
                    }
                    idConfiguracaoTabelaColuna = await repositorio.SelecionarIdConfiguracaoTabelaColunaAsync(Database, Schema, Nome, coluna.Nome);
                    await repositorio.SalvarConfiguracaoColunaAsync(idConfiguracaoTabelaColuna, idConfiguracaoTabela, coluna.Nome, visivel, chave, filtro);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<string>> RetornarColunasVisiveisAsync()
        {
            var repositorio = new SqlClient.Repositorio(IdServidor);
            List<string> colVisiveisList = new List<string>();
            var id = await repositorio.SelecionarIdConfiguracaoTabelaAsync(Database, Schema, Nome);

            if (id > 0)
            {
                colVisiveisList = await repositorio.SelecionarColunasVisiveisAsync(id);
            }


            return colVisiveisList;
        }

        public async Task<List<string>> RetornarColunasFiltroAsync()
        {
            var repositorio = new SqlClient.Repositorio(IdServidor);
            List<string> colFiltroList = new List<string>();
            var id = await repositorio.SelecionarIdConfiguracaoTabelaAsync(Database, Schema, Nome);

            if (id > 0)
            {
                colFiltroList = await repositorio.SelecionarColunasFiltroAsync(id);
            }


            return colFiltroList;
        }

        public async Task<int> RetornarIdConfiguracaoTabelaAsync()
        {
            var repositorio = new SqlClient.Repositorio(IdServidor);
            return await repositorio.SelecionarIdConfiguracaoTabelaAsync(Database, Schema, Nome);
        }

        public async Task CarregarColunasFiltro()
        {
            var repositorio = new SqlClient.Repositorio(IdServidor);
            var colunasFiltro = await repositorio.SelecionarColunasFiltroAsync(Database, Schema, Nome);
            colunasFiltro.ForEach(col => col = col.ToUpper());
            ColunasFiltro = TodasColunas.Where(p => colunasFiltro.Contains(p.Nome.ToUpper())).ToList();
        }

        public string MontarWhere(Dictionary<string, object> valores)
        {
            var retorno = string.Empty;

            foreach (var coluna in ColunasFiltro)
            {
                if (valores.Keys.Contains(coluna.Nome) && valores[coluna.Nome].ToString() != string.Empty)
                {
                    if (retorno != string.Empty)
                    {
                        retorno += " AND ";
                    }

                    string valorParametro = valores[coluna.Nome].ToString();

                    if (coluna.Tipo.ToUpper() == "DATE" ||
                            coluna.Tipo.ToUpper() == "DATETIME" ||
                            coluna.Tipo.ToUpper() == "DATETIME2")
                    {
                        valorParametro = "'" + AjustarFormatoData(valorParametro) + "'";
                    }
                    else if (coluna.Tipo.ToUpper() == "TIME" || coluna.Tipo.ToUpper() == "TIMESPAN")
                    {
                        valorParametro = "'" + valorParametro + "'";
                    }
                    else if (coluna.Tipo.ToUpper() == "VARCHAR" ||
                        coluna.Tipo.ToUpper() == "CHAR" ||
                        coluna.Tipo.ToUpper() == "NVARCHAR" ||
                        coluna.Tipo.ToUpper() == "NCHAR")
                    {
                        valorParametro = "'%" + valorParametro + "%'";
                    }
                    else if (coluna.Tipo.ToUpper() == "BIT" && (valorParametro.ToUpper() == bool.TrueString.ToUpper() || valorParametro.ToUpper() == "ON"))
                    {
                        valorParametro = "1";
                    }

                    /* Concatena o parâmetro */
                    if (coluna.Tipo.ToUpper() == "VARCHAR" ||
                        coluna.Tipo.ToUpper() == "CHAR" ||
                        coluna.Tipo.ToUpper() == "NVARCHAR" ||
                        coluna.Tipo.ToUpper() == "NCHAR")
                    {
                        retorno += $"AL0.{coluna.Nome} LIKE {valorParametro}";
                    }
                    else
                    {
                        retorno += $"AL0.{coluna.Nome} = {valorParametro}";
                    }
                }
            }

            return retorno;
        }
    }
}
