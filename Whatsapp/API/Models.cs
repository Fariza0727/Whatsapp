using Whatsapp.Data;
using static Whatsapp.Data.MainModels;

namespace Whatsapp.API
{
    public class Models
    {
        public class RetornoChamadaAPI
        {
            public string Mensagem { get; set; }
            public bool Sucesso { get; set; }
        }
        public class ChatbotBruto : Chatbot
        {
            public List<SessaoBruto> Sessoes { get; set; }
        }
        public class SessaoBruto : Sessao
        {
            public List<Opcao> Opcoes { get; set; }
        }
        public class TicketBruto : Ticket
        {
            public int SessaoAtual { get; set; }
            public int MenuAnterior { get; set; }
            public string Documento { get; set; }
            public List<Mensagem> Mensagens { get; set; }
        }

        public class PreReferencia
        {
            public string Remetente { get; set; }
            public string Mensagem { get; set; }
            public string Chat { get; set; }

        }
        public class Instancia
        {
            public string Usuario { get; set; }
            public ChromiumWebBrowser Browser { get; set; }
            public string QrCode { get; set; }
            public int Bateria { get; set; }
            public bool Conectando { get; set; }
            public bool Atualizando { get; set; }
            public bool Encerrar { get; set; }
            public bool Conectado { get; set; }
            public bool Comunicacao { get; set; }
            public bool PermitirChatbot { get; set; }
            public ConfigMKAUTH MKAUTH { get; set; }
            public MainModels.Whatsapp Whatsapp { get; set; }
            public List<Chat> Chats { get; set; }
            public ChatbotBruto Chatbot { get; set; }
            public List<TicketBruto> Tickets { get; set; }
            public List<PreReferencia> PreReferencias { get; set; }
        }

        public class Chat
        {

            public string ID { get; set; }
            public int NaoLidas { get; set; }
            public string NomeContato { get; set; }
            public string NumeroContato { get; set; }
            public string ImagemContato { get; set; }
            public string UltimaMensagem { get; set; }
            public List<Mensagem> Mensagens { get; set; }
            public DateTimeOffset UltimaMensagemHorario { get; set; }
        }

        public class Mensagem
        {
            public string ID { get; set; }
            public string Chat { get; set; }
            public string Quoted { get; set; }
            public string Remetente { get; set; }
            public string NomeRemetente { get; set; }
            public string NomeAtendente { get; set; }
            public string ImagemRemetente { get; set; }
            public string B64Media { get; set; }
            public string mimetype { get; set; }
            public string lat { get; set; }
            public string lng { get; set; }
            public string mediaKey { get; set; }
            public string deprecatedMms3Url { get; set; }
            public string caption { get; set; }
            public string Destinatario { get; set; }
            public string Texto { get; set; }
            public string Tipo { get; set; }
            public bool FromMe { get; set; }
            public DateTimeOffset Horario { get; set; }
        }

        public class Respostas
        {

            public class message
            {

                    public string id { get; set; }
                    public string body { get; set; }
                public string type { get; set; }
                public string caption { get; set; }
                public int t { get; set; }
                public string notifyName { get; set; }
                public string mediaKey { get; set; }
                public string mimetype { get; set; }
                public string lat { get; set; }
                public string lng { get; set; }
                public string deprecatedMms3Url { get; set; }
                public string from { get; set; }
                    public string to { get; set; }
                    public string self { get; set; }
                    public int ack { get; set; }
                    public bool invis { get; set; }
                    public bool star { get; set; }
                    public bool isFromTemplate { get; set; }
                    public string thumbnail { get; set; }
                    public int richPreviewType { get; set; }
                    public Quotedmsg quotedMsg { get; set; }
                    public string quotedStanzaID { get; set; }
                    public string quotedParticipant { get; set; }
                    public object[] mentionedJidList { get; set; }
                    public bool isVcardOverMmsDocument { get; set; }
                    public object[] labels { get; set; }
                    public bool productHeaderImageRejected { get; set; }
                    public bool isDynamicReplyButtonsMsg { get; set; }
                    public bool isMdHistoryMsg { get; set; }
                    public bool? requiresDirectConnection { get; set; }
                    public string chatId { get; set; }
                    public bool fromMe { get; set; }
                    public Sender sender { get; set; }
                    public int timestamp { get; set; }
                    public string content { get; set; }
                    public bool isGroupMsg { get; set; }
                    public bool isMedia { get; set; }
                    public bool isNotification { get; set; }
                    public bool isPSA { get; set; }
                    public Chat chat { get; set; }
                    public Quotedmsgobj quotedMsgObj { get; set; }
                    public Mediadata1 mediaData { get; set; }

                public class Quotedmsg
                {
                    public string type { get; set; }
                    public string body { get; set; }
                }

                public class Sender
                {
                    public string id { get; set; }
                    public string type { get; set; }
                    public string formattedName { get; set; }
                    public bool isMe { get; set; }
                    public bool isMyContact { get; set; }
                    public bool isPSA { get; set; }
                    public bool isUser { get; set; }
                    public bool isWAContact { get; set; }
                    public Profilepicthumbobj profilePicThumbObj { get; set; }
                    public object msgs { get; set; }
                }

                public class Profilepicthumbobj
                {
                    public string eurl { get; set; }
                    public string id { get; set; }
                    public string img { get; set; }
                    public string imgFull { get; set; }
                    public object raw { get; set; }
                    public string tag { get; set; }
                }

                public class Chat
                {
                    public string id { get; set; }
                    public bool pendingMsgs { get; set; }
                    public Lastreceivedkey lastReceivedKey { get; set; }
                    public int t { get; set; }
                    public int unreadCount { get; set; }
                    public bool archive { get; set; }
                    public bool isReadOnly { get; set; }
                    public int modifyTag { get; set; }
                    public int muteExpiration { get; set; }
                    public string name { get; set; }
                    public bool notSpam { get; set; }
                    public Int64 pin { get; set; }
                    public int ephemeralDuration { get; set; }
                    public int ephemeralSettingTimestamp { get; set; }
                    public bool hasUnreadMention { get; set; }
                    public bool archiveAtMentionViewedInDrawer { get; set; }
                    public bool hasChatBeenOpened { get; set; }
                    public object msgs { get; set; }
                    public string kind { get; set; }
                    public bool isGroup { get; set; }
                    public string formattedTitle { get; set; }
                    public Contact contact { get; set; }
                    public object groupMetadata { get; set; }
                    public Presence presence { get; set; }
                }

                public class Lastreceivedkey
                {
                    public bool fromMe { get; set; }
                    public string remote { get; set; }
                    public string id { get; set; }
                    public string _serialized { get; set; }
                }

                public class Contact
                {
                    public string id { get; set; }
                    public string name { get; set; }
                    public string type { get; set; }
                    public string verifiedName { get; set; }
                    public bool isBusiness { get; set; }
                    public bool isEnterprise { get; set; }
                    public int verifiedLevel { get; set; }
                    public bool statusMute { get; set; }
                    public object[] labels { get; set; }
                    public int disappearingModeDuration { get; set; }
                    public int disappearingModeSettingTimestamp { get; set; }
                    public string formattedName { get; set; }
                    public bool isMe { get; set; }
                    public bool isMyContact { get; set; }
                    public bool isPSA { get; set; }
                    public bool isUser { get; set; }
                    public bool isWAContact { get; set; }
                    public Profilepicthumbobj1 profilePicThumbObj { get; set; }
                    public object msgs { get; set; }
                }

                public class Profilepicthumbobj1
                {
                }

                public class Presence
                {
                    public string id { get; set; }
                    public object[] chatstates { get; set; }
                }

                public class Quotedmsgobj
                {
                    public string id { get; set; }
                    public string body { get; set; }
                    public string type { get; set; }
                    public string from { get; set; }
                    public string to { get; set; }
                    public string author { get; set; }
                    public string self { get; set; }
                    public bool star { get; set; }
                    public bool isFromTemplate { get; set; }
                    public object[] mentionedJidList { get; set; }
                    public bool isVcardOverMmsDocument { get; set; }
                    public bool isForwarded { get; set; }
                    public bool productHeaderImageRejected { get; set; }
                    public bool isDynamicReplyButtonsMsg { get; set; }
                    public bool isMdHistoryMsg { get; set; }
                    public object? requiresDirectConnection { get; set; }
                    public string chatId { get; set; }
                    public bool fromMe { get; set; }
                    public Sender1 sender { get; set; }
                    public string content { get; set; }
                    public bool isGroupMsg { get; set; }
                    public bool isMedia { get; set; }
                    public bool isNotification { get; set; }
                    public bool isPSA { get; set; }
                    public Chat1 chat { get; set; }
                    public object quotedMsgObj { get; set; }
                    public Mediadata mediaData { get; set; }
                }

                public class Sender1
                {
                    public string id { get; set; }
                    public string name { get; set; }
                    public string type { get; set; }
                    public string verifiedName { get; set; }
                    public bool isBusiness { get; set; }
                    public bool isEnterprise { get; set; }
                    public int verifiedLevel { get; set; }
                    public bool statusMute { get; set; }
                    public object[] labels { get; set; }
                    public int disappearingModeDuration { get; set; }
                    public int disappearingModeSettingTimestamp { get; set; }
                    public string formattedName { get; set; }
                    public bool isMe { get; set; }
                    public bool isMyContact { get; set; }
                    public bool isPSA { get; set; }
                    public bool isUser { get; set; }
                    public bool isWAContact { get; set; }
                    public Profilepicthumbobj2 profilePicThumbObj { get; set; }
                    public object msgs { get; set; }
                }

                public class Profilepicthumbobj2
                {
                }

                public class Chat1
                {
                    public string id { get; set; }
                    public bool pendingMsgs { get; set; }
                    public Lastreceivedkey1 lastReceivedKey { get; set; }
                    public int t { get; set; }
                    public int unreadCount { get; set; }
                    public bool archive { get; set; }
                    public bool isReadOnly { get; set; }
                    public int modifyTag { get; set; }
                    public int muteExpiration { get; set; }
                    public string name { get; set; }
                    public bool notSpam { get; set; }
                    public Int64 pin { get; set; }
                    public int ephemeralDuration { get; set; }
                    public int ephemeralSettingTimestamp { get; set; }
                    public bool hasUnreadMention { get; set; }
                    public bool archiveAtMentionViewedInDrawer { get; set; }
                    public bool hasChatBeenOpened { get; set; }
                    public object msgs { get; set; }
                    public string kind { get; set; }
                    public bool isGroup { get; set; }
                    public string formattedTitle { get; set; }
                    public Contact1 contact { get; set; }
                    public object groupMetadata { get; set; }
                    public Presence1 presence { get; set; }
                }

                public class Lastreceivedkey1
                {
                    public bool fromMe { get; set; }
                    public string remote { get; set; }
                    public string id { get; set; }
                    public string _serialized { get; set; }
                }

                public class Contact1
                {
                    public string id { get; set; }
                    public string name { get; set; }
                    public string type { get; set; }
                    public string verifiedName { get; set; }
                    public bool isBusiness { get; set; }
                    public bool isEnterprise { get; set; }
                    public int verifiedLevel { get; set; }
                    public bool statusMute { get; set; }
                    public object[] labels { get; set; }
                    public int disappearingModeDuration { get; set; }
                    public int disappearingModeSettingTimestamp { get; set; }
                    public string formattedName { get; set; }
                    public bool isMe { get; set; }
                    public bool isMyContact { get; set; }
                    public bool isPSA { get; set; }
                    public bool isUser { get; set; }
                    public bool isWAContact { get; set; }
                    public Profilepicthumbobj3 profilePicThumbObj { get; set; }
                    public object msgs { get; set; }
                }

                public class Profilepicthumbobj3
                {
                }

                public class Presence1
                {
                    public string id { get; set; }
                    public object[] chatstates { get; set; }
                }

                public class Mediadata
                {
                }

                public class Mediadata1
                {
                }

            }
            public class chat
            {
                public string id { get; set; }
                public string unreadCount { get; set; }
                public string formattedTitle { get; set; }
                public Contact contact { get; set; }
                public LastReceivedKey lastReceivedKey { get; set; }

                public class LastReceivedKey
                {
                    public string _serialized { get; set; }
                }
                public class Contact
                {
                    public string id { get; set; }
                    public string formattedName { get; set; }
                    public string pushname { get; set; }
                    public string verifiedName { get; set; }

                }
            }
            public class me
            {
                public string pushname { get; set; }
                public string locales { get; set; }
                public string platform { get; set; }
                public int battery { get; set; }
                public string wid { get; set; }
                public PHONE phone { get; set; }

                public class PHONE
                {
                    public string device_manufacturer { get; set; }
                    public string device_model { get; set; }
                }
            }
        }
    }
}
