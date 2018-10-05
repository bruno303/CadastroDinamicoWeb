namespace CadastroDinamico.Dominio
{
    public class BancoDados
    {
        public string Servidor { get; set; }
        public string Database { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public bool RegistrarLog { get; set; }

        public override string ToString()
        {
            return string.Format("Server={0};Database={1};User Id={2};Password={3};", Servidor, Database, Usuario, Senha);
        }
    }
}
