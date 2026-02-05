namespace Dietcode.Core.Domain.Rules
{
    public class ValidatorRules
    {
        private List<string> errors = [];
        public ValidatorRules() { }

        public List<string> Errors => errors;

        /// <summary>
        /// Indica se o resultado é válido (sem erros).
        /// </summary>
        public bool Valid => errors.Count == 0;

        /// <summary>
        /// Indica se o resultado é inválido (possui erros).
        /// </summary>
        public bool Invalid => errors.Count != 0;

        // ------------------------
        // Métodos Retornos de Erros e ou Mensagens Aglutinadas
        // ------------------------
        public string RenderizeErrosAsHtml()
            => RenderListAsHtml(Errors);

        public string RenderizeErrosAsText()
            => RenderListAsText(Errors);

        // ------------------------
        // Métodos privados auxiliares
        // ------------------------

        private static string RenderListAsHtml(IEnumerable<string> mensagens)
        {
            return string.Concat(mensagens.Select(msg => $"- {msg}<br/>"));
        }

        private static string RenderListAsText(IEnumerable<string> mensagens)
        {
            return string.Concat(mensagens.Select(msg => $"- {msg}{Environment.NewLine}"));
        }

    }
}
