namespace CadastroDinamico.Dominio
{
    public class Servidor
    {
        public int IdServidor { get; set; }
        public string Hostname { get; set; }
        public string Instancia { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public bool GravarLog { get; set; }
        public bool UsarTransacao { get; set; }
    }
}
