using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Whatsapp.Data;

namespace Whatsapp.Components
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly MainContext _context;
        public SidebarViewComponent(MainContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.Usuario = _context.Users.First(p => p.Email == User.Identity.Name);
            return View();
        }
    }
}
