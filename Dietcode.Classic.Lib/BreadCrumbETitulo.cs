using System;
using System.Collections.Generic;

namespace Dietcode.Classic.Lib
{
    [Serializable]
    public class BreadCrumbETitulo
    {
        public string Titulo { get; set; }

        public List<BreadCrumb> BreadCrumbs { get; set; }
    }

}
