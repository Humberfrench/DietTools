# Dietcode.Core.Cep

Biblioteca para consulta de endereço a partir de um CEP, com o provedor
plugável via injeção de dependência.

Hoje o único provedor é o [ViaCEP](https://viacep.com.br/) (v1). Quem
consome a lib depende apenas da abstração `ICepProvider` — trocar (ou
adicionar) um provedor no futuro (ex.: uma v2 usando outra API, como a
"BrasilAPI") não muda a assinatura de `ICepProvider` nem o código de quem já
usa a biblioteca, só o registro feito no `Program.cs`.

## Instalação

```bash
dotnet add package Dietcode.Core.Cep --version 1.0.0
```

## Configuração

`Program.cs`, com os valores padrão do ViaCEP:

```csharp
using Dietcode.Core.Cep.Extensions;

builder.Services.AddDietcodeViaCep();
```

Ou personalizando `BaseUrl`/`TimeoutSeconds`:

```csharp
using Dietcode.Core.Cep.Extensions;

builder.Services.AddDietcodeViaCep(options =>
{
    options.BaseUrl = "https://viacep.com.br/ws/";
    options.TimeoutSeconds = 15;
});
```

`AddDietcodeViaCep` registra `ICepProvider` como um typed `HttpClient`
(`IHttpClientFactory`), implementado por `ViaCepProvider`.

## Uso

O provedor é informado no construtor da classe que consome a busca de CEP —
quem recebe `ICepProvider` não sabe (nem precisa saber) qual provedor está
por trás:

```csharp
using Dietcode.Core.Cep.Abstractions;

public sealed class EnderecoService
{
    private readonly ICepProvider _cepProvider;

    public EnderecoService(ICepProvider cepProvider)
    {
        _cepProvider = cepProvider;
    }

    public async Task<string> DescreverEnderecoAsync(string cep, CancellationToken cancellationToken)
    {
        var resultado = await _cepProvider.GetAddressAsync(cep, cancellationToken);

        if (!resultado.IsSuccess)
            return $"Erro: {resultado.Error}";

        var endereco = resultado.Address!;
        return $"{endereco.Logradouro}, {endereco.Bairro} - {endereco.Localidade}/{endereco.Uf}";
    }
}
```

## Resultado

`GetAddressAsync` nunca lança exceção para CEP inválido ou não encontrado —
sempre retorna `CepLookupResult`:

- `IsSuccess`: indica se o CEP foi encontrado.
- `Address`: dados do endereço (`CepAddress`) quando `IsSuccess` é `true`.
- `Error`: mensagem de erro quando `IsSuccess` é `false` (CEP inválido, CEP
  não encontrado ou falha de comunicação com o provedor).

`CepAddress` traz `Cep`, `Logradouro`, `Complemento`, `Bairro`, `Localidade`,
`Uf`, `Ibge`, `Gia`, `Ddd` e `Siafi`.

## Preparado para novos provedores (v2)

Adicionar um novo provedor (ex.: BrasilAPI) é só criar uma nova implementação
de `ICepProvider` e um novo método de registro, sem tocar em `ICepProvider`,
`CepAddress`, `CepLookupResult` ou em quem já consome a lib:

```csharp
public sealed class BrasilApiCepProvider : ICepProvider
{
    public Task<CepLookupResult> GetAddressAsync(string cep, CancellationToken cancellationToken = default)
    {
        // implementação para a BrasilAPI
    }
}
```

```csharp
public static IServiceCollection AddDietcodeBrasilApiCep(this IServiceCollection services)
{
    services.AddHttpClient<ICepProvider, BrasilApiCepProvider>(/* ... */);
    return services;
}
```

Basta chamar `AddDietcodeBrasilApiCep()` em vez de `AddDietcodeViaCep()` no
`Program.cs` — nenhum outro código muda.

## Licença

MIT
