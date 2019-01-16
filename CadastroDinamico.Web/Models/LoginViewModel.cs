using CadastroDinamico.Dominio;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CadastroDinamico.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Campo usuário é obrigatório")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Campo senha é obrigatório")]
        public string Senha { get; set; }

        public int? Servidor { get; set; }

        public List<Servidor> Servidores { get; set; }

        public LoginViewModel()
        {
            CarregarServidoresAsync();
        }

        public async void CarregarServidoresAsync()
        {
            Servidores = await new Repositorio.SQLite.Repositorio().RetornarServidoresAsync();
        }
    }
}
