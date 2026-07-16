using System;
using System.Collections.Generic;

namespace Dietcode.Classic.Lib
{
    [Serializable]
        public class Aviso
        {
            public List<string> Mensagens { get; set; }

            public Tipo TipoMensagem { get; set; }

            public string TituloMensagem { get; set; }

            public string Mensagem
            {
                get
                {
                    return String.Join(", ", Mensagens);
                }

            }

            public List<string> FieldAlerts { get; set; }

            public enum Tipo
            {
                Atencao, Sucesso, Informacao, Erro
            }

            public Aviso()
            {
                FieldAlerts = new List<string>();
                Mensagens = new List<string>();
            }

            public Aviso(string mensagem)
                : this(mensagem, Tipo.Erro)
            {

            }

            public Aviso(string mensagem, Tipo tipoMensagem)
                : this()
            {
                TipoMensagem = tipoMensagem;
                Mensagens.Add(mensagem);
            }
        }
 
}