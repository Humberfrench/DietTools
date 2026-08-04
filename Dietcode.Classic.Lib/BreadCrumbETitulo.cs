namespace Dietcode.Classic.Lib
{
    [Serializable]
    public class BreadCrumbETitulo
    {
        public string Titulo { get; set; } = string.Empty;

        public List<BreadCrumb> BreadCrumbs { get; set; } = [];
    }

}
