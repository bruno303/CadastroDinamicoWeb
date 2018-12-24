using System.ComponentModel.DataAnnotations;

namespace CadastroDinamico.Web.Models
{
    public class ConfiguracaoBancoDadosViewModel
    {
        [Required(ErrorMessage = "É necessário informar o servidor")]
        [Display(Name = "Servidor")]
        public string Servidor { get; set; }

        [Display(Name = "Instância")]
        public string Instancia { get; set; }

        [Required(ErrorMessage = "É necessário informar o usuário")]
        [Display(Name = "Usuário")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "É necessário informar a senha")]
        [Display(Name = "Senha")]
        public string Senha { get; set; }

        [Display(Name = "Gravar Log")]
        public bool RegistrarLog { get; set; }
    }
}
