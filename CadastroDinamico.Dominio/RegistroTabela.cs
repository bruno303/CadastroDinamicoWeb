using System.Collections.Generic;

namespace CadastroDinamico.Dominio
{
    public class RegistroTabela
    {
        private List<object> colunas = new List<object>();

        public void AdicionarColuna(object valor)
        {
            colunas.Add(valor);
        }

        public string GetValor(int index)
        {
            return colunas[index].ToString();
        }
    }
}
