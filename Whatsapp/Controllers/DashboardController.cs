using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Whatsapp.Data;
using static Whatsapp.API.Models;
using static Whatsapp.Data.MainModels;
using static Whatsapp.Models.DashboardModels;
using static Whatsapp.Models.DashboardViewModels;

namespace Whatsapp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly MainContext _context;
        private readonly UserManager<Usuario> _userManager;
        public DashboardController(MainContext context, UserManager<Usuario> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        //public IActionResult DownloadMedia(string msg, string chat)
        //{
        //    return Ok(API.Whatsapp.DownloadMsg(msg, chat, User.Identity.Name));
        //}

        public IActionResult Teste()
        {
            var config = _context.ConfigsMKAUTH.First(p => p.Usuario == User.Identity.Name);
            var clientes = API.MKAUTH.API.Cliente.Listagem(config);
            return Ok();
        }
      
        public IActionResult Index()
        {        
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            ViewBag.Usuario = atendente;

            ViewBag.TicketsTotal = _context.Tickets.Where(p => p.Gerente == gerente.Email && p.Inicio.DayOfYear == DateTime.Now.DayOfYear).Count();
            ViewBag.TicketsOn = _context.Tickets.Where(p => p.Gerente == gerente.Email && p.Inicio == p.Termino && p.Inicio.DayOfYear == DateTime.Now.DayOfYear && p.Status == "Humano").Count();
            ViewBag.TicketsBot = _context.Tickets.Where(p => p.Gerente == gerente.Email && p.Inicio == p.Termino && p.Inicio.DayOfYear == DateTime.Now.DayOfYear && p.Status == "BOT").Count();
            ViewBag.AtendentesOn = 1;

            //  '7/12', '8/12', '9/12', '10/12', '11/12', '12/12', '13/12', '14/12', '15/12', '16/12', '17/12' datas
            //     275, 90, 190, 205, 125, 85, 55, 87, 127, 150, 230, 280, 190 valores
            var dias = 29;
            var dataInicio = DateTime.Now.AddDays(-dias);
            var ultimosTickets = _context.Tickets.Where(p => p.Gerente == gerente.Email && p.Inicio <= dataInicio).ToList();
            var listDatas = new List<string>();
            var listValores = new List<int>();
            for (var i = 0; i <= dias; i++)
            {
                listDatas.Add(dataInicio.AddDays(i).Day + "/" + dataInicio.AddDays(i).Month);
            }
            foreach(var item in listDatas)
            {
                listValores.Add(ultimosTickets.Where(p => p.Inicio.Day == Convert.ToInt32(item.Split('/')[0])).Count());
            }

            Dictionary<int, int> list = new Dictionary<int, int>();
            var tickets = _context.Tickets.Where(p => p.Gerente == gerente.Email);
            foreach (var item in tickets)
            {
                if(item.Inicio != null && item.Termino != null)
                {
                    var x = item.Termino - item.Inicio;
                    list.Add(item.ID, Convert.ToInt32(x.Value.TotalSeconds));
                }
            }
            var y = list.OrderByDescending(p => p.Value).First();

            var nData = new DateTime().AddSeconds(y.Value);
            ViewBag.TicketLongo = nData.ToString("HH:mm:ss");

            ViewBag.Datas = listDatas;
            ViewBag.Valores = listValores;


            return View();
        }
        
        public IActionResult Atendentes()
        {
            var usuario = _context.Users.First(p => p.Email == User.Identity.Name);
            ViewBag.Usuario = usuario;

            if (usuario.Cargo == "Gerente")
            {
                ViewBag.Atendentes = _context.Users.Where(p => p.Gerente == User.Identity.Name && p.Cargo == "Atendente").ToList();
            }
            else if (usuario.Cargo == "Administrador")
            {
                ViewBag.Atendentes = _context.Users.Where(p => p.Cargo == "Atendente").ToList();
            }
            return View();
        }
        public IActionResult Tickets()
        {
            var usuario = _context.Users.First(p => p.Email == User.Identity.Name);
            ViewBag.Usuario = usuario;

            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            ViewBag.Tickets = _context.Tickets.Where(p => p.Gerente == gerente.Email).ToList();
            ViewBag.Atendentes = _context.Users.Where(p => p.Gerente == gerente.Email).ToList();
            ViewBag.Clientes = _context.Clientes.Where(p => p.Usuario == gerente.Email).ToList();

            return View();
        }
        public IActionResult VisualizarTicket(string ID)
        {
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            var t = _context.Tickets.First(p => p.ID == Convert.ToInt32(ID));
            var bruto = System.IO.File.ReadAllText(t.Path + @"\data.json");
            var data = JsonConvert.DeserializeObject<TicketBruto>(bruto);
            ViewBag.Data = data;
            return View();
        }

        public IActionResult AdicionarSessao()
        {
            return View();
        }

        public IActionResult Reiniciar()
        {
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            var i = API.Whatsapp.Instancias.First(p => p.Usuario == gerente.Email);
            API.Whatsapp.PararInstancia(i);
            API.Whatsapp.IniciarInstancia(gerente);

            return RedirectToAction("Configuracoes");
        }
        public IActionResult EditarOpcoes(string ID)
        {
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            ViewBag.Sessao = ID;
            ViewBag.Data = _context.Opcoes.Where(p => p.Sessao == Convert.ToInt32(ID)).ToList();
            ViewBag.Sessoes = _context.Sessoes.Where(p => p.Usuario == gerente.Email).ToList();

            return View();
        }

        [Consumes("application/x-www-form-urlencoded")]
        [HttpPost]
        public async Task<IActionResult> SalvarOpcoes([FromForm] string model)
        {
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);



            var obj = JsonConvert.DeserializeObject<List<OpcaoModel>>(Request.Form.Keys.ToList().First());

            //Entende quantas opções nos tempos atraves das keys
            var opCount = obj.Where(p => p.name.Contains("[Key]")).Count();
            var sessao = Convert.ToInt32(obj.First().value);

            //Remove todas as opções da sessao
            _context.Opcoes.RemoveRange(_context.Opcoes.Where(p => p.Usuario == gerente.Email && p.Sessao == sessao).ToList());
            _context.SaveChanges();

            //Cria as opções com base na contagem
            for (var i = 0; i < opCount; i++)
            {
                var palavra = "";
                var tipo = "";
                var api = "";
                int? sessaoref = null;
                var mensagem = "";

                try { palavra = obj.First(p => p.name == "invoice[" + i + "][Key]").value; } catch { continue; }
                try { tipo = obj.First(p => p.name == "invoice[" + i + "][Tipo]").value; } catch { continue; }

                try { api = obj.First(p => p.name == "invoice[" + i + "][Api]").value; } catch { if (tipo == "Chamada para API") { continue; } }
                try { sessaoref = Convert.ToInt32(obj.First(p => p.name == "invoice[" + i + "][Sessao]").value); } catch { if (tipo == "Abrir outra sessao") { continue; } }
                try { mensagem = obj.First(p => p.name == "invoice[" + i + "][Mensagem]").value; } catch { if (tipo == "Enviar uma mensagem") { continue; } }

                var nOpcao = new Opcao
                {
                    Sessao = sessao,
                    Palavra = palavra,
                    Tipo = tipo,
                    Api = api,
                    Mensagem = mensagem,
                    Usuario = gerente.Email,
                };
                if(sessaoref != null) { nOpcao.SessaoRef = Convert.ToInt32(sessaoref); }
                _context.Opcoes.Add(nOpcao);
            }
            //Insere as novas opções da sessao
            _context.SaveChanges();


            var ins = API.Whatsapp.Instancias.First(p => p.Usuario == gerente.Email);
            API.Whatsapp.ReiniciarInstancia(ins, gerente);

            return RedirectToAction("EditarOpcoes","Dashboard", new { ID = sessao });
        }


        [HttpPost]
        public IActionResult EditarSessao(SessaoViewModel model)
        {
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            var sessao = _context.Sessoes.First(p => p.Usuario == gerente.Email && p.ID == model.ID);

            sessao.Titulo = model.Titulo;
            sessao.Texto = model.Texto;

            if (model.Arquivo != null)
            {
                using (var ms = new MemoryStream())
                {
                    model.Arquivo.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    sessao.Imagem = Convert.ToBase64String(fileBytes);
                }
            }

            if (model.SemImagem)
            {
                sessao.Imagem = null;
            }

            _context.Update(sessao);
            _context.SaveChanges();

            var ins = API.Whatsapp.Instancias.First(p => p.Usuario == gerente.Email);
            API.Whatsapp.ReiniciarInstancia(ins, gerente);

            return RedirectToAction("Sessoes");
        }

        public IActionResult RemoverSessao(string ID)
        {
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            _context.Sessoes.Remove(_context.Sessoes.First(p => p.ID == Convert.ToInt32(ID) && p.Usuario == gerente.Email));
            _context.SaveChanges();

            var ins = API.Whatsapp.Instancias.First(p => p.Usuario == gerente.Email);
            API.Whatsapp.ReiniciarInstancia(ins, gerente);

            return RedirectToAction("Sessoes");
        }
        public IActionResult EditarSessao(string ID)
        {
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            var model = _context.Sessoes.First(p => p.Usuario == gerente.Email && p.ID == Convert.ToInt32(ID));
            var nModel = new SessaoViewModel
            {
                ID = model.ID,
                Imagem = model.Imagem,
                Texto = model.Texto,
                Titulo = model.Titulo,
                Usuario = model.Usuario,
            };

            return View(nModel);
        }

        [HttpPost]
        public IActionResult AdicionarSessao(SessaoViewModel model)
        {
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            var n = new Sessao
            {
                Texto = model.Texto,
                Titulo = model.Titulo,
                Usuario = gerente.Email,
            };

            if(model.Arquivo != null)
            {
                using (var ms = new MemoryStream())
                {
                    model.Arquivo.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    n.Imagem = Convert.ToBase64String(fileBytes);
                }
            }

            _context.Sessoes.Add(n);
            _context.SaveChanges();
            return RedirectToAction("Sessoes");
        }
        public IActionResult Sessoes()
        {
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            ViewBag.Data = _context.Sessoes.Where(p => p.Usuario == gerente.Email).ToList();
            ViewBag.Opcoes = _context.Opcoes.Where(p => p.Usuario == gerente.Email).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NovoAtendente(AtendenteViewModel model)
        {
            var usuario = _context.Users.First(p => p.Email == User.Identity.Name);
            ViewBag.Usuario = usuario;

            if(usuario.Cargo == "Atendente") { return RedirectToAction("Atendentes"); }

            if (_context.Users.Where(p => p.Email == model.Email).Count() == 0)
            {
                var user = new Usuario { Cargo = "Atendente", Gerente = User.Identity.Name, Assinatura = usuario.Assinatura, Imagem = "/app-assets/images/portrait/small/avatar-s-11.jpg", UserName = model.Email, Email = model.Email, Nome = model.Nome, FimDoAcesso = usuario.FimDoAcesso, Registro = DateTime.Now };
                var result = await _userManager.CreateAsync(user, model.Senha);
                if (result.Succeeded)
                {              
                    return RedirectToAction("Atendentes");
                }

            }
            return RedirectToAction("Atendentes");
        }
        public IActionResult NovoAtendente()
        {
            return PartialView("Modals/Atendentes/_NovoAtendente");
        }

        public IActionResult Relatorios()
        {
            return View();
        }
        public IActionResult Configuracoes()
        {
            var u = _context.Users.First(p => p.Email == User.Identity.Name);
            var i = API.Whatsapp.BuscarInstancia(u);
            ViewBag.Instancia = i;
            return View();
        }

        [HttpPost]
        public IActionResult Chatbot(Chatbot model)
        {
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            var chatbot = _context.Chatbots.First(p => p.Usuario == gerente.Email);

            chatbot.ChaveHumano = model.ChaveHumano;
            chatbot.ChaveEncerrar = model.ChaveEncerrar;
            chatbot.ChaveInicioDestino = model.ChaveInicioDestino;
            chatbot.ChaveInicio = model.ChaveInicio;
            chatbot.ChaveMenuAnterior = model.ChaveMenuAnterior;
            chatbot.ChaveAtual = model.ChaveAtual;
            _context.Update(chatbot);
            _context.SaveChanges();

            var ins = API.Whatsapp.Instancias.First(p => p.Usuario == gerente.Email);
            API.Whatsapp.ReiniciarInstancia(ins, gerente);

            return RedirectToAction("Chatbot");
        }
        public IActionResult Chatbot()
        {
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);
            ViewBag.Sessoes = _context.Sessoes.Where(p => p.Usuario == gerente.Email).ToList();
            var chatbot = _context.Chatbots.First(p => p.Usuario == gerente.Email);

            return View(chatbot);
        }
        public IActionResult Whatsapp(string ID)
        {
            var abrirTicket = ViewData["AbrirTicket"];
            //Busca o atendente e o gerente
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            //Verifica se existe um whatsapp 
            if (_context.Whatsapps.Where(p => p.Usuario == gerente.Email).Count() != 0)
            {
                ViewBag.WhatsappInfo = _context.Whatsapps.Where(p => p.Usuario == gerente.Email).First();
            }

            ViewData["AbrirTicket"] = ID;

            return View();
        }
        public async Task<IActionResult> VerificarAlertas()
        {
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            var i = API.Whatsapp.BuscarInstancia(gerente);
                i = await API.Whatsapp.AtualizacaoRotineira(i);

                //Verifica se a instancia esta conectada
                if (i.Conectado) { ViewBag.AlertaConexao = true; } else { ViewBag.AlertaConexao = false; }
                if (i.Bateria <= 30) { ViewBag.Alerta = i.Bateria; }

            if (!i.Comunicacao) { ViewBag.ComunicacaoCortada = true; }

            return View("_Alertas");
        }
        public IActionResult WhatsappExterno(string ID)
        {
            var abrirTicket = ViewData["AbrirTicket"];
            //Busca o atendente e o gerente
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            //Verifica se existe um whatsapp 
            if (_context.Whatsapps.Where(p => p.Usuario == gerente.Email).Count() != 0)
            {
                ViewBag.WhatsappInfo = _context.Whatsapps.Where(p => p.Usuario == gerente.Email).First();
            }

            ViewData["AbrirTicket"] = ID;
            return View();
        }
        public IActionResult Assinatura()
        {
            return View();
        }
        public IActionResult MinhaConta()
        {
            var user = _context.Users.First(p => p.Email == User.Identity.Name);
            var model = new MinhaContaViewModel
            {
                Email = user.Email,
                Nome = user.Nome,
                Senha = ""
            };
            return View(model);
        }

        public IActionResult Clientes()
        {

            //Busca o atendente e o gerente
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            ViewBag.Usuario = atendente;
            ViewBag.Clientes = _context.Clientes.Where(p => p.Usuario == gerente.Email).ToList();
            return View();
        }
        public IActionResult AdicionarCliente(string ID)
        {   
            //Busca o atendente e o gerente
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            //Busca o chat
            var i = API.Whatsapp.BuscarInstancia(gerente);
            var c = i.Chats.First(p => p.ID == ID);

            AdicionarCliente(c, i);
            return RedirectToAction("Clientes");
        }
        public IActionResult RemoverCliente(string ID)
        {
            _context.Clientes.Remove(_context.Clientes.First(p => p.ID == Convert.ToInt32(ID)));
            _context.SaveChanges();
            return RedirectToAction("Clientes");

        }
        public static void AdicionarCliente(Chat c, Instancia i)
        {
            var n = new Cliente
            {
                Nome = c.NomeContato,
                Registro = DateTime.Now,
                Telefone = c.NumeroContato,
                UltimoTicket = new DateTime(),
                Observacao = "",
                Usuario = i.Usuario
            };
            using(var _cont = new MainContext())
            {
                _cont.Clientes.Add(n);
                _cont.SaveChanges();
            }
        }
        public IActionResult Chat(string ID)
        {
            //Busca o atendente e o gerente
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            ViewBag.Clientes = _context.Clientes.Where(p => p.Usuario == gerente.Email).ToList();
            var u = _context.Users.First(p => p.Email == User.Identity.Name);
            var i = API.Whatsapp.BuscarInstancia(u);
            if (i.Conectado)
            {
                ViewBag.Chat = i.Chats.First(p => p.ID == ID);
            }
            else
            {
                ViewBag.Chat = new List<API.Models.Chat>();
            }
            ViewBag.IsTicket = false;
            return View("_Chat");
        }
        public IActionResult Ticket(string ID)
        {
            //Busca o atendente e o gerente
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            ViewBag.Clientes = _context.Clientes.Where(p => p.Usuario == gerente.Email).ToList();
            var u = _context.Users.First(p => p.Email == User.Identity.Name);
            var i = API.Whatsapp.BuscarInstancia(u);
            if (i.Conectado)
            {
                var ticket = i.Tickets.First(p => p.ID == Convert.ToInt32(ID));
                var chat = i.Chats.First(p => p.ID == ticket.Chat);
                ViewBag.Chat = chat;
                i.Browser.ExecuteScriptAsync("WAPI.sendSeen('" + chat.ID + "')");
                ticket.NaoLidas = 0;
            }
            else
            {
                ViewBag.Chat = new List<API.Models.Chat>();
            }

            ViewBag.IsTicket = true;
            return View("_Chat");
        }
        public IActionResult Desconectar()
        {

            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            var i = API.Whatsapp.Instancias.First(p => p.Usuario == gerente.Email);
            API.Whatsapp.DesconectarWhatsapp(i);
            return RedirectToAction("Configuracoes");


        }
        public IActionResult Desligar()
        {
            //Busca o atendente e o gerente
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);
            var i = API.Whatsapp.BuscarInstancia(gerente);
            API.Whatsapp.Instancias.First(p => p.Usuario == gerente.Email).Comunicacao = false;
            API.Whatsapp.PausarInstancia(i);
            return RedirectToAction("Configuracoes");
        }
        public IActionResult Ligar()
        {
            //Busca o atendente e o gerente
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);
            var i = API.Whatsapp.BuscarInstancia(gerente);
            API.Whatsapp.Instancias.First(p => p.Usuario == gerente.Email).Comunicacao = true;
            API.Whatsapp.DespausarInstancia(i);
            return RedirectToAction("Configuracoes");
        }
        public IActionResult ChatMensagens(string ID, string Ticket)
        {
            //Busca o atendente e o gerente
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);
            var i = API.Whatsapp.BuscarInstancia(gerente);

            if (i.Conectado)
            {
                ViewBag.ChatMensagens = i.Chats.First(p => p.ID == ID).Mensagens;

                if (Ticket == "True")
                {
                    //Verifica se tem ticket rolando para enviar so as mensagens do ticket pra exibição
                    if (i.Tickets.Where(p => p.Chat == ID && p.Status != "Encerrado").Count() != 0)
                    {
                        i.Browser.ExecuteScriptAsync("WAPI.sendSeen('" + i.Chats.First(p => p.ID == ID).ID + "')");
                        i.Tickets.First(p => p.Chat == ID && p.Status != "Encerrado").NaoLidas = 0;
                        ViewBag.ChatMensagens = i.Tickets.First(p => p.Chat == ID && p.Status != "Encerrado").Mensagens;
                    }
                }

            }
            else
            {
                ViewBag.ChatMensagens = new List<Mensagem>();
            }
            return View("_ChatMensagens");
        }
        public IActionResult Chats()
        {
            var u = _context.Users.First(p => p.Email == User.Identity.Name);
            var i = API.Whatsapp.BuscarInstancia(u);
            if (i.Conectado)
            {
                ViewBag.Chats = i.Chats;
                if(i.Chatbot != null)
                {
                    ViewBag.Tickets = i.Tickets;
                }
            }
            else
            {
                ViewBag.Chats = new List<API.Models.Chat>();
            }
            return View("_Chats");
        }

        public IActionResult Sincronia()
        {
            var atendente = _context.Users.First(p => p.Email == User.Identity.Name);
            var gerente = _context.Users.First(p => p.Email == atendente.Gerente);

            var i = API.Whatsapp.BuscarInstancia(gerente);
            if (!i.Conectado && !i.Conectando) { var x = new Thread(() =>API.Whatsapp.IniciarInstancia(gerente)); x.Start(); }
            ViewBag.Instancia = i;
            return View();
        }
        public IActionResult QrCode()
        {
            var u = _context.Users.First(p => p.Email == User.Identity.Name);
            var i = API.Whatsapp.BuscarInstancia(u);
            ViewBag.QrCode = i.QrCode;
            ViewBag.Conectado = i.Conectado;
            return View();
        }

        [HttpPost]
        public IActionResult MKAUTH(ConfigMKAUTH model) 
        {
            var x = _context.ConfigsMKAUTH.First(p => p.Usuario == User.Identity.Name);
            x.ClientID = model.ClientID;
            x.ClientSecret = model.ClientSecret;
            x.IP = model.IP;
            _context.Update(x);
            _context.SaveChanges();
            return RedirectToAction("MKAUTH");
        }
        public IActionResult MKAUTH() { 
        
            if(_context.ConfigsMKAUTH.Where(p => p.Usuario == User.Identity.Name).Count() == 0)
            {
                var n = new ConfigMKAUTH
                {
                    Usuario = User.Identity.Name,
                };
                _context.ConfigsMKAUTH.Add(n);
                _context.SaveChanges();
            }

            return View(_context.ConfigsMKAUTH.First(p => p.Usuario == User.Identity.Name));
        }





    }
}
