using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Api.Core.Results
{
    public class MethodResult<TContent> : MethodResult
    {
        public TContent Content { get; set; }

        public MethodResult(TContent content, ResultStatusCode status)
            : base(status)
        {
            Content = content;
        }
    }
}
