# Dietcode.Core.Domain.Rules

Biblioteca para composição de regras de validação de domínio em aplicações .NET modernas (net10.0), baseada no padrão *Specification*. Permite descrever regras de negócio reutilizáveis, fortemente tipadas e expressas por meio de expressões lambda sobre as propriedades da entidade.

## Instalação

```bash
dotnet add package Dietcode.Core.Domain.Rules --version 10.2.0
```

## Funcionalidades

- `ISpecification<T>`: contrato de uma especificação de negócio (`IsSatisfiedBy`).
- `IRule<TEntity>`: contrato de uma regra nomeada, com `MensagemErro` e `Validar`.
- `IValidator<TEntity>`: contrato de um validador que retorna um `ValidatorRules`.
- `Rule<TEntity>`: implementação de `IRule<TEntity>` que encapsula uma `ISpecification<TEntity>` e a mensagem de erro associada.
- `Validator<TEntity>`: classe abstrata base (padrão *Strategy*) para compor validadores a partir de regras nomeadas — `AdicionarRegra`, `RemoverRegra`, `ObterRegra` e `Validar`.
- `ValidatorRules`: resultado da validação, com a lista de `Errors`, as flags `Valid`/`Invalid` e renderização em texto ou HTML.
- Especificações prontas para uso, todas parametrizadas por expressão lambda:
  - `PropriedadeStringPreenchida<T>`: string não nula, vazia ou composta só por espaços.
  - `PropriedadeIntMaiorQueZero<T>` / `PropriedadeIntMaiorIgualZero<T>`.
  - `PropriedadeDecimalMaiorQueZero<T>` / `PropriedadeDecimalMaiorIgualZero<T>`.
  - `PropriedadeEmailValido<T>`: valida o formato do e-mail via expressão regular.
  - `PropriedadeNumeroStringValido<T>`: string contendo apenas dígitos.
  - `PropriedadeMesValido<T>`: mês entre 1 e 12.
  - `PropriedadeDiaValido<T>`: dia válido para o mês informado (não considera anos bissextos).
  - `RequisitoMinimoPreenchido<T>`: satisfeita quando pelo menos uma dentre várias propriedades string está preenchida.

## Criando um validador

```csharp
using Dietcode.Core.Domain.Rules;
using Dietcode.Core.Domain.Rules.Specifications;

public class UsuarioValidator : Validator<Usuario>
{
    public UsuarioValidator()
    {
        AdicionarRegra("NomeObrigatorio",
            new Rule<Usuario>(new PropriedadeStringPreenchida<Usuario>(u => u.Nome),
                "Nome é obrigatório."));

        AdicionarRegra("EmailValido",
            new Rule<Usuario>(new PropriedadeEmailValido<Usuario>(u => u.Email),
                "E-mail inválido."));

        AdicionarRegra("IdadeValida",
            new Rule<Usuario>(new PropriedadeIntMaiorQueZero<Usuario>(u => u.Idade),
                "Idade deve ser maior que zero."));
    }
}
```

## Executando a validação

```csharp
var validator = new UsuarioValidator();
ValidatorRules resultado = validator.Validar(usuario);

if (resultado.Invalid)
{
    string erros = resultado.RenderizeErrosAsText();
    // ou resultado.RenderizeErrosAsHtml();
}
```

## Combinando especificações

`RequisitoMinimoPreenchido<T>` é útil para exigir que ao menos um entre vários campos opcionais esteja preenchido:

```csharp
AdicionarRegra("TelefoneOuEmail",
    new Rule<Usuario>(
        new RequisitoMinimoPreenchido<Usuario>(u => u.Telefone, u => u.Email),
        "Informe ao menos um contato: telefone ou e-mail."));
```

## Pacotes relacionados

- `Dietcode.Classic.Domain.Rules`: versão legada para projetos .NET Framework 4.8, com a mesma API (interfaces, `Rule<TEntity>`, `Validator<TEntity>` e especificações idênticas).

## Licença

MIT
