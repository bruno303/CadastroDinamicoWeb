using System.ComponentModel.DataAnnotations;

namespace CadastroDinamico.Web.Models
{
    public class UsuarioViewModel
    {
        public int? IdUsuario { get; set; }

        [Required(ErrorMessage = "Campo nome é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo login é obrigatório")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Campo senha é obrigatório")]
        public string Senha { get; set; }
    }
}
