using AutoMapper;
using CefSharp;
using CefSharp.OffScreen;
using libaxolotl.kdf;
using libaxolotl.util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Whatsapp.API.Auxiliares;
using Whatsapp.Data;
using static Whatsapp.API.Models;
using static Whatsapp.Data.MainModels;

namespace Whatsapp.API
{
    public class Whatsapp
    {
        public static bool INIT = false;
        public static List<Instancia> Instancias = new List<Instancia>();
        public static List<string> FilaInicializacao = new List<string>();

        //Controle de Instancias
        public static void PausarInstancia(Instancia i)
        {

            //Muda o status para parar de atualizar mensagens
            Instancias.First(p => p.Usuario == i.Usuario).Comunicacao = false;

        }
        public static void DespausarInstancia(Instancia i)
        {
            //Muda o status para parar de atualizar mensagens
            Instancias.First(p => p.Usuario == i.Usuario).Comunicacao = true;


        }
        public static void ReiniciarInstancia(Instancia i, Usuario gerente)
        {
            PararInstancia(i);
            IniciarInstancia(gerente);
        }
        public static void InicioGeral()
        {
            //Verifica se o sistema principal de atualização de mensagens está rodando, se não, inicia
            if (!INIT) { Thread x = new Thread(() => Init()); x.Start(); }

            //Inicia o processador de inicializações em fila para impedir duplicata
            Thread y = new Thread(() => ProcessarInicializacao()); y.Start();

            using (var _context = new MainContext())
            {
                //Percorre todos os usuarios que são gerentes
                foreach (var u in _context.Users.Where(p => p.Gerente == p.Email).ToList())
                {
                    //Verifica se temos cache armazenado
                    if (Directory.Exists(Environment.CurrentDirectory + @"\Log\" + u.Email + @"\Browser"))
                    {
                        //Inicia a instancia desse usuario
                        var i = API.Whatsapp.BuscarInstancia(u);
                        if (!i.Conectado && !i.Conectando) { var x = new Thread(() => API.Whatsapp.IniciarInstancia(u)); x.Start(); }

                    }
                }
            }
        }
        public static void ProcessarInicializacao()
        {
            while (true)
            {
                try
                {

                    foreach (var item in FilaInicializacao)
                    {
                        if (Instancias.Where(p => p.Usuario == item).Count() == 0)
                        {

                            var i = new Instancia
                            {
                                Browser = IniciarBrowser(item),
                                Conectado = false,
                                Conectando = false,
                                QrCode = "",
                                Usuario = item,
                                Chats = new List<Chat>(),
                                Tickets = new List<TicketBruto>(),
                                PreReferencias = new List<PreReferencia>(),
                                Comunicacao = true
                            };


                            //Verifica se existe um whatsapp no banco e se não tiver adiciona
                            using (var _context = new MainContext())
                            {
                                if (_context.Whatsapps.Where(p => p.Usuario == item).Count() == 0)
                                {
                                    var n = new MainModels.Whatsapp
                                    {
                                        Dispositivo = "",
                                        Imagem = "",
                                        Nome = "",
                                        Numero = "",
                                        Plataforma = "",
                                        Usuario = item
                                    };
                                    _context.Whatsapps.Add(n);
                                    _context.SaveChanges();
                                }

                                i.Whatsapp = _context.Whatsapps.First(p => p.Usuario == item);

                                //Seta configurações de api
                                i.MKAUTH = _context.ConfigsMKAUTH.First(p => p.Usuario == item);

                                Instancias.Add(i);

                                var chatbot = BuscarChatbot(i);

                                var configuration = new MapperConfiguration(cfg =>
                                {
                                    cfg.CreateMap<Ticket, TicketBruto>();
                                });

                                var mapper = configuration.CreateMapper();

                                //Busca tickets do banco, no caso de um reinicio ele recupera de onde parou
                                //var tickets = _context.Tickets.Where(p => p.Gerente == i.Usuario && p.Chatbot == chatbot.ID).ToList();
                                //Instancias.First(p => p.Usuario == i.Usuario).Tickets = mapper.Map<List<TicketBruto>>(tickets);
                                //foreach(var tt in Instancias.First(p => p.Usuario == i.Usuario).Tickets) //Recupera lista de mensagens
                                //{
                                //    tt.Mensagens = new List<Mensagem>();
                                //}

                                //Remove todos os tickets que ficaram abertos durante um reinicio
                                _context.Tickets.RemoveRange(_context.Tickets.Where(p => p.Gerente == i.Usuario && p.Termino == null));
                                _context.SaveChanges();

                            }
                        }
                    }
                    Thread.Sleep(100);
                }
                catch(Exception e)
                {

                }
            }
        }
        public static void IniciarInstancia(Usuario u)
        {
            var i = BuscarInstancia(u);
            if (!i.Conectado && !i.Conectando)
            {
                //Envia ordem de conexão que vai fornecer o qrcode e aguarda ação do usuario
                ConectarInstania(i);
                while (!i.Conectado) { Thread.Sleep(1000); }
            }
        }
        public static Instancia BuscarInstancia(Usuario u)
        {
            //Verifica se existe uma instancia desse usuario, se tiver retorna ela
            if (Instancias.Where(p => p.Usuario == u.Email).Count() == 0)
            {
                //Se não, adiciona na fila
                FilaInicializacao.Add(u.Email);
            }

            //Aguarda instancia ser inicializada
            while (Instancias.Where(p => p.Usuario == u.Email).Count() == 0)
            {
                Thread.Sleep(100);
            }

            return Instancias.First(p => p.Usuario == u.Email);

        }
        public static async void ConectarInstania(Instancia instancia)
        {
            var i = Instancias.First(p => p.Usuario == instancia.Usuario);
            i.Conectando = true;

            var browser = i.Browser;
            while (browser.IsLoading) { Thread.Sleep(1000); }

            //Verifica conexão, atualiza o status e retorna o qrcode
            while (!await ChecarConexao(browser))
            {
                i.QrCode = await ObterQrCode(browser);
                i.Conectado = false;
                Thread.Sleep(1000);
            }

            //Inicia WAPI
            browser.ExecuteScriptAsync(File.ReadAllText(@"sharp.cef"));

            //Inicia carregamento geral de todas as mensagens
            //browser.ExecuteScriptAsync("WAPI.getAllChats().forEach(function(e){WAPI.loadEarlierMessages(e.id, true, false);})");

            //Atualiza dados da conexão e status da instancia
            AtualizarDadosPessoais(i);
        }
        public static void PararInstancia(Instancia i)
        {
            //Encerra o navegador
            Instancias.First(p => p.Usuario == i.Usuario).Browser.GetBrowser().CloseBrowser(true);

            //Muda o status para parar de atualizar mensagens
            Instancias.First(p => p.Usuario == i.Usuario).Encerrar = true;
            var u = i.Usuario;
            while (Instancias.Where(p => p.Usuario == u).Count() != 0)
            {
                Thread.Sleep(100);
            }
        }
        public static async void DesconectarWhatsapp(Instancia i)
        {
            var path = Environment.CurrentDirectory + @"\Log\" + i.Usuario + @"\Browser";

            //Encerra o navegador
            var code0 = "var xPathRes = document.evaluate('//*[@id=\"side\"]/header/div[2]/div/span/div[3]/div/span', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null); xPathRes.singleNodeValue.click();";

            var code = "var xPathRes = document.evaluate('/html/body/div[1]/div[1]/div[1]/div[3]/div/header/div[2]/div/span/div[3]/span/div[1]/ul/li[4]/div[1]', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null); xPathRes.singleNodeValue.click();";
            var code2 = "var xPathRes = document.evaluate('/html/body/div[1]/div[1]/div[1]/div[3]/div/header/div[2]/div/span/div[3]/span/div[1]/ul/li[5]/div[1]', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null); xPathRes.singleNodeValue.click();";
            Instancias.First(p => p.Usuario == i.Usuario).Browser.ExecuteScriptAsync(code0);
            Thread.Sleep(1000);
            Instancias.First(p => p.Usuario == i.Usuario).Browser.ExecuteScriptAsync(code);
            Instancias.First(p => p.Usuario == i.Usuario).Browser.ExecuteScriptAsync(code2);
            //Thread.Sleep(5000);
            Thread.Sleep(2000);
            //Instancias.First(p => p.Usuario == i.Usuario).Browser.GetBrowser().CloseBrowser(true);
            Thread a = new Thread(() => {
                ConectarInstania(i);
            });
            a.Start();
            //Muda o status para parar de atualizar mensagens
            // Instancias.First(p => p.Usuario == i.Usuario).Encerrar = true;

            //while (Instancias.Where(p => p.Usuario == i.Usuario).Count() != 0)
            //{
            //    Thread.Sleep(100);
            //}
        }

        private static void clearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
        }
        //Funções de Conexão

        public static async Task<Instancia> AtualizacaoRotineira(Instancia i)
        {
            //Atualiza status de conexão e nivel de bateria para emissão de alertas
            if (i.Browser != null)
            {
                var browser = i.Browser;
                var aguardando = false;
                Respostas.me data = null;

                aguardando = true;
                await browser.EvaluateScriptAsync("JSON.stringify(Store.Conn);").ContinueWith(y =>
                {
                    if ((string)y.Result.Result != null)
                    {
                        data = JsonConvert.DeserializeObject<Respostas.me>((string)y.Result.Result);
                    }
                    aguardando = false;
                });
                while (aguardando) { Thread.Sleep(1000); }

                if (data != null)
                {
                    Instancias.First(p => p.Usuario == i.Usuario).Bateria = data.battery;
                    Instancias.First(p => p.Usuario == i.Usuario).Conectado = true;
                }
                else
                {
                    if (!Instancias.First(p => p.Usuario == i.Usuario).Comunicacao)
                    {
                        Instancias.First(p => p.Usuario == i.Usuario).Conectado = false;
                    }
                }
            }

            return Instancias.First(p => p.Usuario == i.Usuario);

        }
        public static async void AtualizarDadosPessoais(Instancia i)
        {
            var browser = i.Browser;
            var dadosAntigos = new MainModels.Whatsapp();
            var dados = new MainModels.Whatsapp();
            var aguardando = false;

            using (var _context = new MainContext())
            {
                dadosAntigos = _context.Whatsapps.First(p => p.Usuario == i.Usuario);
            }
            Respostas.me data = null;

        GetConn:
            aguardando = true;
            await browser.EvaluateScriptAsync("xConn;").ContinueWith(y =>
            {
                if ((string)y.Result.Result != null)
                {
                    data = JsonConvert.DeserializeObject<Respostas.me>((string)y.Result.Result);
                }
                aguardando = false;
            });
            while (aguardando) { Thread.Sleep(1000); }
            if (data == null) { browser.ExecuteScriptAsync("var xConn = JSON.stringify(Store.Conn);"); goto GetConn; }

            if (data != null)
            {
                dados.Dispositivo = data.phone.device_manufacturer.ToUpper() + " " + data.phone.device_model.ToUpper();
                dados.Numero = data.wid.Replace("@c.us", "");
                dados.Nome = data.pushname;
                dados.Plataforma = data.platform;
            }

            aguardando = true;
            await browser.EvaluateScriptAsync("Store.ProfilePicThumb.get('" + dados.Numero + "@c.us').img").ContinueWith(async y =>
            {
                dados.Imagem = (string)y.Result.Result;

                await browser.EvaluateScriptAsync(File.ReadAllText("down.js").Replace("url", dados.Imagem).Replace("nome", dados.Numero + ".jpg")).ContinueWith(j =>
                {
                    if (j.Result.Success)
                    {
                        dados.Imagem = @"\Imagens\" + dados.Numero + ".jpg";
                        aguardando = false;
                    }
                });

            });
            while (aguardando) { Thread.Sleep(1000); }

            //Possivel verificação de divergência de dados

            //Atualiza dados
            using (var _context = new MainContext())
            {
                var x = _context.Whatsapps.First(p => p.Usuario == i.Usuario);
                x.Dispositivo = dados.Dispositivo;
                x.Numero = dados.Numero;
                x.Nome = dados.Nome;
                x.Imagem = dados.Imagem;
                x.Plataforma = dados.Plataforma;
                _context.Update(x);
                _context.SaveChanges();

                //Atualiza os dados na instancia
                Instancias.First(p => p.Usuario == i.Usuario).Whatsapp = x;
            }



            //Define instancia como conectada
            Instancias.First(p => p.Usuario == i.Usuario).Conectado = true;
            Instancias.First(p => p.Usuario == i.Usuario).Conectando = false;
        }
        public async static Task<string> ObterQrCode(ChromiumWebBrowser browser)
        {
            string result = "";

            await browser.EvaluateScriptAsync("document.querySelectorAll(\"[aria-label='Scan me!']\")[0].toDataURL()").ContinueWith(y =>
            {
                result = (string)y.Result.Result;
            });

            browser.ExecuteScriptAsync("function a(){if(document.querySelectorAll(\"[data-testid='refresh-large']\").length > 0){document.querySelectorAll(\"[data-testid='refresh-large']\")[0].click() }}a();");

            return result;
        }
        public async static Task<bool> ChecarConexao(ChromiumWebBrowser browser)
        {
            bool result = false;

            await browser.EvaluateScriptAsync("function a(){if(document.querySelectorAll(\"[data-testid='chat']\").length > 0){ return true; }else{return false;}}a();").ContinueWith(y =>
            {
                result = (bool)y.Result.Result;
            });

            return result;
        }
        public static ChromiumWebBrowser IniciarBrowser(string u)
        {

            var newsettings = new BrowserSettings();
            var browserSettings = new BrowserSettings();
            var requestContextSettings = new RequestContextSettings { CachePath = Environment.CurrentDirectory + @"\Log\" + u + @"\Browser" };
            var requestContext = new RequestContext(requestContextSettings);
            var browser = new ChromiumWebBrowser("web.whatsapp.com", browserSettings, requestContext);
            browser.DownloadHandler = new DownloadHandler();
            return browser;
        }

        public static string Decrypt(string tipo, string mediakey, string link, Instancia i, Chat c, string mimetype)
        {
            if (!i.PermitirChatbot) { return ""; }

            byte[] info = new byte[0];

            if (tipo.Contains("audio") || tipo.Contains("ptt"))
            {
                info = Encoding.UTF8.GetBytes("WhatsApp Audio Keys");
            }
            else if (tipo.Contains("image"))
            {
                info = Encoding.UTF8.GetBytes("WhatsApp Image Keys");
            }
            else if (tipo.Contains("sticker"))
            {
                info = Encoding.UTF8.GetBytes("WhatsApp Image Keys");
            }
            else if (tipo.Contains("document"))
            {
                info = Encoding.UTF8.GetBytes("WhatsApp Document Keys");
            }
            else if (tipo.Contains("video"))
            {
                info = Encoding.UTF8.GetBytes("WhatsApp Video Keys");
            }

            var file = DownloadFile(link, i, c);
            if (file == "")
            {
                return "";
            }

        INIT:
            try
            {
                var bytes = File.ReadAllBytes(file);
                //Remove os dois iniciais
                //bytes = bytes.ToList().Skip(2).ToArray();
                if (bytes.Length == 0)
                {
                    return "";
                }
                var nBytes = new byte[bytes.Length - 10];
                //var str = "[";
                //foreach(var item in bytes)
                //{
                //    str += item+", ";
                //}
                //str += "]";
                //System.Diagnostics.Debug.WriteLine(str);
                Array.Copy(bytes, 0, nBytes, 0, bytes.Length - 10);

                //Pega o mediakey base54 da mensagem em bytes decodificados
                byte[] data = Convert.FromBase64String(mediakey);

                //Obtem detalhes adicionais
                byte[] derivative = new HKDFv3().deriveSecrets(data, info, 112);
                byte[][] parts = ByteUtil.split(derivative, 16, 32);
                var iv = parts[0];
                var key = parts[1];

                using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
                {
                    aesAlg.Key = key;
                    aesAlg.IV = iv;
                    aesAlg.Mode = CipherMode.CBC;
                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption. 
                    using (MemoryStream msDecrypt = new MemoryStream(nBytes))
                    using (MemoryStream output = new MemoryStream())
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            csDecrypt.CopyTo(output);
                            // File.WriteAllBytes("1.ogg", output.ToArray());
                            return "data:" + mimetype + ";base64," + Convert.ToBase64String(output.ToArray());


                        }

                    }
                }
            }
            catch (Exception e)
            {
                goto INIT;
            }

        }
        public static string DownloadFile(string url, Instancia i, Chat c)
        {
            try
            {
                if (url == null) { return ""; }
                string nome = url.Split('/').Last();
                //Verifica existencia do diretorio, se não, o cria
                string path = Environment.CurrentDirectory + @"\Log\" + i.Usuario + @"\Chats\" + c.NumeroContato + @"\Encodeds\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //verifica se o download já foi feito
                if (!File.Exists(path + nome))
                {
                    using (WebClient myWebClient = new WebClient())
                    {
                        myWebClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:93.0) Gecko/20100101 Firefox/93.0";
                        myWebClient.DownloadFile(url, path + nome);
                        return path + nome;
                    }
                }
                else
                {
                    //Caso ja tenha baixado
                    return path + nome;
                }
            }
            catch (Exception e)
            {
                return "";
            }
        }


        //Processo principal
        public static void Init()
        {
            INIT = true;
            //Processo principal de gerenciamento de todas as instancias
            while (true)
            {
                //Percorre todas as instâncias conectadas
                foreach (var i in Instancias.FindAll(p => p.Conectado && p.Comunicacao))
                {
                    //Controle de possiveis instancias duplicadas
                    if (Instancias.FindAll(p => p.Usuario == i.Usuario && p.Encerrar == false).Count > 1)
                    {
                        i.Browser.GetBrowser().CloseBrowser(true);
                        i.Encerrar = true;
                        continue;
                    }

                    //Atualiza os chats e mensagens
                    //Inicia thread de atualização
                    if (!i.Atualizando)
                    {
                        i.Atualizando = true;
                        Thread x = new Thread(() => AtualizarConversas(i));
                        x.Start();
                    }
                    Thread.Sleep(100);
                }

                Instancias.RemoveAll(p => p.Encerrar == true);



                Thread.Sleep(1000);
            }
        }
        public static int counter = 0;


        public static async void AtualizarConversas(Instancia instancia)
        {
            try
            {
                while (true)
                {
                    counter++;
                    var i = Instancias.First(p => p.Usuario == instancia.Usuario);

                    var browser = i.Browser;
                    var wait = false;
                    //Requisita todos os chats do whatsapp conectado
                    wait = true;
                    await browser.EvaluateScriptAsync("JSON.stringify(WAPI.getAllChats());").ContinueWith(async y =>
                    {
                        if (y.Result.Success)
                        {
                            var chatsDict = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>((string)y.Result.Result);
                            var chats = JsonConvert.DeserializeObject<List<Respostas.chat>>((string)y.Result.Result);

                            //Percorre todos os chats recebidos     
                            var count = -1;
                            foreach (var item in chats)
                            {
                                var c = new Chat(); count++;
                                bool aguardando = false;

                                //Verifica a existencia de um chat com o mesmo id, se não, insere ele na lista e atualiza a imagem
                                if (i.Chats.Where(p => p.ID == item.id).Count() == 0)
                                {
                                    var n = new Chat
                                    {
                                        ID = item.id,
                                        NumeroContato = item.contact.id.Replace("@c.us", ""),
                                        Mensagens = new List<Mensagem>(),
                                    };


                                    //Atualização do Nome   -- //pushname: nome de registro //formattedName: nome q eu salvei
                                    var contact = JsonConvert.DeserializeObject<Dictionary<string, object>>(chatsDict[count]["contact"].ToString());
                                    if (contact.ContainsKey("name")) { n.NomeContato = contact["name"].ToString(); }
                                    else
                                    {
                                        if (contact.ContainsKey("verifiedName")) { n.NomeContato = contact["verifiedName"].ToString(); }
                                        else if (contact.ContainsKey("pushname")) { n.NomeContato = contact["pushname"].ToString(); }
                                        else { n.NomeContato = contact["formattedName"].ToString(); }
                                    }

                                    //Atualização da imagem
                                    aguardando = true;
                                    await browser.EvaluateScriptAsync("Store.ProfilePicThumb.get('" + n.ID + "').img").ContinueWith(async x =>
                                    {
                                        n.ImagemContato = (string)x.Result.Result;

                                        if (n.ImagemContato == null)
                                        {
                                            n.ImagemContato = "/app-assets/images/semfoto.jpg";
                                            aguardando = false;
                                        }
                                        else
                                        {
                                            await browser.EvaluateScriptAsync(File.ReadAllText("down.js").Replace("url", n.ImagemContato).Replace("nome", n.NumeroContato + ".jpg")).ContinueWith(j =>
                                            {
                                                if (j.Result.Success)
                                                {
                                                    n.ImagemContato = "/Imagens/" + n.NumeroContato + ".jpg";
                                                }
                                                aguardando = false;
                                            });
                                        }

                                    });
                                    while (aguardando) { Thread.Sleep(100); }


                                    i.Chats.Add(n);
                                }

                                //Vincula o modelo de chat inicial ao chat já existente ou recém criado
                                c = i.Chats.First(p => p.ID == item.id);

                                //Atualização de mensagens                        
                                aguardando = true;
                                c.NaoLidas = Convert.ToInt32(item.unreadCount);
                                await browser.EvaluateScriptAsync("JSON.stringify(WAPI.getAllMessagesInChat('" + c.ID + "', true, false))").ContinueWith(y =>
                                {
                                    if (y.Result.Success)
                                    {
                                        if (y.Result.Result.ToString() != "{}")
                                        {

                                            var data = JsonConvert.DeserializeObject<List<Respostas.message>>((string)y.Result.Result);
                                            foreach (var msg in data)
                                            {
                                                //Caso essa menasgem já não tenha sido computada nesse chat
                                                if (c.Mensagens.FindAll(p => p.ID == msg.id).Count == 0)
                                                {
                                                    ProcessarMensagem(c, msg, i);
                                                }
                                            }
                                            if (data.Count() != 0)
                                            {
                                                c.UltimaMensagem = ConvertMsg(data.Last());
                                                c.UltimaMensagemHorario = DateTimeOffset.FromUnixTimeSeconds(data.Last().timestamp).AddHours(-3);
                                            }
                                            aguardando = false;
                                        }
                                        else
                                        {
                                            aguardando = false;
                                        }
                                    }
                                });
                                while (aguardando) { Thread.Sleep(100); }




                            }
                        }
                        else
                        {
                            if (y.Result.Message.Contains("WAPI is"))
                            {
                                browser.ExecuteScriptAsync(System.IO.File.ReadAllText("sharp.cef"));
                            }
                        }
                        wait = false;
                    });
                    while (wait) { Thread.Sleep(100); }
                    Instancias.First(p => p.Usuario == instancia.Usuario).PermitirChatbot = true;
                    

                    Thread.Sleep(1000); //Atualiza a cada 1 segundos em busca de novas mensagens }
                }

            }
            catch (Exception e)
            {

            }
        }

        public static void ProcessarMensagem(Chat c, Respostas.message msg, Instancia i)
        {
            var nome = c.NomeContato;

            var n = new Mensagem
            {
                Destinatario = msg.to,
                Horario = DateTimeOffset.FromUnixTimeSeconds(msg.timestamp).AddHours(-3),
                Texto = msg.body,
                Remetente = msg.from,
                Tipo = msg.type,
                ID = msg.id,
                Chat = c.ID,
                FromMe = msg.fromMe,
                NomeRemetente = nome,
                caption = msg.caption,
                mimetype = msg.mimetype,
            };
            if(msg.quotedMsg != null)
            {
                n.Quoted = msg.quotedMsg.body;
            }
            //Busca pre referencia para vincular à um remetente
            var mensagem = msg.body;
            if (msg.caption != null) { mensagem = msg.caption; }
            if (i.PreReferencias.Where(p => p.Chat == n.Chat && p.Mensagem == mensagem).Count() != 0)
            {
                var r = i.PreReferencias.Where(p => p.Chat == n.Chat && p.Mensagem == mensagem).First();
                n.NomeAtendente = r.Remetente;
                Instancias.First(p => p.Usuario == i.Usuario).PreReferencias.RemoveAll(p => p.Chat == n.Chat && p.Mensagem == mensagem);
            }

            if (n.Tipo == "location")
            {
                n.lat = msg.lat;
                n.lng = msg.lng;
            }

            if (n.Tipo == "audio" || n.Tipo == "ptt")
            {
                n.B64Media = Decrypt(n.Tipo, msg.mediaKey, msg.deprecatedMms3Url, i, c, msg.mimetype);
            }
            if (n.Tipo == "image")
            {
                n.B64Media = Decrypt(n.Tipo, msg.mediaKey, msg.deprecatedMms3Url, i, c, msg.mimetype);
            }
            if (n.Tipo == "sticker")
            {
                n.B64Media = Decrypt(n.Tipo, msg.mediaKey, msg.deprecatedMms3Url, i, c, msg.mimetype);
            }
            if (n.Tipo == "video")
            {
                n.B64Media = Decrypt(n.Tipo, msg.mediaKey, msg.deprecatedMms3Url, i, c, msg.mimetype);
            }
            if (n.Tipo == "document")
            {
                n.B64Media = Decrypt(n.Tipo, msg.mediaKey, msg.deprecatedMms3Url, i, c, msg.mimetype);
            }

            if (n.FromMe) { n.ImagemRemetente = i.Whatsapp.Imagem; }
            else { n.ImagemRemetente = c.ImagemContato; }



            //Verifica se as mensagens já carregaram totalmente, para não processar nesse momento de load
            if (i.PermitirChatbot && i.Comunicacao)
            {

                //Verifica se existe um ticket para esse chat em aberto e adiciona a mensagem nele
                if (i.Tickets.Where(p => p.Chat == c.ID && p.Status != "Encerrado").Count() != 0)
                {
                    Instancias.First(p => p.Usuario == i.Usuario).Tickets.First(p => p.Chat == c.ID && p.Status != "Encerrado").NaoLidas = c.NaoLidas;
                    Instancias.First(p => p.Usuario == i.Usuario).Tickets.First(p => p.Chat == c.ID && p.Status != "Encerrado").Mensagens.Add(n);
                }

                //Enviamos a mensagem para o chatbot processar a resposta
                Thread x = new Thread(() => ChatBot(c, n, i)); x.Start();

            }


            //Armazenamos essa mensagem na lista
            c.Mensagens.Add(n);
        }

        public static void ChatBot(Chat c, Mensagem n, Instancia i)
        {
            //Não processa mensagens enviadas por si msm
            if (n.FromMe) { return; }

            //Trava de segurança para testes, só permitirá envio para esse numero
            //if(n.Remetente != "5521988766618@c.us") { return; }

            //Localizamos o chatbot desse usuario
            var bot = BuscarChatbot(i);

            //Verificar se temos um ticket aberto para direcionar essa mensagem
            if (VerificarTicketExistente(n.Chat, i))
            {
                var t = BuscarTicket(n.Chat, bot, i);


                if (bot.ChaveEncerrar == n.Texto)
                {
                    //Encerra o ticket                    
                    EnviarMensagem(null, "Ticket da Conversa Finalizada: " + t.Protocolo, c, t, i, "BOT", "");
                    EncerrarTicket(t, i, c);
                    return;
                }

                //Verifica se é possivel enviar mensagens automaticas para esse ticket no momento (em atendimento por humano ou standby)
                if (VerificarPossibilidadeEnvio(t))
                {
                    //Verifica se a sessão atual ainda está acessivel
                    if (bot.Sessoes.FindAll(p => p.ID == t.SessaoAtual).Count() == 0)
                    {
                        return;
                    }


                    //Caso seja possivel enviar, então busca a sessao atual
                    var s = bot.Sessoes.First(p => p.ID == t.SessaoAtual);

                    var opcao = new Opcao();

                    //Busca nas opções alguma que a palavra chave seja */* 
                    if (s.Opcoes.FindAll(p => p.Palavra == "*/*").Count() != 0)
                    {
                        opcao = s.Opcoes.First(p => p.Palavra == "*/*");
                    }

                    //Busca nas opções alguma que a palavra chave seja igual a mensagem atual
                    if (s.Opcoes.FindAll(p => p.Palavra == n.Texto).Count() != 0)
                    {
                        opcao = s.Opcoes.First(p => p.Palavra == n.Texto);
                    }

                    //Verifica se é alguma das chaves padrão
                    if (bot.ChaveInicio == n.Texto)
                    {

                        //Seta bot com a sessão de inicio e armazena a atual, envia texto referente
                        var anterior = t.SessaoAtual; var atual = bot.ChaveInicioDestino;
                        Instancias.First(p => p.Usuario == i.Usuario).Tickets.First(p => p.ID == t.ID).MenuAnterior = anterior; t.MenuAnterior = anterior;
                        Instancias.First(p => p.Usuario == i.Usuario).Tickets.First(p => p.ID == t.ID).SessaoAtual = atual; t.SessaoAtual = atual;
                        EnviarMensagem(bot.Sessoes.First(p => p.ID == bot.ChaveInicioDestino).Imagem, bot.Sessoes.First(p => p.ID == bot.ChaveInicioDestino).Texto, c, t, i, "BOT", "");
                        return;
                    }

                    if (bot.ChaveAtual == n.Texto)
                    {
                        EnviarMensagem(bot.Sessoes.First(p => p.ID == t.SessaoAtual).Imagem, bot.Sessoes.First(p => p.ID == t.SessaoAtual).Texto, c, t, i, "BOT","");

                    }

                    if (bot.ChaveHumano == n.Texto)
                    {
                        //Coloca ticket em espera do atendimento humano
                        EnviarMensagem(null, "Aguarde um momento, buscando atendente...", c, t, i, "BOT","");
                        SolicitarHumano(t, i);
                        return;
                    }

                    if (bot.ChaveMenuAnterior == n.Texto)
                    {
                        //Retorna ao menu anterior
                        if (t.MenuAnterior == 0) { t.MenuAnterior = t.SessaoAtual; }
                        var anterior = t.MenuAnterior; var atual = t.SessaoAtual;
                        Instancias.First(p => p.Usuario == i.Usuario).Tickets.First(p => p.ID == t.ID).MenuAnterior = atual; t.MenuAnterior = atual;
                        Instancias.First(p => p.Usuario == i.Usuario).Tickets.First(p => p.ID == t.ID).SessaoAtual = anterior; t.SessaoAtual = anterior;
                        EnviarMensagem(bot.Sessoes.First(p => p.ID == anterior).Imagem, bot.Sessoes.First(p => p.ID == anterior).Texto, c, t, i, "BOT","");
                        return;
                    }

                    //Caso não tenha nenhuma opção que aceite qualquer coisa e nem uma opção com chave que combine , enviamos mensagem de erro
                    if (s.Opcoes.FindAll(p => p.Palavra == n.Texto || p.Palavra == "*/*").Count() == 0)
                    {
                        EnviarMensagem(null, "Não entendi, por favor selecione uma outra opção. \\nDigite *" + bot.ChaveAtual + "* para ver as opções disponíveis.", c, t, i, "BOT", "");
                    }
                    else
                    {
                        //Caso contrario processa de acordo com a opção escolhida
                        if (opcao.Tipo == "Chamada para API")
                        {

                            //Processa chamada na api
                            var retornoApi = ProcessarChamadaAPI(i, t, c, opcao.Api, n.Texto);
                            if (retornoApi.Sucesso)
                            {
                                //Seta nova sessão, envia retorno da API e Mensagem da sessão atual
                                var anterior = t.SessaoAtual; var atual = opcao.SessaoRef;
                                Instancias.First(p => p.Usuario == i.Usuario).Tickets.First(p => p.ID == t.ID).MenuAnterior = anterior; t.MenuAnterior = anterior;
                                Instancias.First(p => p.Usuario == i.Usuario).Tickets.First(p => p.ID == t.ID).SessaoAtual = atual; t.SessaoAtual = atual;
                                EnviarMensagem(null, retornoApi.Mensagem, c, t, i, "BOT", "");
                                EnviarMensagem(bot.Sessoes.First(p => p.ID == atual).Imagem, bot.Sessoes.First(p => p.ID == atual).Texto, c, t, i, "BOT", "");

                            }
                            else
                            {
                                EnviarMensagem(null, retornoApi.Mensagem, c, t, i, "BOT", "");
                            }

                        }
                        else if (opcao.Tipo == "Abrir outra sessão")
                        {
                            //Seta a nova sessão e envia mensagem referente
                            var anterior = t.SessaoAtual; var atual = opcao.SessaoRef;
                            Instancias.First(p => p.Usuario == i.Usuario).Tickets.First(p => p.ID == t.ID).MenuAnterior = anterior; t.MenuAnterior = anterior;
                            Instancias.First(p => p.Usuario == i.Usuario).Tickets.First(p => p.ID == t.ID).SessaoAtual = atual; t.SessaoAtual = atual;
                            EnviarMensagem(bot.Sessoes.First(p => p.ID == atual).Imagem, bot.Sessoes.First(p => p.ID == atual).Texto, c, t, i, "BOT", "");
                        }
                        else if (opcao.Tipo == "Enviar uma mensagem")
                        {
                            EnviarMensagem(null, opcao.Mensagem, c, t, i, "BOT", "");
                        }
                    }
                }
            }
            else
            {
                //Se não, cria um novo ticket
                var t = CriarTicket(c, i, "BOT");
                Instancias.First(p => p.Usuario == i.Usuario).Tickets.First(p => p.ID == t.ID && p.Status == "BOT").Mensagens.Add(n);

                //Envia mensagem inicial
                EnviarMensagem(bot.Sessoes.First(p => p.ID == bot.ChaveInicioDestino).Imagem, bot.Sessoes.First(p => p.ID == bot.ChaveInicioDestino).Texto, c, t, i, "BOT", "");
            }
        }

        public static RetornoChamadaAPI ProcessarChamadaAPI(Instancia i, TicketBruto t, Chat c, string chamada, string content)
        {
            var result = new RetornoChamadaAPI();
            t = Instancias.First(p => p.Usuario == i.Usuario).Tickets.First(p => p.ID == t.ID);
            if (chamada.Contains("[MKAUTH]"))
            {
                if (chamada.Contains("Última Fatura"))
                {

                    var x = MKAUTH.API.Titulo.Listagem(i.MKAUTH);
                    var titulos = x.titulos.Where(p => p.cpf_cnpj == t.Documento).ToList();
                    if (titulos.Count() != 0)
                    {
                        var msg = "Você possui (" + titulos.Count + ") faturas no sistema. \r\n\r\n";
                        var last = titulos.Last();
                        msg += "Vencimento: " + last.datavenc + " | Valor: R$" + last.valor + " | Status: " + last.status.ToUpper() + "\r\n";
                        msg += "Linha Digitável: *" + last.linhadig + "*\r\n\r\n";


                        result.Sucesso = true;
                        result.Mensagem = msg;
                    }
                    else
                    {

                        result.Sucesso = false;
                        result.Mensagem = "Não foi possível localizar nenhuma fatura em seu documento, tente novamente outra hora!";
                    }
                }
                else if (chamada.Contains("Histórico de Faturas"))
                {

                    var x = MKAUTH.API.Titulo.Listagem(i.MKAUTH);
                    var titulos = x.titulos.Where(p => p.cpf_cnpj == t.Documento).ToList(); if (titulos.Count() != 0)
                    {
                        var msg = "Você possui (" + titulos.Count + ") faturas no sistema. \r\n\r\n";

                        foreach (var item in titulos)
                        {
                            msg += "Vencimento: " + item.datavenc + " | Valor: R$" + item.valor + " | Status: " + item.status.ToUpper() + "\r\n";
                            msg += "Linha Digitável: *" + item.linhadig + "*\r\n\r\n";
                        }


                        result.Sucesso = true;
                        result.Mensagem = msg;
                    }
                    else
                    {

                        result.Sucesso = false;
                        result.Mensagem = "Não foi possível localizar nenhuma fatura em seu documento, tente novamente outra hora!";
                    }

                }
                else if (chamada.Contains("Fatura Atrasada"))
                {
                    var x = MKAUTH.API.Titulo.Listagem(i.MKAUTH);
                    var titulos = x.titulos.Where(p => p.cpf_cnpj == t.Documento && p.status == "vencido").ToList(); if (titulos.Count() != 0)
                    {
                        var msg = "Você possui (" + titulos.Count + ") faturas atrasadas no sistema. \r\n\r\n";

                        foreach (var item in titulos)
                        {
                            msg += "Vencimento: " + item.datavenc + " | Valor: R$" + item.valor + " | Status: " + item.status.ToUpper() + "\r\n";
                            msg += "Linha Digitável: *" + item.linhadig + "*\r\n\r\n";
                        }


                        result.Sucesso = true;
                        result.Mensagem = msg;
                    }
                    else
                    {

                        result.Sucesso = false;
                        result.Mensagem = "Não foi possível localizar nenhuma fatura em seu documento, tente novamente outra hora!";
                    }
                }
                else if (chamada.Contains("Próxima Fatura"))
                {
                    var x = MKAUTH.API.Titulo.Listagem(i.MKAUTH);
                    var titulos = x.titulos.Where(p => p.cpf_cnpj == t.Documento && Convert.ToDateTime(p.datavenc) >= DateTime.Now).ToList();
                    if (titulos.Count() != 0)
                    {
                        var msg = "";

                        foreach (var item in titulos)
                        {
                            msg += "Vencimento: " + item.datavenc + " | Valor: R$" + item.valor + " | Status: " + item.status.ToUpper() + "\r\n";
                            msg += "Linha Digitável: *" + item.linhadig + "*\r\n\r\n";
                        }


                        result.Sucesso = true;
                        result.Mensagem = msg;
                    }
                    else
                    {

                        result.Sucesso = false;
                        result.Mensagem = "Não foi possível localizar nenhuma fatura em seu documento, tente novamente outra hora!";
                    }
                }
                else if (chamada.Contains("Consultar Planos"))
                {

                    var x = MKAUTH.API.Plano.Listagem(i.MKAUTH);
                    if (x.planos.Count() != 0)
                    {
                        var msg = "";

                        foreach (var item in x.planos)
                        {
                            msg += "\r\n\r\n####################\r\n";
                            msg += "Plano: " + item.nome + "\r\n";
                            msg += "Valor: R$" + item.valor + "\r\n";
                            msg += "Descrição: " + item.descricao + "\r\n";
                            msg += "Download: " + item.veldown + "\r\n";
                            msg += "Upload: " + item.velup + "\r\n";
                        }

                        msg += "\r\n####################\r\n";
                        result.Sucesso = true;
                        result.Mensagem = msg;
                    }
                    else
                    {

                        result.Sucesso = false;
                        result.Mensagem = "Não foi possível localizar nenhuma fatura em seu documento, tente novamente outra hora!";
                    }
                }
                else if (chamada.Contains("Meu Plano"))
                {

                    var clientes = MKAUTH.API.Cliente.Listagem(i.MKAUTH);
                    var cliente = MKAUTH.API.Cliente.Buscar(i.MKAUTH, clientes.clientes.First(p => p.cpf_cnpj == t.Documento).uuid);
                    var planos = MKAUTH.API.Plano.Listagem(i.MKAUTH);
                    var plano = planos.planos.Where(p => p.nome == cliente.dados[0].plano);
                    var msg = "";
                    if (plano.Count() != 0)
                    {
                        foreach (var item in plano)
                        {
                            msg += "\r\n\r\n####################\r\n";
                            msg += "Plano: " + item.nome + "\r\n";
                            msg += "Valor: R$" + item.valor + "\r\n";
                            msg += "Descrição: " + item.descricao + "\r\n";
                            msg += "Download: " + item.veldown + "\r\n";
                            msg += "Upload: " + item.velup + "\r\n";
                        }
                        msg += "\r\n####################\r\n";

                        result.Sucesso = true;
                        result.Mensagem = msg;
                    }
                    else
                    {
                        result.Sucesso = false;
                        result.Mensagem = "Não foram encontradas informações sobre seu plano atual.";
                    }
                }
                else if (chamada.Contains("Minha Instalação"))
                {

                    var clientes = MKAUTH.API.Cliente.Listagem(i.MKAUTH);
                    var cliente = clientes.clientes.First(p => p.cpf_cnpj == t.Documento);
                    var xx = MKAUTH.API.Instalacao.Listagem(i.MKAUTH);
                    var x = xx.instalacoes.Where(p => p.login == cliente.login).ToList();
                    if (x.Count() != 0)
                    {
                        var msg = "";
                        foreach (var item in x)
                        {
                            msg += "Nome: " + item.nome + "\r\n";
                            msg += "Email: " + item.email + "\r\n";
                            msg += "Login: " + item.login + "\r\n";
                            msg += "Senha: " + item.senha + "\r\n";
                        }
                        result.Sucesso = true;
                        result.Mensagem = msg;

                    }
                    else
                    {
                        result.Sucesso = false;
                        result.Mensagem = "Não foi possível localizar a sua instalação, tente novamente depois!";

                    }
                }
                else if (chamada.Contains("Contas Bancárias"))
                {


                    var contas = MKAUTH.API.Conta.Listagem(i.MKAUTH);
                    if (contas.contas.Count() != 0)
                    {
                        var msg = "";
                        foreach (var item in contas.contas)
                        {
                            msg += "Banco: " + item.banco + "\r\n";
                            msg += "Agência: " + item.agencia + "\r\n";
                            msg += "Dígito Agência: " + item.ag_digito + "\r\n";
                            msg += "Conta: " + item.conta + "\r\n";
                            msg += "Dígito Conta: " + item.ct_digito + "\r\n\r\n\r\n";
                        }
                        result.Sucesso = true;
                        result.Mensagem = msg;
                    }
                    else
                    {

                        result.Sucesso = false;
                        result.Mensagem = "Não foi possível localizar nenhuma conta no momento, tente novamente depois!";
                    }
                }
                else if (chamada.Contains("Buscar Cliente"))
                {

                    //tratamento
                    if (content != null)
                    {
                        content = content.Replace(".", "");
                        content = content.Replace(",", "");
                        content = content.Replace("-", "");
                        content = content.Replace("/", "");

                        var x = MKAUTH.API.Cliente.Listagem(i.MKAUTH);
                        if (x.clientes.Where(p => p.cpf_cnpj == content).Count() != 0)
                        {
                            var r = x.clientes.First(p => p.cpf_cnpj == content);
                            var msg = "Seja bem-vindo(a) " + r.nome;
                            result.Sucesso = true;
                            result.Mensagem = msg;

                            //Seta nesse ticket o numero do documento para futuras consultas
                            Instancias.First(p => p.Usuario == i.Usuario).Tickets.First(p => p.ID == t.ID).Documento = content;
                        }
                        else
                        {
                            result.Sucesso = false;
                            result.Mensagem = "Não foi possível localizar este documento, tente novamente!";
                        }
                    }
                }
                else if (chamada.Contains("Meus Chamados"))
                {

                    var clientes = MKAUTH.API.Cliente.Listagem(i.MKAUTH);
                    var cliente = clientes.clientes.First(p => p.cpf_cnpj == t.Documento);
                    var xx = MKAUTH.API.Chamado.Listagem(i.MKAUTH);
                    var x = xx.chamados.Where(p => p.login == cliente.login).ToList();
                    if (x.Count() != 0)
                    {
                        var msg = "";
                        foreach (var item in x)
                        {
                            msg += "Abertura: " + item.abertura + " | Chamado: " + item.chamado + "| Código: " + item.uuid + "\r\n";
                        }
                        result.Sucesso = true;
                        result.Mensagem = msg;
                    }
                    else
                    {
                        result.Sucesso = false;
                        result.Mensagem = "Não foram encontrados nenhum chamado na sua conta";
                    }
                }
                else if (chamada.Contains("Visualizar Chamado"))
                {
                    var clientes = MKAUTH.API.Cliente.Listagem(i.MKAUTH);
                    var cliente = clientes.clientes.First(p => p.cpf_cnpj == t.Documento);
                    var x = MKAUTH.API.Chamado.Buscar(i.MKAUTH, content);
                    if (x != null)
                    {
                        var msg = "";
                        msg += "Abertura: " + x.abertura + "\r\n";
                        msg += "Fechamento: " + x.fechamento + "\r\n";
                        msg += "Assunto: " + x.assunto + "\r\n";
                        msg += "Atendente: " + x.atendente + "\r\n";
                        msg += "Chamado: " + x.chamado + "\r\n";
                        msg += "Status: " + x.status + "\r\n";
                        msg += "Técnico: " + x.tecnico + "\r\n";
                        msg += "Visita: " + x.visita + "\r\n";
                        result.Sucesso = true;
                        result.Mensagem = msg;
                    }
                    else
                    {
                        result.Sucesso = false;
                        result.Mensagem = "Não foi possível localizar esse chamado";
                    }
                }
            }
            else if (chamada.Contains("[SGP]")) { }
            else if (chamada.Contains("[IXC]")) { }

            return result;
        }
        public static string GerarProtocolo(string ultimos)
        {
            //var data = DateTime.Now.ToShortDateString().Replace("/","");
            //var hora = DateTime.Now.ToLongTimeString();
            //var result = data + hora.Replace(":","") + ultimos;
            var data = DateTime.Now.ToString("ddMMyyyHHmmss");
            return data;
        }
        public static TicketBruto CriarTicket(Chat c, Instancia i, string atendente)
        {
            var bot = BuscarChatbot(i);
            var n = new Ticket
            {
                Atendente = atendente,
                Chat = c.ID,
                Chatbot = bot.ID,
                Inicio = DateTime.Now,
                Gerente = i.Usuario,
                Observacao = "",
                Path = "\\Log\\" + i.Usuario + "\\Chats\\" + c.NumeroContato + "\\Tickets\\",
                Termino = null,
                Humano = false,
                Nome = c.NomeContato,
                Imagem = c.ImagemContato,
                Telefone = c.NumeroContato,
                Protocolo = GerarProtocolo(c.NumeroContato.Substring(c.NumeroContato.Length - 4)),
                NaoLidas = c.NaoLidas
            };

            n.Path = Environment.CurrentDirectory + @"\Log\" + i.Usuario + @"\Chats\" + c.NumeroContato + @"\Tickets\" + n.Protocolo;

            if (atendente == "BOT")
            {
                n.Status = "BOT";
            }

            if (atendente != "BOT")
            {
                n.Status = "Humano";
                n.Humano = true;
            }

            using (var _context = new MainContext())
            {
                var x = _context.Tickets.Add(n);
                _context.SaveChanges();
                n.ID = x.Entity.ID;
            }

            var nTicket = BuscarTicket(c.ID, bot, i);
            Instancias.First(p => p.Usuario == i.Usuario).Tickets.Add(nTicket);
            return nTicket;
        }
        public static void SolicitarHumano(Ticket t, Instancia i)
        {
            using (var _context = new MainContext())
            {
                //Atualiza no banco
                _context.Tickets.First(p => p.ID == t.ID).Humano = true;
                _context.Tickets.First(p => p.ID == t.ID).Status = "Espera";
                _context.SaveChanges();
            }
            //Atualiza no local
            i.Tickets.First(p => p.ID == t.ID).Humano = true;
            i.Tickets.First(p => p.ID == t.ID).Status = "Espera";
        }
        public static void ArmazenarTicket(TicketBruto t, Instancia i, Chat c)
        {
            var path = t.Path;
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            File.WriteAllText(path + @"\data.json", JsonConvert.SerializeObject(t));
        }
        public static void EncerrarTicket(TicketBruto t, Instancia i, Chat c)
        {

            t.Status = "Encerrado";
            t.Termino = DateTime.Now;

            using (var _context = new MainContext())
            {
                //Atualiza no banco
                _context.Tickets.First(p => p.ID == t.ID).Status = t.Status;
                _context.Tickets.First(p => p.ID == t.ID).Termino = t.Termino;
                _context.SaveChanges();
            }

            //Atualiza no local
            i.Tickets.First(p => p.ID == t.ID).Status = t.Status;
            i.Tickets.First(p => p.ID == t.ID).Termino = t.Termino;

            ArmazenarTicket(i.Tickets.First(p => p.ID == t.ID), i, c);

        }
        public static void EnviarArquivo(string caption, string b64, string arquivo, Chat c, Instancia i, bool ptt, string duration, string reply)
        {
            var code = @"var c = Store.Chat.models.find(c => c.id._serialized ===  '" + c.ID + @"');                
                        var mediaBlob = window.WAPI.base64ImageToFile(b64, '" + arquivo + @"');
                        var mc = new Store.MediaCollection(c);
                        mc.processAttachments([{file: mediaBlob}, 1], c, 1).then(async () => {
                            var media = mc.models[0];            
                            await media.sendToChat(c, { caption:'" + caption + @"', quotedMsg: Store.Msg.get('" + reply + @"') });               
                        });";

            var codePtt = @"var c = Store.Chat.models.find(c => c.id._serialized ===  '" + c.ID + @"');                
                        var mediaBlob = window.WAPI.base64AudioToFile(b64);
                        var mc = new Store.MediaCollection(c);
                        mc.processAttachments([{file: mediaBlob}, 1], c, 1).then(async () => {
                            var media = mc.models[0];  
                            media.type = 'ptt';
                            media.mediaPrep._mediaData.__x_type = 'ptt';
                            media.mediaPrep._baseType = 'ptt';
                            await media.sendToChat(c, { caption:'" + caption + @"', quotedMsg: Store.Msg.get('"+reply+@"') });               
                        });";

            i.Browser.ExecuteScriptAsync("var b64 = '" + b64 + "';");

            if (ptt)
            {
                i.Browser.EvaluateScriptAsync(codePtt).ContinueWith(y => {
                    if (y.Result.Success)
                    {

                    }
                });
            }
            else
            {
                i.Browser.EvaluateScriptAsync(code).ContinueWith(y => {
                    if (y.Result.Success)
                    {

                    }
                });
            }
        }
        public static async void EnviarMensagem(string? media, string msg, Chat c, TicketBruto t, Instancia i, string Remetente, string reply)
        {
            if (msg == null) { return; }

            //Adiciona pre referencia ao remetente
            var nRef = new PreReferencia
            {
                Remetente = Remetente,
                Mensagem = msg,
                Chat = c.ID
            };
            if (media != null && msg == null)
            {
                nRef.Mensagem = Convert.ToString(media);
            }
            Instancias.First(p => p.Usuario == i.Usuario).PreReferencias.Add(nRef);


            //var code = "Promise.resolve(WAPI.sendMessage('5521974559103@c.us','u52i').then(function() { return WAPI.getChat('5521974559103@c.us').lastReceivedKey._serialized}))";
            //var code = "WAPI.sendMessage(\"" + c.ID + "\",\"" + msg.Replace("\r\n", "\\n") + "\")";
            var code = "WAPI.getChat('" + c.ID + "').sendMessage('" + msg.Replace("\r\n", "\\n") + "',  { quotedMsg: Store.Msg.get('" + reply + "') })";
            if (media != null)
            {
                code = "var b64 = 'data:image/jpeg;base64," + media + "'; var mediaBlob=window.WAPI.base64ImageToFile(b64,\"audio.ogg\"),chat=Store.Chat.models.find(e=>\"" + c.ID + "\"==e.id),mc=new Store.MediaCollection(chat);mc.processAttachments([{file:mediaBlob}],chat).then(()=>{var e=mc.models[0];e.mediaPrep._mediaData.__x_type=\"image\",e.mediaPrep._baseType=\"image\",e.type=\"image\",e.__x_type=\"image\",e.mediaPrep.sendToChat(chat,{caption:\"" + msg.Replace("\r\n", "\\n") + "\" , quotedMsg: Store.Msg.get('" + reply + @"')}),mc.delete()});";
                i.Browser.ExecuteScriptAsync(code);
            }
            else
            {
                i.Browser.ExecuteScriptAsync(code);
                //i.Browser.ExecuteScriptAsync(code);                
            }

            //Thread.Sleep(2000);

            ////Pega o id da ultima mensagem enviada para esse chat
            //bool aguardando = true;
            //await i.Browser.EvaluateScriptAsync("WAPI.getChat('" + c.ID + "').lastReceivedKey._serialized;", TimeSpan.FromSeconds(10)).ContinueWith(y =>
            //{
            //    var result = y.Result.Result;
            //    nRef.ID = result.ToString();
            //    aguardando = false;
            //});
            //while (aguardando) { Thread.Sleep(100); }

        }
        public static ChatbotBruto BuscarChatbot(Instancia i)
        {
            //Se não possui um, cria um novo
            if (i.Chatbot == null)
            {
                using (var _context = new MainContext())
                {
                    var chatbot = _context.Chatbots.First(p => p.Usuario == i.Usuario);

                    var configuration = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Chatbot, ChatbotBruto>();
                        cfg.CreateMap<Ticket, TicketBruto>();
                        cfg.CreateMap<Sessao, SessaoBruto>();
                    });

                    var mapper = configuration.CreateMapper();
                    var chatbotBruto = mapper.Map<ChatbotBruto>(chatbot);

                    //Busca sessões e opções
                    var sessoes = _context.Sessoes.Where(p => p.Usuario == i.Usuario).ToList();
                    var sessoesBruto = mapper.Map<List<SessaoBruto>>(sessoes).ToList();
                    foreach (var item in sessoesBruto)
                    {
                        item.Opcoes = _context.Opcoes.Where(p => p.Sessao == item.ID).ToList();
                    }
                    chatbotBruto.Sessoes = sessoesBruto;

                    //Atualiza para sessao inicial caso tenha sido criado recenetemente
                    //if(t.SessaoAtual == 0) { t.SessaoAtual = chatbotBruto.ChaveInicioDestino; }

                    Instancias.First(p => p.Usuario == i.Usuario).Chatbot = chatbotBruto;
                    return chatbotBruto;
                }
            }
            else
            {
                return Instancias.First(p => p.Usuario == i.Usuario).Chatbot;
            }
        }
        public static bool VerificarPossibilidadeEnvio(Ticket t)
        {
            if (t.Humano) { return false; }
            else if (t.Status == "BOT") { return true; }
            else { return false; }
        }
        public static bool VerificarTicketExistente(string chat, Instancia i)
        {
            if (i.Tickets.Where(p => p.Chat == chat && p.Status != "Encerrado").Count() != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static TicketBruto BuscarTicket(string chat, ChatbotBruto bot, Instancia i)
        {
            if (i.Tickets.Where(p => p.Chat == chat && p.Status != "Encerrado").Count() != 0)
            {
                return i.Tickets.Where(p => p.Chat == chat && p.Status != "Encerrado").First();
            }
            else
            {
                using (var _context = new MainContext())
                {
                    //Verifica 

                    var ticket = _context.Tickets.First(p => p.Chat == chat && p.Status != "Encerrado");
                    var configuration = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Ticket, TicketBruto>();
                    });

                    var mapper = configuration.CreateMapper();
                    var n = mapper.Map<TicketBruto>(ticket);

                    if (n.Mensagens == null)
                    {
                        n.Mensagens = new List<Mensagem>();
                    }

                    if (n.SessaoAtual == 0) { n.SessaoAtual = bot.ChaveInicioDestino; }

                    return n;
                }
            }
        }

        public static string ConvertMsg(Respostas.message d)
        {
            string x = "";
            x = d.body;
            if (d.type == "audio") { x = "[ÁUDIO]"; }
            if (d.type == "ptt") { x = "[ÁUDIO]"; }
            if (d.type == "image") { x = "[IMAGEM]"; }
            if (d.type == "video") { x = "[VÍDEO]"; }
            if (d.type == "document") { x = "[DOCUMENTO]"; }
            if (d.type == "sticker") { x = "[FIGURINHA]"; }
            if (d.type == "location") { x = "[LOCALIZAÇÃO]"; }
            if (d.type == "vcard") { x = "[VCARD]"; }
            if (d.type == "multi_vcard") { x = "[VCARD]"; }
            if (d.type == "revoked") { x = "[REVOKED]"; }
            if (d.type == "order") { x = "[ORDER]"; }
            if (d.type == "unknown") { x = "[UNKNOWN]"; }
            return x;
        }

    }
}
