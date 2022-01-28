using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Whatsapp.Data.MainModels;

namespace Whatsapp.Models
{
    public class DashboardViewModels
    {
        public class EnviarMensagemViewModel
        {
            public int Ticket { get; set; }
            public string Mensagem { get; set; }
        }
        public class MinhaContaViewModel
        {
            public string Nome { get; set; }
            public string Email { get; set; }
            public string Senha { get; set; }
        }
        public class SessaoViewModel : Sessao
        { 
            public IFormFile Arquivo { get; set; }
            public bool SemImagem { get; set; }
        }
        public class AtendenteViewModel
        {
            public string Nome { get; set; }
            public string Email { get; set; }
            public string Senha { get; set; }
        }
    }
}
