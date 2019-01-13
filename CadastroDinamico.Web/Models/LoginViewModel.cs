using System.ComponentModel.DataAnnotations;

namespace CadastroDinamico.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Campo usuário é obrigatório")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Campo senha é obrigatório")]
        public string Senha { get; set; }
    }
}
