using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace CadastroDinamico.Web.Filters
{
    public class CadDinamicoAuthAttribute : Attribute, IAsyncResourceFilter
    {
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var idUsuario = context.HttpContext.Session.GetInt32("idUsuario");
            if ((!idUsuario.HasValue) || idUsuario.Value == 0)
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }
            else
            {
                var resultContext = await next();
            }
        }
    }
}
