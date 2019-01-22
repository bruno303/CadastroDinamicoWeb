using System.Collections.Generic;

namespace CadastroDinamico.Dominio
{
    public class RegistroTabela
    {
        private List<object> colunas = new List<object>();
        private string pk = "";

        public RegistroTabela(string pk)
        {
            this.pk = pk;
        }

        public void AdicionarColuna(object valor)
        {
            colunas.Add(valor);
        }

        public string GetValor(int index)
        {
            return colunas[index].ToString();
        }

        public string GetPk()
        {
            return pk;
        }
    }
}
