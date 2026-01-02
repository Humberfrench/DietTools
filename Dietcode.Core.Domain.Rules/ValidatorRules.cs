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

    }
}
