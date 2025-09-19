using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Core.Lib.Rest
{
    public class ApiResponse<T> where T : class, new()
    {
        public ApiResponse()
        {
            Data = new T();
        }
        public bool Success { get; set; }
        public string RawContent { get; set; } = string.Empty;
        public string Erro { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; }
        public T Data { get; set; }
    }
}
