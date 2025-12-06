namespace Dietcode.Core.DomainValidator
{
    /// <summary>
    /// Classe que centraliza mensagens de resposta utilizadas em validações e retornos de serviços.
    /// </summary>
    public class ResponseText
    {
        /// <summary>
        /// Mensagem para campos obrigatórios não preenchidos.
        /// </summary>
        public const string PreenchimentoObrigatorio = "O campo {campo} é de preenchimento obrigatorio.";

        /// <summary>
        /// Mensagem padrão para acesso negado.
        /// </summary>
        public const string AcessoNegado = "Acesso negado.";

        /// <summary>
        /// Mensagem genérica para erro de validação.
        /// </summary>
        public const string ErroValidacao = "Erro de validação.";

        /// <summary>
        /// Mensagem genérica para erro de requisição.
        /// </summary>
        public const string ErroRequisicao = "Erro de requisição.";

        /// <summary>
        /// Mensagem para requisições cujo corpo não corresponde ao esperado.
        /// </summary>
        public const string ErroRequisicaoNaoConfere = "Erro de requisição: O corpo da requisição não confere com o esperado.";

        /// <summary>
        /// Mensagem usada quando não há dados disponíveis para exibir.
        /// </summary>
        public const string ErroSemDados = "Não há dados para serem exibidos.";

        /// <summary>
        /// Mensagem para valores inválidos ou inexistentes.
        /// </summary>
        public const string ErroInvalidoOuInexistente = "{valor} inválido ou inexistente.";

        /// <summary>
        /// Mensagem usada quando um serviço não é encontrado ou não foi implementado.
        /// </summary>
        public const string ServiceNotFound = "Serviço não encontrado ou não implementado.";

        /// <summary>
        /// Mensagem usada quando um serviço está indisponível.
        /// </summary>
        public const string ServiceUnavailable = "Serviço indisponível.";
    }
}
