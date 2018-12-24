namespace CadastroDinamico.Dominio
{
    public class BancoDados
    {
        public string Servidor { get; set; }
        public string Instancia { get; set; }
        public string Database { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public bool RegistrarLog { get; set; }

        public override string ToString()
        {
            var retorno = string.Format("Server={0}", Servidor);
            if (!string.IsNullOrWhiteSpace(Instancia))
            {
                retorno += string.Format("\\{0}", Instancia);
            }

            retorno += string.Format(";Database={0};User Id={1};Password={2};", string.IsNullOrWhiteSpace(Database) ? "master" : Database, Usuario, Senha);

            return retorno;
        }
    }
}
