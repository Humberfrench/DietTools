namespace Dietcode.Core.DomainValidator.ObjectValue
{
    public class MensagemData
    {
        public int Codigo { get; set; }
        public string Mensagem { get; set; } = string.Empty;

        public MensagemData()
        {
            Codigo = 0;
            Mensagem = string.Empty;
        }

        public MensagemData(int codigo, string mensagem)
        {
            Codigo = codigo;
            Mensagem = mensagem;
        }

        public MensagemData(string mensagem)
        {
            Codigo = 0;
            Mensagem = mensagem;
        }
    }

}
