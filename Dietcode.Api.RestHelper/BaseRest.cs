using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;

namespace Dietcode.Api.RestHelper
{
    public abstract class BaseRest : IDisposable
    {
        private bool isDisposed;
        private IntPtr nativeResource = Marshal.AllocHGlobal(100);

        public string Endereco { get; private set; }

        ~BaseRest() => Dispose(false);

        protected BaseRest(string endereco = "")
        {

            Endereco = endereco;
            Erro = "";
        }

        protected JsonSerializerSettings MicrosoftDateFormatSettings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Local,
            FloatFormatHandling = FloatFormatHandling.DefaultValue,
            FloatParseHandling = FloatParseHandling.Decimal,
            Culture = CultureInfo.CurrentCulture,
            MaxDepth = 8,

        };

        #region Rest 

        protected RestClientDispose CriaRestClient()
        {
            var client = new RestClientDispose(Endereco);

            //if (Acesso != null)
            //{
            //    client.AddDefaultHeader("Authorization", "Bearer " + Acesso.AccessToken);
            //}
            //client.AddDefaultHeader("merchant_id", Config.MerchantId);
            //client.AddDefaultHeader("merchant_key", Config.MerchantKey);

            return client;
        }

        protected string Erro { get; set; }

        protected T? ApiMethod<T>(string uri, object? model, Method method, out HttpStatusCode responseStatus, out string content)
        {
            using (var restClient = CriaRestClient())
            {
                var request = new RestRequest(uri, method)
                {
                    RequestFormat = DataFormat.Json,
                    Timeout = -1
                };

                if (method == Method.Post || method == Method.Put)
                {
                    if (model != null)
                    {
                        request.AddJsonBody(Json.Serialize(model));
                    }
                }
                var task = restClient.ExecuteAsync(request);

                var response = task.Result;

                //var response = await Task.Run(() => restClient.ExecuteAsync(request));

                responseStatus = response.StatusCode;
                content = response.Content ?? "";

                var retorno = (T?)Activator.CreateInstance(typeof(T));

                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
                {
                    return retorno;
                }

                try
                {
                    if (response.Content != null)
                    {
                        retorno = JsonConvert.DeserializeObject<T>(response.Content);
                    }
                }
                catch (Exception ex)
                {
                    Erro = ex.Message;
                    return retorno;
                }

                return retorno;
            }
        }

        protected async Task<T?> ApiMethodAsync<T>(string uri, object? model, Method method)
        {
            using (var restClient = CriaRestClient())
            {
                var request = new RestRequest(uri, method)
                {
                    RequestFormat = DataFormat.Json,
                    Timeout = -1,
                };
                if (method == Method.Post || method == Method.Put)
                {
                    if (model != null)
                    {
                        request.AddJsonBody(JsonConvert.SerializeObject(model, Formatting.None, MicrosoftDateFormatSettings);
                    }
                }

                //Bug version RestSharp
                var response = await restClient.ExecuteAsync(request);

                if (response.StatusCode == 0) // erro da maldição do rest_sharp
                {
                    throw new TimeoutException("Erro na Api. Time out");
                }
                var retorno = (T?)Activator.CreateInstance(typeof(T));

                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
                {
                    return retorno;
                }

                try
                {
                    if (response.Content != null)
                    {
                        retorno = JsonConvert.DeserializeObject<T>(response.Content);
                    }
                }
                catch (Exception ex)
                {
                    Erro = ex.Message;
                    return retorno;
                }


                return retorno;
            }
        }

        protected string ObterJson(object objeto)
        {

            var resultado = JsonConvert.SerializeObject(objeto, Formatting.Indented);

            return resultado;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                // free managed resources

            }

            // free native resources if there are any.
            if (nativeResource != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(nativeResource);
                nativeResource = IntPtr.Zero;
            }

            isDisposed = true;
        }

        #endregion
    }
}