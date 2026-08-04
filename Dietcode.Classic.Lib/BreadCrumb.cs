namespace Dietcode.Classic.Lib
{
    [Serializable]
    public class BreadCrumb
    {
        public string LinkText { get; set; } = string.Empty;

        public string ActionName { get; set; } = string.Empty;

        public string ControllerName { get; set; } = string.Empty;

        public bool Voltar { get; set; }

        public string VoltarUrl { get; set; } = string.Empty;

        public string Link { get; set; } = string.Empty;

        public bool LinkRoot { get; set; }
    }
}
