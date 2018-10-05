using CadastroDinamico.Dominio;
using System.Collections.Generic;
using System.Data;

namespace CadastroDinamico.Core
{
    public interface ITabelaCore
    {
        #region Propriedades
        string Nome { get; set; }
        string Database { get; set; }
        string Schema { get; set; }
        bool TemChavePrimaria { get; set; }
        bool TemChaveEstrangeira { get; set; }
        int QuantidadeCampos { get; set; }
        List<Coluna> Colunas { get; set; }
        #endregion

        #region Métodos
        string Carregar();
        DataTable RetornarAmostraDados(List<string> camposNaoExibir);
        string RetornarSelect(bool amostra = false);
        #endregion
    }
}
