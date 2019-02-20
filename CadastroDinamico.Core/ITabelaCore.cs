using CadastroDinamico.Dominio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CadastroDinamico.Core
{
    public interface ITabelaCore : IDisposable
    {
        string Nome { get; set; }
        string Database { get; set; }
        string Schema { get; set; }
        bool TemChavePrimaria { get; set; }
        bool TemChaveEstrangeira { get; }
        int QuantidadeCampos { get; set; }
        List<Coluna> Colunas { get; set; }
        List<Coluna> TodasColunas { get; set; }
        List<object> Valores { get; set; }
        MatrizValores ValoresMultilinha { get; set; }
        List<Coluna> ChavesPrimarias { get; set; }
        int QuantidadeColunas { get; set; }
        int QuantidadeLinhas { get; set; }
        int IdServidor { get; set; }
        bool IsIdentity { get; set; }
        List<Coluna> ColunasFiltro { get; set; }
        object[,] ConsultaDados { get; set; }

        Task<string> CarregarAsync(string tabela, string schema, string database, int idServidor, bool carregarFks = true);
        string MontarUrlChavesPrimarias(int linha);
        Task CarregarValoresAsync(bool amostra = false, string pk = "");
        Task CarregarValoresAsync(string query);
        Task<string> RetornarSelectAsync(string where, bool amostra, bool limitarUm);
        string RetornarUpdate(string pk, Dictionary<string, object> valores);
        Task<string> RetornarInsert(Dictionary<string, object> valores);
        string GetPk();
        Task<string> AlterarRegistroAsync(Dictionary<string, string> valores);
        Task<string> InserirRegistroAsync(Dictionary<string, string> valores);
        Task<string> PesquisarRegistrosAsync(Dictionary<string, string> valores);
        Task SalvarConfiguracoesColunasAsync(List<string> colunasVisiveis, List<string> colunasChave, List<string> colunasFiltro);
        Task<List<string>> RetornarColunasVisiveisAsync();
        Task<List<string>> RetornarColunasFiltroAsync();
        Task<int> RetornarIdConfiguracaoTabelaAsync();
        Task CarregarColunasFiltro();
        string MontarWhere(Dictionary<string, object> valores);
    }
}
