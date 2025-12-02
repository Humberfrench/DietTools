namespace Dietcode.Core.DomainValidator
{
    [Serializable]
    public class ValidationError
    {
        public int Codigo { get; set; }
        public string Message { get; set; }

        public ValidationError()
        {
            Codigo = 0;
            Message = string.Empty;
        }

        public ValidationError(string message)
        {
            Codigo = 0;
            Message = message;
        }

        public ValidationError(int codigo, string message)
        {
            Codigo = codigo;
            Message = message;
        }
    }
}

