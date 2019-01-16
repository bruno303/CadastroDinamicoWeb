using System.ComponentModel.DataAnnotations;

namespace CadastroDinamico.Web.Models
{
    public class ServidorViewModel
    {
        public int? IdServidor { get; set; }

        [Required(ErrorMessage = "Campo hostname obrigatório")]
        public string Hostname { get; set; }

        [Display(Name = "Instância")]
        public string Instancia { get; set; }

        [Required(ErrorMessage = "Campo usuário obrigatório")]
        [Display(Name = "Usuário")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Campo Senha obrigatório")]
        public string Senha { get; set; }
    }
}
