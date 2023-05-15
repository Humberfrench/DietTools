using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Api.Core.Results
{
    public class OkResult<TContent> : MethodResult<TContent>
    {
        public OkResult(TContent content)
            : base(content, ResultStatusCode.OK)
        {
        }
    }
}
