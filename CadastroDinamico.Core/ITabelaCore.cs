using CadastroDinamico.Dominio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CadastroDinamico.Core
{
    public interface ITabelaCore : IDisposable
    {
        #region Propriedades
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
        #endregion

        #region Metodos
        /// <summary>
        /// Carrega todas a instância de TabelaCore com as informações da tabela.
        /// </summary>
        /// <param name="tabela">Nome da tabela.</param>
        /// <param name="schema">Nome do schema.</param>
        /// <param name="database">Nome da database (banco de dados).</param>
        /// <param name="idServidor">Id do servidor utilizado. Esse ID está cadastrado na tabela SERVIDOR no banco de dados SQLite.</param>
        /// <param name="carregarFks">Booleano indicando se as chaves estrangeiras devem ser carregadas.</param>
        /// <returns>String: texto de erro, se houver. Em caso de sucesso retorna string.Empty.</returns>
        Task<string> CarregarAsync(string tabela, string schema, string database, int idServidor, bool carregarFks = true);

        /// <summary>
        /// Monta a URL utilizada pela camada WEB com as chaves primárias de um registro.
        /// </summary>
        /// <param name="linha">Numero da linha do registro carregado.</param>
        /// <returns>String: Retorna a string montada com as chaves primárias e valores do registro.</returns>
        string MontarUrlChavesPrimarias(int linha);

        /// <summary>
        /// Carrega valores de registros da tabela.
        /// </summary>
        /// <param name="amostra">Booleano: indica se deve ser carregada uma amostragem de dados.</param>
        /// <param name="pk">String: Caso seja necessário carregar valores de um registro, sua chave primária deve ser informada.</param>
        /// <returns></returns>
        Task CarregarValoresAsync(bool amostra = false, string pk = "");

        /// <summary>
        /// Carrega valores de registros da tabela.
        /// </summary>
        /// <param name="query">String: Query a ser executada para retornar os valores.</param>
        /// <returns></returns>
        Task CarregarValoresAsync(string query);

        /// <summary>
        /// Monta a instrução de Select da tabela.
        /// </summary>
        /// <param name="where">String: Condição where.</param>
        /// <param name="amostra">Booleano: Amostragem de dados (top 10) ou não.</param>
        /// <param name="limitarUm">Booleano: Limitar a um registro. Útil em casos onde deve ser retornado apenas um registro para não haver erros.</param>
        /// <returns>String: Instrução select montada e pronta para ser executada.</returns>
        Task<string> RetornarSelectAsync(string where, bool amostra, bool limitarUm);

        /// <summary>
        /// Monta a instrução de Update de determinado registro da tabela.
        /// </summary>
        /// <param name="pk">String: Chave primária do registro.</param>
        /// <param name="valores">Dictionary: Novos valores do registro.</param>
        /// <returns>String: Comando update montado e pronto para ser executado.</returns>
        string RetornarUpdateAsync(string pk, Dictionary<string, object> valores);

        /// <summary>
        /// Retorna a instrução de Insert de um novo registro na tabela.
        /// </summary>
        /// <param name="valores">Dictionary: Valores do novo registro.</param>
        /// <returns>String: Comando insert montado e pronto para ser executado.</returns>
        Task<string> RetornarInsertAsync(Dictionary<string, object> valores);

        /// <summary>
        /// Retorna a chave primária de um registro, salvo anteriormente.
        /// </summary>
        /// <returns>String: Chave primária salva.</returns>
        string GetPk();

        /// <summary>
        /// Altera determinado registro da tabela.
        /// </summary>
        /// <param name="valores">Dictionary: Lista de valores do registro a ser alterado.</param>
        /// <returns>String: Resultado da operação.</returns>
        Task<string> AlterarRegistroAsync(Dictionary<string, string> valores);

        /// <summary>
        /// Insere um novo registro na tabela.
        /// </summary>
        /// <param name="valores">Dictionary: Lista de valores do registro a ser inserido.</param>
        /// <returns>String: Resultado da operação.</returns>
        Task<string> InserirRegistroAsync(Dictionary<string, string> valores);

        /// <summary>
        /// Pesquisa determinado(s) registro(s) na tabela.
        /// </summary>
        /// <param name="valores">Dictionary: Lista de valores para filtragem.</param>
        /// <returns>String: Resultado da operação.</returns>
        Task<string> PesquisarRegistrosAsync(Dictionary<string, string> valores);

        /// <summary>
        /// Salva as configurações das colunas da tabela.
        /// </summary>
        /// <param name="colunasVisiveis">List: Colunas visíveis.</param>
        /// <param name="colunasChave">List: Colunas usadas para exibição de chaves estrangeiras.</param>
        /// <param name="colunasFiltro">List: Colunas usadas para filtragem.</param>
        /// <returns></returns>
        Task SalvarConfiguracoesColunasAsync(List<string> colunasVisiveis, List<string> colunasChave, List<string> colunasFiltro);

        /// <summary>
        /// Retorna as colunas visíveis da tabela.
        /// </summary>
        /// <returns>List: Colunas visíveis da tabela.</returns>
        Task<List<string>> RetornarColunasVisiveisAsync();

        /// <summary>
        /// Retorna as colunas de filtro da tabela.
        /// </summary>
        /// <returns>List: Colunas de filtro da tabela.</returns>
        Task<List<string>> RetornarColunasFiltroAsync();

        /// <summary>
        /// Busca uma configuração já salva para a tabela.
        /// </summary>
        /// <returns>Int: 0 caso não tenha configuração salva. Caso tenha, retorna o Id.</returns>
        Task<int> RetornarIdConfiguracaoTabelaAsync();

        /// <summary>
        /// Carrega as colunas de filtro da tabela.
        /// </summary>
        /// <returns></returns>
        Task CarregarColunasFiltro();

        /// <summary>
        /// Monta a instrução Where para determinado valor.
        /// </summary>
        /// <param name="valores">Dictionary: Valores do registro a ser considerado para montar o Where.</param>
        /// <returns>String: Instrução Where do registro.</returns>
        string MontarWhere(Dictionary<string, object> valores);

        /// <summary>
        /// Tenta deletar um registro da tabela.
        /// </summary>
        /// <param name="pk">Chave primária do registro.</param>
        /// <returns></returns>
        Task<string[]> DeletarRegistroAsync(int idServidor, string pk);
        #endregion
    }
}
