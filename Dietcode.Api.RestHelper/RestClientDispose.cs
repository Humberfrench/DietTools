using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Api.RestHelper
{
    public class RestClientDispose : RestClient, IDisposable
    {
        private bool isDisposed;
        private IntPtr nativeResource = Marshal.AllocHGlobal(100);

        ~RestClientDispose() => Dispose(false);

        public RestClientDispose(string enderecoBaseApi)
            : base(enderecoBaseApi)
        {
            isDisposed = false;
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected override void Dispose(bool disposing)
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

    }
}
