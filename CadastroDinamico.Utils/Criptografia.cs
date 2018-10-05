using System;

namespace CadastroDinamico.Utils
{
    public class Criptografia
    {
        public string Criptografar(string text)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(text));
        }

        public string Descriptografar(string text)
        {
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(text));
        }

    }
}
