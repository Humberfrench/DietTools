# Dietcode.Classic.DomainValidator

Versão legada do `Dietcode.Core.DomainValidator` para projetos .NET Framework 4.8. Representa resultados de validação de domínio, agregando erros, mensagens informativas, código de status HTTP e metadados auxiliares de forma padronizada entre camadas de domínio, aplicação e API.

## Instalação

```bash
dotnet add package Dietcode.Classic.DomainValidator --version 4.8.1
```

## Funcionalidades

- `ValidationResultBase`: classe base abstrata com a lista de erros, o `HttpStatusCode` do resultado, mensagens informativas e as flags `Valid`/`Invalid`.
- `ValidationResult<T>`: resultado de validação com um objeto de retorno fortemente tipado (`Retorno`).
- `ValidationResult`: variação não genérica (`ValidationResult<object>`), com o método `TryReturnAs<T>()` para tentar recuperar o retorno como um tipo específico.
- `Converter<U>()`: converte um `ValidationResult<T>` em `ValidationResult<U>`, copiando os erros e convertendo o `Retorno` via serialização JSON (`System.Text.Json`, referenciado explicitamente no pacote por não fazer parte do net48).
- `ValidationError`: erro de validação com `Codigo` e `Message`.
- `MensagemData` (namespace `ObjectValue`): mensagem informativa com `Codigo` e `Mensagem` — não é um erro.
- `Entries` (namespace `ObjectValue`): par `EntryName`/`EntryKeyValue`, útil para registrar IDs criados ou atualizados em operações de persistência.
- `ResponseText`: mensagens padrão reutilizáveis (`PreenchimentoObrigatorio`, `AcessoNegado`, `ErroValidacao`, `ErroRequisicao`, `ErroRequisicaoNaoConfere`, `ErroSemDados`, `ErroInvalidoOuInexistente`, `ServiceNotFound`, `ServiceUnavailable`).
- Renderização de erros e mensagens como texto ou HTML.
- `IValidationResult` (namespace `Interfaces`): contrato disponível no pacote, embora nenhuma das classes atuais o implemente.
- Propriedades `Mensagem` e `CodigoMensagem` em `ValidationResultBase`, mantidas apenas por compatibilidade e marcadas com `[Obsolete(..., error: true)]` — usar qualquer uma delas impede a compilação. Utilize `Mensagens`/`AddMensagem` no lugar.

## Uso básico

```csharp
using Dietcode.Classic.DomainValidator;

public ValidationResult<UsuarioDto> Cadastrar(UsuarioDto usuario)
{
    var result = new ValidationResult<UsuarioDto>();

    if (string.IsNullOrWhiteSpace(usuario.Nome))
        result.AddError("Nome é obrigatório.");

    if (string.IsNullOrWhiteSpace(usuario.Email))
        result.AddError("E-mail é obrigatório.", codigo: 101);

    if (result.Invalid)
        return result;

    result.Retorno = usuario;
    return result;
}
```

`AddError` também aceita um `HttpStatusCode`, atribuindo-o diretamente ao `StatusCode` do resultado:

```csharp
result.AddError("Usuário não encontrado.", HttpStatusCode.NotFound);
```

## Agregando erros de outro resultado

```csharp
ValidationResultBase resultadoInterno = ValidarDadosBancarios(conta);

var resultado = new ValidationResult<ContaDto>();
resultado.Add(resultadoInterno); // copia os erros de resultadoInterno
```

## Mensagens informativas e renderização

Mensagens não são erros — servem para comunicar informações adicionais ao chamador.

```csharp
result.AddMensagem("Cadastro realizado com sucesso.");
result.AddMensagem(100, "Processamento assíncrono iniciado.");

if (result.TemMensagens)
{
    string html = result.RenderizeMensagensAsHtml();
    string texto = result.RenderizeMensagensAsText();
}
```

Os mesmos métodos existem para os erros: `RenderizeErrosAsHtml()` e `RenderizeErrosAsText()`.

## Convertendo entre tipos de retorno

```csharp
ValidationResult<UsuarioEntity> resultadoEntidade = ValidarEntidade(entity);

// Copia os erros e converte o Retorno via JSON
ValidationResult<UsuarioDto> resultadoDto = resultadoEntidade.Converter<UsuarioDto>();
```

## Pacotes relacionados

- `Dietcode.Core.DomainValidator`: versão atual para projetos .NET modernos (net10.0), com a mesma API. A versão Core já removeu por completo as propriedades obsoletas `Mensagem` e `CodigoMensagem` que a versão Classic ainda mantém, desativadas, para sinalizar a migração.

## Licença

MIT
