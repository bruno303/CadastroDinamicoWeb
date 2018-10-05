using System.Data;

namespace CadastroDinamico.Utils
{
    public static class Extensions
    {
        public static bool TemRegistros(this DataTable dataTable)
        {
            return (dataTable?.Rows?.Count ?? 0) > 0;
        }

        public static bool Igual(this string texto, string textoComparacao)
        {
            return texto.ToUpper().Equals(textoComparacao.ToUpper());
        }
    }
}
