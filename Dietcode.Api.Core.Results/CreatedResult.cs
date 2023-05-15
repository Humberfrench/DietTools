using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Api.Core.Results
{
    public class CreatedResult : MethodResult
    {
        public CreatedResult()
            : base(ResultStatusCode.OK)
        {
        }

    }
}
