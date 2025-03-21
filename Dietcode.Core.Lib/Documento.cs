namespace Dietcode.Core.Lib
{
    public static class Documento
    {
        public static string SemFormatacao(string documento)
        {
            return documento.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);
        }

        public static string TratarDocumento(string documento)
        {
            documento = SemFormatacao(documento);
            if (documento.Length == 11)
            {
                return TratarDocumentoCpf(documento);
            }

            return TratarDocumentoCnpj(documento);
        }

        public static string TratarDocumentoCpf(string documento)
        {
            return Convert.ToUInt64(documento).ToString(@"000\.000\.000\-00");
        }

        public static string TratarDocumentoCnpj(string documento)
        {
            return Convert.ToUInt64(documento).ToString(@"00\.000\.000\/0000\-00");
        }
    }
}
