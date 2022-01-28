using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Threading.Tasks;
using Whatsapp.Data;
using static Whatsapp.Data.MainModels;

namespace Whatsapp.API.MKAUTH
{
    public class API
    {
        public class Auth
        {
            public static string AtualizarToken(ConfigMKAUTH api)
            {
                var url = "https://"+api.IP+"/api/";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["client_id"] = api.ClientID;
                httpRequest.ServerCertificateValidationCallback = (a, b, c, d) => { return true; };

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    using(var _context = new MainContext())
                    {
                        _context.ConfigsMKAUTH.First(p => p.ID == api.ID).Token = result;
                        _context.SaveChanges();
                    }

                    return result;
                }
            }
        }
        public class Caixa {        
            public static Models.Caixa.RetornoListagem Listagem(ConfigMKAUTH api) {

                INIT:
                var url = "http://" + api.IP + "/api/caixa/listagem";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if(result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Caixa.RetornoListagem>(result);
                }

            }
            public static Models.Caixa.RetornoShow Buscar(ConfigMKAUTH api, string id) 
            {
                INIT:
                var url = "http://" + api.IP + "/api/caixa/show/"+id;

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Caixa.RetornoShow>(result);
                }
            }
            public static void Inserir() { }
            public static void Remover() { }
        }
        public class Chamado
        {
            public static Models.Chamado.RetornoListagem Listagem(ConfigMKAUTH api)
            {
                INIT:
                var url = "http://" + api.IP + "/api/chamado/listagem";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Chamado.RetornoListagem>(result);
                }
            }
            public static Models.Chamado.RetornoShow Buscar(ConfigMKAUTH api, string id)
            {
                INIT:
                var url = "http://" + api.IP + "/api/chamado/show/" + id;

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Chamado.RetornoShow>(result);
                }
            }
            public static void Fechar() { }
            public static void Remover() { }
        }
        public class Cliente
        {
            public static Models.Cliente.RetornoListagem Listagem(ConfigMKAUTH api)
            {
                INIT:
                var url = "http://" + api.IP + "/api/cliente/listagem";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Cliente.RetornoListagem>(result);
                }
            }
            public static Models.Cliente.RetornoShow Buscar(ConfigMKAUTH api, string id) 
            {
                INIT:
                var url = "http://" + api.IP + "/api/cliente/show/" + id;

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Cliente.RetornoShow>(result);
                }
            }
            public static void Remover() { }
        }
        public class Conta
        {
            public static Models.Conta.RetornoListagem Listagem(ConfigMKAUTH api)
            {
                INIT:
                var url = "http://" + api.IP + "/api/conta/listagem";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Conta.RetornoListagem>(result);
                }
            }
            public static Models.Conta.RetornoShow Buscar(ConfigMKAUTH api, string id) 
            {
                INIT:
                var url = "http://" + api.IP + "/api/conta/show/" + id;

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Conta.RetornoShow>(result);
                }
            }
            public static void Remover() { }
        }
        public class Contato
        {
            public static Models.Contato.RetornoListagem Listagem(ConfigMKAUTH api)
            {
                INIT:
                var url = "http://" + api.IP + "/api/contato/listagem";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Contato.RetornoListagem>(result);
                }
            }
            public static Models.Contato.RetornoShow Buscar(ConfigMKAUTH api, string id) 
            {
                INIT:
                var url = "http://" + api.IP + "/api/contato/show/" + id;

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Contato.RetornoShow>(result);
                }
            }
            public static void Remover() { }
            public static void Editar() { }
            public static void Inserir() { }
        }
        public class Instalacao
        {
            public static Models.Instalacao.RetornoListagem Listagem(ConfigMKAUTH api)
            {
                INIT:
                var url = "http://" + api.IP + "/api/instalacao/listagem";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Instalacao.RetornoListagem>(result);
                }
            }
            public static Models.Instalacao.RetornoShow Buscar(ConfigMKAUTH api, string id) 
            {
                INIT:
                var url = "http://" + api.IP + "/api/instalacao/show/" + id;

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Instalacao.RetornoShow>(result);
                }
            }
            public static void Remover() { }
        }
        public class Plano
        {
            public static Models.Plano.RetornoListagem Listagem(ConfigMKAUTH api)
            {
                INIT:
                var url = "http://" + api.IP + "/api/plano/listagem";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Plano.RetornoListagem>(result);
                }
            }
            public static Models.Plano.RetornoShow Buscar(ConfigMKAUTH api, string id) 
            {
                INIT:
                var url = "http://" + api.IP + "/api/plano/show/" + id;

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Plano.RetornoShow>(result);
                }
            }
            public static void Remover() { }
        }
        public class Titulo
        {
            public static Models.Titulo.RetornoListagem Listagem(ConfigMKAUTH api)
            {
                INIT:
                var url = "http://" + api.IP + "/api/titulo/listagem";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Titulo.RetornoListagem>(result);
                }
            }
            public static Models.Titulo.RetornoShow Buscar(ConfigMKAUTH api, string id) 
            {
                INIT:
                var url = "http://" + api.IP + "/api/titulo/show/" + id;

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Titulo.RetornoShow>(result);
                }
            }
            public static void Remover() { }
            public static void Receber() { }
        }
        public class Usuario
        {
            public static Models.Usuario.RetornoListagem Listagem(ConfigMKAUTH api)
            {
                INIT:
                var url = "http://" + api.IP + "/api/usuario/listagem";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Usuario.RetornoListagem>(result);
                }
            }
            public static Models.Usuario.RetornoShow Buscar(ConfigMKAUTH api, string id) 
            {
                INIT:
                var url = "http://" + api.IP + "/api/usuario/show/" + id;

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + api.Token;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Contains("JWT invalido")) { api.Token = Auth.AtualizarToken(api); goto INIT; }
                    return JsonConvert.DeserializeObject<Models.Usuario.RetornoShow>(result);
                }
            }
            public static void Remover() { }
        }
    }
}
