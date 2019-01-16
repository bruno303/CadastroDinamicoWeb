namespace CadastroDinamico.Web.Models
{
    public class CustomErrorViewModel
    {
        public int IdErro { get; set; }
        public bool ShowIdErro => IdErro > 0;
        public string Mensagem { get; set; }
    }
}
