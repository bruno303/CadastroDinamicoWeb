namespace CadastroDinamico.Web.Models
{
    public class CustomErrorViewModel
    {
        public int IdErro { get; set; }
        public bool ShowIdErro => IdErro > 0;
        public string Mensagem { get; set; }
        public bool ShowMessage => !string.IsNullOrWhiteSpace(Mensagem);
        public string Titulo { get; set; }
        public bool ShowTitulo => !string.IsNullOrWhiteSpace(Titulo);
        public string ComandoExecutado { get; set; }
        public bool ShowComandoExecutado => !string.IsNullOrWhiteSpace(ComandoExecutado);
    }
}
