namespace Dietcode.Core.Lib
{
    public static class ApiCodeBasic
    {
        public static (string ClientId, string ClientSecret) Obter(string codigoApi)
        {
            (string ClientId, string ClientSecret) retorno = (ClientId: string.Empty, ClientSecret: string.Empty);

            var stringLogin = codigoApi.FromBase64ToString();
            var array = stringLogin.Split(':');

            retorno.ClientId = array[0];
            retorno.ClientSecret = array[1];

            return retorno;
        }

    }
}
