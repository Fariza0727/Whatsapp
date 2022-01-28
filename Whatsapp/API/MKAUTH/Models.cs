using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Whatsapp.API.MKAUTH
{
    public class Models
    {
        public class Caixa
        {
            public class RetornoListagem
            {
                public Caixa[] caixa { get; set; }

                public class Caixa
                {
                    public string uuid { get; set; }
                    public string historico { get; set; }
                }

            }
            public class RetornoShow
            {
                public string id { get; set; }
                public string uuid_caixa { get; set; }
                public string usuario { get; set; }
                public string data { get; set; }
                public string historico { get; set; }
                public string complemento { get; set; }
                public string entrada { get; set; }
                public string saida { get; set; }
                public string tipomov { get; set; }
                public string planodecontas { get; set; }

            }
        }
        public class Chamado
        {
            public class RetornoListagem
            {
                public Chamado[] chamados { get; set; }

                public class Chamado
                {
                    public string uuid { get; set; }
                    public string id { get; set; }
                    public string abertura { get; set; }
                    public string login { get; set; }
                    public string prioridade { get; set; }
                    public string chamado { get; set; }
                }

            }
            public class RetornoShow
            {
                public string id { get; set; }
                public string uuid_suporte { get; set; }
                public string assunto { get; set; }
                public string abertura { get; set; }
                public object fechamento { get; set; }
                public object email { get; set; }
                public string status { get; set; }
                public string chamado { get; set; }
                public string nome { get; set; }
                public string login { get; set; }
                public string atendente { get; set; }
                public string visita { get; set; }
                public string prioridade { get; set; }
                public string ramal { get; set; }
                public string reply { get; set; }
                public string tecnico { get; set; }
                public string login_atend { get; set; }
                public object motivo_fechar { get; set; }
            }
        }
        public class Cliente
        {
            public class RetornoListagem
            {
                public Cliente[] clientes { get; set; }

                public class Cliente
                {
                    public string uuid { get; set; }
                    public string id { get; set; }
                    public string codigo { get; set; }
                    public string nome { get; set; }
                    public string nome_res { get; set; }
                    public string login { get; set; }
                    public string cpf_cnpj { get; set; }
                    public string tipo { get; set; }
                    public string coordenadas { get; set; }
                    public string senha { get; set; }
                    public string email { get; set; }
                    public object ip { get; set; }
                    public string mac { get; set; }
                    public string ramal { get; set; }
                    public string endereco { get; set; }
                    public string numero { get; set; }
                    public string bairro { get; set; }
                    public string complemento { get; set; }
                    public string cidade { get; set; }
                    public string estado { get; set; }
                    public string cep { get; set; }
                }
            }

            public class RetornoShow
            {
                public Dado[] dados { get; set; }
                public Wifi wifi { get; set; }

                public class Wifi
                {
                    public string sinal { get; set; }
                    public string rate { get; set; }
                }

                public class Dado
                {
                    public string id { get; set; }
                    public string nome { get; set; }
                    public string email { get; set; }
                    public string endereco { get; set; }
                    public object bairro { get; set; }
                    public object cidade { get; set; }
                    public object cep { get; set; }
                    public string estado { get; set; }
                    public string cpf_cnpj { get; set; }
                    public object fone { get; set; }
                    public object obs { get; set; }
                    public object nascimento { get; set; }
                    public string estado_civil { get; set; }
                    public string cadastro { get; set; }
                    public string login { get; set; }
                    public string tipo { get; set; }
                    public string night { get; set; }
                    public string aviso { get; set; }
                    public string foto { get; set; }
                    public string venc { get; set; }
                    public object mac { get; set; }
                    public object complemento { get; set; }
                    public object ip { get; set; }
                    public string ramal { get; set; }
                    public string rg { get; set; }
                    public string isento { get; set; }
                    public object celular { get; set; }
                    public string bloqueado { get; set; }
                    public string autoip { get; set; }
                    public string automac { get; set; }
                    public string conta { get; set; }
                    public object ipvsix { get; set; }
                    public string plano { get; set; }
                    public string send { get; set; }
                    public string cli_ativado { get; set; }
                    public string simultaneo { get; set; }
                    public string turbo { get; set; }
                    public string comodato { get; set; }
                    public string observacao { get; set; }
                    public string chavetipo { get; set; }
                    public object chave { get; set; }
                    public string contrato { get; set; }
                    public string ssid { get; set; }
                    public string senha { get; set; }
                    public object numero { get; set; }
                    public object responsavel { get; set; }
                    public object nome_pai { get; set; }
                    public object nome_mae { get; set; }
                    public object expedicao_rg { get; set; }
                    public object naturalidade { get; set; }
                    public string acessacen { get; set; }
                    public string pessoa { get; set; }
                    public string endereco_res { get; set; }
                    public object numero_res { get; set; }
                    public string bairro_res { get; set; }
                    public object cidade_res { get; set; }
                    public object cep_res { get; set; }
                    public string estado_res { get; set; }
                    public object complemento_res { get; set; }
                    public string desconto { get; set; }
                    public string acrescimo { get; set; }
                    public string equipamento { get; set; }
                    public string vendedor { get; set; }
                    public object nextel { get; set; }
                    public string accesslist { get; set; }
                    public string resumo { get; set; }
                    public string grupo { get; set; }
                    public string codigo { get; set; }
                    public string prilanc { get; set; }
                    public string tipobloq { get; set; }
                    public string adesao { get; set; }
                    public string mbdisco { get; set; }
                    public string sms { get; set; }
                    public string ltrafego { get; set; }
                    public string planodown { get; set; }
                    public string ligoudown { get; set; }
                    public string statusdown { get; set; }
                    public string statusturbo { get; set; }
                    public string opcelular { get; set; }
                    public string nome_res { get; set; }
                    public string coordenadas { get; set; }
                    public string rem_obs { get; set; }
                    public string valor_sva { get; set; }
                    public string dias_corte { get; set; }
                    public string user_ip { get; set; }
                    public string user_mac { get; set; }
                    public string data_ip { get; set; }
                    public string data_mac { get; set; }
                    public string last_update { get; set; }
                    public object data_bloq { get; set; }
                    public object tags { get; set; }
                    public string tecnico { get; set; }
                    public object data_ins { get; set; }
                    public string altsenha { get; set; }
                    public string geranfe { get; set; }
                    public string mesref { get; set; }
                    public object ipfall { get; set; }
                    public string tit_abertos { get; set; }
                    public string parc_abertas { get; set; }
                    public string tipo_pessoa { get; set; }
                    public object mac_serial { get; set; }
                    public string status_corte { get; set; }
                    public string plano15 { get; set; }
                    public object celular2 { get; set; }
                    public string pgaviso { get; set; }
                    public object porta_olt { get; set; }
                    public object caixa_herm { get; set; }
                    public object porta_splitter { get; set; }
                    public object onu_ont { get; set; }
                    public object _switch { get; set; }
                    public string tit_vencidos { get; set; }
                    public object _interface { get; set; }
                    public string login_atend { get; set; }
                    public object cidade_ibge { get; set; }
                    public object estado_ibge { get; set; }
                    public string data_desbloq { get; set; }
                    public string pool_name { get; set; }
                    public string pgcorte { get; set; }
                    public string rec_email { get; set; }
                    public object dot_ref { get; set; }
                    public string conta_cartao { get; set; }
                    public string termo { get; set; }
                    public string opcelular2 { get; set; }
                    public object armario_olt { get; set; }
                    public string plano_bloqc { get; set; }
                    public string tipo_cliente { get; set; }
                    public string uuid_cliente { get; set; }
                    public object data_desativacao { get; set; }
                    public string tipo_cob { get; set; }
                    public string fortunus { get; set; }
                    public string gsici { get; set; }
                    public string local_dici { get; set; }
                }
            }
        }
        public class Conta
        {
            public class RetornoListagem
            {
                public Conta[] contas { get; set; }

                public class Conta
                {
                    public string id { get; set; }
                    public string uuid { get; set; }
                    public string banco { get; set; }
                    public object agencia { get; set; }
                    public object ag_digito { get; set; }
                    public object conta { get; set; }
                    public object ct_digito { get; set; }
                }

            }
            public class RetornoShow
            {
                public string id { get; set; }
                public string uuid_boleto { get; set; }
                public string utilizar { get; set; }
                public string banco { get; set; }
                public object codigo_cedente { get; set; }
                public object agencia { get; set; }
                public object ag_digito { get; set; }
                public object conta { get; set; }
                public object ct_digito { get; set; }
                public object carteira { get; set; }
                public object convenio { get; set; }
                public object cedente { get; set; }
                public object contrato { get; set; }
                public string obs_linha1 { get; set; }
                public string obs_linha2 { get; set; }
                public string obs_linha3 { get; set; }
                public string obs_linha4 { get; set; }
                public string instr_linha1 { get; set; }
                public object instr_linha2 { get; set; }
                public object instr_linha3 { get; set; }
                public object instr_linha4 { get; set; }
                public object instr_linha5 { get; set; }
                public object taxa { get; set; }
                public object nosso { get; set; }
                public string multa { get; set; }
                public string juros { get; set; }
                public object codigo_cliente { get; set; }
                public object ponto_venda { get; set; }
                public object nosso1 { get; set; }
                public object nosso2 { get; set; }
                public object nosso3 { get; set; }
                public object constante1 { get; set; }
                public object constante2 { get; set; }
                public object _byte { get; set; }
                public string token { get; set; }
                public string gateway { get; set; }
                public object pagseguro { get; set; }
                public object pagdigital { get; set; }
                public string modalidade { get; set; }
                public object paypalconta { get; set; }
                public string diasatraso { get; set; }
                public string layout { get; set; }
                public string logosva { get; set; }
                public string instauto { get; set; }
                public object nossonumfinal { get; set; }
                public object tipo { get; set; }
                public string variacao { get; set; }
                public string localpag { get; set; }
                public string titulo_inicial { get; set; }
                public string avalista { get; set; }
                public string desconto { get; set; }
                public string tipodesc { get; set; }
                public string cpf_cnpj { get; set; }
                public string esp_doc { get; set; }
                public string token_gnet { get; set; }
                public string nome { get; set; }
                public string varalfa { get; set; }
                public string calc_boleto { get; set; }
                public object token_bcash { get; set; }
                public object token_pagseguro { get; set; }
                public string last_update { get; set; }
                public string referencia { get; set; }
                public object transmissao { get; set; }
                public string layout_pdf { get; set; }
                public string complemento { get; set; }
                public string contra { get; set; }
                public string cnab { get; set; }
                public string num_remessa { get; set; }
                public string id_conta_gnet { get; set; }
                public string cliente_id_gnet { get; set; }
                public string cliente_secret_gnet { get; set; }
                public string ocorrencia { get; set; }
                public object codbanco { get; set; }
                public string tipo_desc { get; set; }
                public string cliente_id_teste_gnet { get; set; }
                public string cliente_secret_teste_gnet { get; set; }
                public object transmissao240 { get; set; }
                public object transmissao400 { get; set; }
                public object url_gnet { get; set; }
                public string email_gnet { get; set; }
                public object token_bfacil { get; set; }
                public string email_bfacil { get; set; }
                public object token_juno { get; set; }
                public object token_sjuno { get; set; }
                public string email_juno { get; set; }
                public string dias_baixa { get; set; }
            }
        }
        public class Contato
        {
            public class RetornoListagem
            {
            }
            public class RetornoShow
            {

            }
        }
        public class Instalacao
        {
            public class RetornoListagem
            {
                public Instalaco[] instalacoes { get; set; }

                public class Instalaco
                {
                    public string id { get; set; }
                    public string uuid { get; set; }
                    public string login { get; set; }
                    public string senha { get; set; }
                    public string email { get; set; }
                    public string nome { get; set; }
                }

            }
            public class RetornoShow
            {
                public string id { get; set; }
                public string uuid_solic { get; set; }
                public string login { get; set; }
                public string senha { get; set; }
                public string email { get; set; }
                public string nome { get; set; }
                public object data_nasc { get; set; }
                public string cpf { get; set; }
                public string endereco { get; set; }
                public string bairro { get; set; }
                public string cidade { get; set; }
                public string estado { get; set; }
                public string cep { get; set; }
                public string telefone { get; set; }
                public string vencimento { get; set; }
                public string plano { get; set; }
                public string complemento { get; set; }
                public object rg { get; set; }
                public string celular { get; set; }
                public string comodato { get; set; }
                public string datainst { get; set; }
                public string visitado { get; set; }
                public string instalado { get; set; }
                public string tecnico { get; set; }
                public string obs { get; set; }
                public string tipo { get; set; }
                public object ip { get; set; }
                public object mac { get; set; }
                public string valor { get; set; }
                public string concluido { get; set; }
                public string promocod { get; set; }
                public string numero { get; set; }
                public string endereco_res { get; set; }
                public string numero_res { get; set; }
                public string bairro_res { get; set; }
                public string cidade_res { get; set; }
                public string cep_res { get; set; }
                public string estado_res { get; set; }
                public string complemento_res { get; set; }
                public string vendedor { get; set; }
                public object nextel { get; set; }
                public string disp { get; set; }
                public string contrato { get; set; }
                public string adesao { get; set; }
                public string visita { get; set; }
                public string equipamento { get; set; }
                public string codigo { get; set; }
                public object ipcadastro { get; set; }
                public object processamento { get; set; }
                public string status { get; set; }
                public string opcelular { get; set; }
                public object coordenadas { get; set; }
                public string login_atend { get; set; }
                public object ramal { get; set; }
                public string termo { get; set; }
                public string opcelular2 { get; set; }
                public object celular2 { get; set; }
                public object naturalidade { get; set; }
                public object dot_ref { get; set; }
            }
        }
        public class Plano
        {
            public class RetornoListagem
            {
                public Plano[] planos { get; set; }

                public class Plano
                {
                    public string uuid { get; set; }
                    public string nome { get; set; }
                    public string valor { get; set; }
                    public string velup { get; set; }
                    public string veldown { get; set; }
                    public string prioridade { get; set; }
                    public string descricao { get; set; }
                }

            }
            public class RetornoShow
            {
                public string nome { get; set; }
                public string uuid_plano { get; set; }
                public string valor { get; set; }
                public string velup { get; set; }
                public string veldown { get; set; }
                public string garup { get; set; }
                public string gardown { get; set; }
                public object tempoup { get; set; }
                public object tempodown { get; set; }
                public string prioridade { get; set; }
                public string maxup { get; set; }
                public string maxdown { get; set; }
                public string desaup { get; set; }
                public string desadown { get; set; }
                public object burst { get; set; }
                public object descricao { get; set; }
                public string oculto { get; set; }
                public string valor_scm { get; set; }
                public string valor_sva { get; set; }
                public string pool { get; set; }
                public string valor_desc { get; set; }
                public string list { get; set; }
                public string aliquota { get; set; }
                public string cfop_plano { get; set; }
                public string desc_titulo { get; set; }
                public string perc_ibpt { get; set; }
                public string tipo { get; set; }
                public string ipv6a { get; set; }
                public string ipv6b { get; set; }
                public string vpm { get; set; }
                public string faixa { get; set; }
                public string tecnologia { get; set; }
                public string pis_pasep { get; set; }
                public string cofins { get; set; }
                public string perc_ibpt_m { get; set; }
                public string perc_ibpt_e { get; set; }
                public string perc_ibpt_f { get; set; }
            }
        }
        public class Titulo
        {
            public class RetornoListagem
            {
                public Titulo[] titulos { get; set; }
                public class Titulo
                {
                    public string uuid { get; set; }
                    public string titulo { get; set; }
                    public string valor { get; set; }
                    public string valorpag { get; set; }
                    public string datavenc { get; set; }
                    public string nossonum { get; set; }
                    public string linhadig { get; set; }
                    public string nome { get; set; }
                    public string login { get; set; }
                    public string cpf_cnpj { get; set; }
                    public string tipo { get; set; }
                    public string email { get; set; }
                    public string endereco { get; set; }
                    public string numero { get; set; }
                    public string bairro { get; set; }
                    public string complemento { get; set; }
                    public string cidade { get; set; }
                    public string estado { get; set; }
                    public string cep { get; set; }
                    public string status { get; set; }
                    public string uuid_lanc { get; set; }
                }

            }
            public class RetornoShow
            {
                public string id { get; set; }
                public string datavenc { get; set; }
                public string nossonum { get; set; }
                public string datapag { get; set; }
                public object nome { get; set; }
                public string recibo { get; set; }
                public string status { get; set; }
                public string login { get; set; }
                public string tipo { get; set; }
                public string cfop_lanc { get; set; }
                public string obs { get; set; }
                public string processamento { get; set; }
                public string aviso { get; set; }
                public object url { get; set; }
                public string usergerou { get; set; }
                public string valorger { get; set; }
                public string coletor { get; set; }
                public object linhadig { get; set; }
                public string valor { get; set; }
                public string valorpag { get; set; }
                public string gwt_numero { get; set; }
                public string imp { get; set; }
                public string referencia { get; set; }
                public string tipocob { get; set; }
                public object codigo_carne { get; set; }
                public object chave_gnet { get; set; }
                public object chave_gnet2 { get; set; }
                public object chave_juno { get; set; }
                public object numconta { get; set; }
                public string gerourem { get; set; }
                public string remvalor { get; set; }
                public string remdata { get; set; }
                public string formapag { get; set; }
                public string percmulta { get; set; }
                public string valormulta { get; set; }
                public string percmora { get; set; }
                public string valormora { get; set; }
                public string percdesc { get; set; }
                public string valordesc { get; set; }
                public object fcartaobandeira { get; set; }
                public object fcartaonumero { get; set; }
                public object fchequenumero { get; set; }
                public object fchequebanco { get; set; }
                public object fchequeagcc { get; set; }
                public string deltitulo { get; set; }
                public object datadel { get; set; }
                public string num_recibos { get; set; }
                public string num_retornos { get; set; }
                public string alt_venc { get; set; }
                public string uuid_lanc { get; set; }
                public string tarifa_paga { get; set; }
                public string id_empresa { get; set; }
                public string oco01 { get; set; }
                public string oco02 { get; set; }
                public string oco06 { get; set; }
            }
        }
        public class Usuario
        {
            public class RetornoListagem
            {
                public Usuario[] usuarios { get; set; }

                public class Usuario
                {
                    public string uuid { get; set; }
                    public string login { get; set; }
                    public string email { get; set; }
                }

            }
            public class RetornoShow
            {
                    public string uuid_acesso { get; set; }
                    public string nome { get; set; }
                    public string login { get; set; }
                    public string email { get; set; }
                    public string avatar { get; set; }
                    public string ultacesso { get; set; }
                    public string nivel { get; set; }
            }
        }
    }
}
