namespace Dietcode.Core.DomainValidator
{
    public class ResponseText
    {
        public const string PreenchimentoObrigatorio = "O campo {campo} é de preenchimento obrigatorio.";
        public const string AcessoNegado = "Acesso negado.";
        public const string ErroValidacao = "Erro de validação.";
        public const string ErroRequisicao = "Erro de requisição.";
        public const string ErroRequisicaoNaoConfere = "Erro de requisição: O corpo da requisição não confere com o esperado.";
        public const string ErroSemDados = "Não há dados para serem exibidos.";
        public const string ErroInvalidoOuInexistente = "{valor} inválido ou inexistente.";

        public const string ServiceNotFound = "Serviço não encontrado ou não implementado.";
        public const string ServiceUnavailable = "Serviço indisponível.";
    }
}
