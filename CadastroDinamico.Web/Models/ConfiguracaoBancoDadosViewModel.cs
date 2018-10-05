using System.ComponentModel.DataAnnotations;

namespace CadastroDinamico.Web.Models
{
    public class ConfiguracaoBancoDadosViewModel
    {
        [Required(ErrorMessage = "É necessário informar o servidor")]
        [Display(Name = "Servidor")]
        public string Servidor { get; set; }

        [Required(ErrorMessage = "É necessário informar o banco de dados")]
        [Display(Name = "Banco de Dados")]
        public string Database { get; set; }

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
