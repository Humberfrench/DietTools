namespace Dietcode.Core.DomainValidator
{
    [Serializable]
    public class ValidationError
    {
        public ValidationError()
        {
            Codigo = 0;
            Message = "";
            Erro = false;
        }
        public ValidationError(string message)
        {
            Codigo = 0;
            Message = message;
            Erro = true;
        }

        public ValidationError(string message, bool erro)
        {
            Codigo = 0;
            Message = message;
            Erro = erro;
        }
        public ValidationError(int codigo, string message, bool erro)
        {
            Codigo = codigo;
            Message = message;
            Erro = erro;
        }
        public int Codigo { get; set; }

        public string Message { get; set; }

        public bool Erro { get; set; }
    }
}

