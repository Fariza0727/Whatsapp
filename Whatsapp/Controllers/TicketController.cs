using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Whatsapp.Data;
using static Whatsapp.API.Models;
using static Whatsapp.Data.MainModels;
using static Whatsapp.Models.DashboardModels;

namespace Whatsapp.Controllers
{
    public class TicketController : Controller
    {
        private readonly MainContext _context;
        private readonly UserManager<Usuario> _userManager;
        public TicketController(MainContext context, UserManager<Usuario> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult IniciarTicket(string ID)
        {
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            //Localiza o chat
            var i = API.Whatsapp.BuscarInstancia(gerente);
            var chat = i.Chats.First(p => p.ID == ID);

            //Verifica se o cliente já está no banco
            if(_context.Clientes.Where(p => p.Telefone == chat.NumeroContato && p.Usuario == User.Identity.Name).Count() == 0)
            {
                //Se não tiver adiciona
                var nCliente = new Cliente
                {
                    Nome = chat.NomeContato,
                    Registro = DateTime.Now,
                    Telefone = chat.NumeroContato,
                    UltimoTicket = DateTime.Now,
                    Usuario = User.Identity.Name,
                };
                _context.Clientes.Add(nCliente);
                _context.SaveChanges();
            }
            var cliente = _context.Clientes.First(p => p.Telefone == chat.NumeroContato && p.Usuario == User.Identity.Name);

            //Verifica se já existe um ticket aberto para esse cliente
            
            if (!API.Whatsapp.VerificarTicketExistente(chat.ID, i)) 
            {
                //Caso não exista cria um novo
                var t = API.Whatsapp.CriarTicket(chat, i, User.Identity.Name);
                //AdicionarTicket(i, cliente, "Em Atendimento", atendente, gerente, chat);

                return RedirectToAction("Whatsapp", "Dashboard", new { ID = t.ID });
            }
            else
            {
                return RedirectToAction("Whatsapp","Dashboard", new { Erro = "Já existe um ticket aberto para esse cliente no momento" });
            }

        }

        [HttpPost]
        public IActionResult EnviarMensagem()
        {
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);            
            string reply = "";
            var x = Request.Form;

            //Localiza o chat
            var i = API.Whatsapp.BuscarInstancia(gerente);
            var chat = i.Chats.First(p => p.ID == Request.Form["chat"]);
            var t = i.Tickets.First(p => p.Chat == chat.ID && p.Status != "Encerrado");

            //Verifica se é resposta à alguma mensagem
            if(Request.Form["reply"] != "")
            {
                reply = Request.Form["reply"];
            }

            //Verifica se tem voice
            if(Request.Form["voice"] != "")
            {
                var voiceComplete = Request.Form["voice"].ToString();
                var voice = voiceComplete.Split('|')[0];
                var duration = voiceComplete.Split('|')[1];
                string b64 = "data:audio/ogg;base64,"+ voice;
                API.Whatsapp.EnviarArquivo(Request.Form["voice"], b64, "audio.ogg", chat, i, true, duration, reply);
            }

            //Verifica se tem arquivo
            if(Request.Form.Files.Count() > 0)
            {
                var file = Request.Form.Files[0];
                string b64 = "data:"+file.ContentType+";base64,";
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    b64 += Convert.ToBase64String(fileBytes);
                }
                API.Whatsapp.EnviarArquivo(Request.Form["mensagem"], b64, file.FileName, chat, i, false, "", reply);
            }
            else
            {
                API.Whatsapp.EnviarMensagem(null, Request.Form["mensagem"], chat, t, i, User.Identity.Name, reply);
            }
            return Ok();
        }


        public IActionResult AlterarTicket(string ID, string Status)
        {
            //Localiza o ticket
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);
            var i = API.Whatsapp.BuscarInstancia(gerente);

            var ticket = i.Tickets.First(p => p.ID == Convert.ToInt32(ID));

            i.Tickets.First(p => p.ID == Convert.ToInt32(ID)).Status = Status;
            _context.Tickets.First(p => p.ID == Convert.ToInt32(ID)).Status = Status;
            _context.SaveChanges();

            return RedirectToAction("Whatsapp","Dashboard", new { ID = ticket.ID });
        }
        public IActionResult EncerrarTicket(string ID)
        {
            //Localiza o ticket
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);
            var i = API.Whatsapp.BuscarInstancia(gerente);
            var chat = i.Chats.First(p => p.ID == ID);
            var ticket = i.Tickets.First(p => p.Chat == ID && p.Status != "Encerrado");

            API.Whatsapp.EncerrarTicket(ticket, i, chat);

            return RedirectToAction("Whatsapp", "Dashboard");
        }
    }
}
