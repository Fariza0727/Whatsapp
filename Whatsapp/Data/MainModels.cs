using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Whatsapp.Data
{
    public class MainModels
    {
        public class Usuario : IdentityUser
        {
            public string Gerente { get; set; }
            public string Nome { get; set; }
            public string Cargo { get; set; }
            public string Imagem { get; set; }
            public string Assinatura { get; set; }
            public DateTime FimDoAcesso { get; set; }
            public DateTime Registro { get; set; }
            public DateTime UltimoLogin { get; set; }
        }

        public class ConfigMKAUTH
        {
            public int ID { get; set; }
            public string Usuario { get; set; }
            public string IP { get; set; }
            public string ClientID { get; set; }
            public string ClientSecret { get; set; }
            public string Token { get; set; }
        }
        public class Whatsapp
        {            
            public int ID { get; set; }
            public string Usuario { get; set; }
            public string Numero { get; set; }
            public string Dispositivo { get; set; }
            public string Nome { get; set; }
            public string Plataforma { get; set; }
            public string Imagem { get; set; }

        }
        public class Cliente
        {
            public int ID { get; set; }
            public string Usuario { get; set; }
            public string Nome { get; set; }
            public string Observacao { get; set; }
            public string Telefone { get; set; }
            public DateTime UltimoTicket { get; set; }
            public DateTime Registro { get; set; }
        }

        public class Sessao
        {
            public int ID { get; set; }
            public string Usuario { get; set; }
            public string Imagem { get; set; }
            public string Titulo { get; set; }
            public string Texto { get; set; }
        }

        public class Opcao
        {
            public int ID { get; set; }
            public string Usuario { get; set; }
            public int Sessao { get; set; }
            public string Palavra { get; set; }
            public string Tipo { get; set; } // MENSAGEM - SESSAO - API
            public string Mensagem { get; set; }
            public string Api { get; set; }
            public int SessaoRef { get; set; }
        }

        public class Ticket { 
            public int ID { get; set; }
            public int Chatbot { get; set; }
            public int NaoLidas { get; set; }
            public string Gerente { get; set; }
            public bool Humano { get; set; }
            public string Chat { get; set; }
            public string Status { get; set; }
            public DateTime Inicio { get; set; }
            public DateTime? Termino { get; set; }
            public string Atendente { get; set; }
            public string Telefone { get; set; }
            public string Nome { get; set; }
            public string Imagem { get; set; }
            public string Path { get; set; }
            public string Observacao { get; set; }
            public string Protocolo { get; set; }
        }
        public class Chatbot
        {
            public int ID { get; set; }
            public string Usuario { get; set; }
            public string ChaveHumano { get; set; }
            public string ChaveInicio { get; set; }
            public string ChaveAtual { get; set; }
            public int ChaveInicioDestino { get; set; }
            public string ChaveEncerrar { get; set; }
            public string ChaveMenuAnterior { get; set; }
         //   public int MenuAnterior { get; set; }
         //   public int SessaoAtual { get; set; }
        }
    }
}
