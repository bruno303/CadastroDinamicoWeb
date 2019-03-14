using System.Collections.Generic;

namespace CadastroDinamico.Dominio
{
    public class MatrizValores
    {
        private List<RegistroTabela> objeto = null;

        public int QtdLinhas { get; set; }
        public int QtdColunas { get; set; }

        public MatrizValores(int linhas, int colunas)
        {
            objeto = new List<RegistroTabela>();
            QtdLinhas = linhas;
            QtdColunas = colunas;
        }

        public string GetValor(int linha, int coluna)
        {
            if (objeto != null)
            {
                return objeto[linha].GetValor(coluna);
            }
            else
            {
                return "NULL";
            }
        }

        public void SetValor(int linha, int coluna, object valor, string pk)
        {
            if (objeto.Count <= linha)
            {
                objeto.Add(new RegistroTabela(pk));
            }
            objeto[linha].AdicionarColuna(valor);
        }

        public string GetPk(int linha)
        {
            return objeto[linha].GetPk();
        }
    }
}
