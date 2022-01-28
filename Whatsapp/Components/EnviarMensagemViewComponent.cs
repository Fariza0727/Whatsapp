using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Whatsapp.Data;
using static Whatsapp.Models.DashboardViewModels;

namespace Whatsapp.Components
{
    public class EnviarMensagemViewComponent : ViewComponent
    {
        private readonly MainContext _context;
        public EnviarMensagemViewComponent(MainContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string ID)
        {
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);
            var i = API.Whatsapp.BuscarInstancia(gerente);

            var chat = i.Chats.First(p => p.ID == ID);
            ViewBag.Chat = chat;

            //Verifica se está rolando ticket nesse chat e envia o ticket junto
            if(i.Tickets.Where(p => p.Chat == chat.ID && p.Status != "Encerrado").Count() != 0)
            {
                ViewBag.TicketData = i.Tickets.First(p => p.Chat == chat.ID && p.Status != "Encerrado");
            }

            return View();
        }
    }
}
