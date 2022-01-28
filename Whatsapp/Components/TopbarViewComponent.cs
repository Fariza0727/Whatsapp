using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Whatsapp.Data;

namespace Whatsapp.Components
{
    public class TopbarViewComponent : ViewComponent
    {
        private readonly MainContext _context;
        public TopbarViewComponent(MainContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //Busca o atendente e o gerente
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            //Verifica se existe um whatsapp 
            if(_context.Whatsapps.Where(p => p.Usuario == gerente.Email).Count() != 0)
            {
                ViewBag.Whatsapp = _context.Whatsapps.Where(p => p.Usuario == gerente.Email).First();
            }
            ViewBag.Usuario = _context.Users.First(p => p.Email == User.Identity.Name);
            return View();
        }
    }
}
