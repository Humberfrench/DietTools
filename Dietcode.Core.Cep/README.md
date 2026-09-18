# Dietcode.Core.Cep

Biblioteca para consulta de endereço a partir de um CEP, com o provedor
plugável via injeção de dependência.

Hoje existem dois provedores — [ViaCEP](https://viacep.com.br/) e
[BrasilAPI](https://brasilapi.com.br/docs#tag/CEP) —, com failover automático
entre eles. Quem consome a lib depende apenas da abstração `ICepProvider` e do
`CepLookupResult`, que é **o mesmo objeto de retorno independente do provedor
escolhido**: trocar o provedor primário, ou adicionar um terceiro no futuro,
não muda a assinatura de `ICepProvider` nem o código de quem já usa a
biblioteca, só o registro feito no `Program.cs`.

## Instalação

```bash
dotnet add package Dietcode.Core.Cep --version 1.0.0
```

## Configuração

### Um único provedor, sem failover

```csharp
using Dietcode.Core.Cep.Extensions;

builder.Services.AddDietcodeViaCep();
// ou
builder.Services.AddDietcodeBrasilApi();
```

Ou personalizando `BaseUrl`/`TimeoutSeconds`:

```csharp
builder.Services.AddDietcodeViaCep(options =>
{
    options.BaseUrl = "https://viacep.com.br/ws/";
    options.TimeoutSeconds = 15;
});
```

### Os dois provedores, com failover automático (recomendado)

```csharp
using Dietcode.Core.Cep;
using Dietcode.Core.Cep.Extensions;

builder.Services.AddDietcodeCep(CepProviderPrimario.ViaCep);
```

`AddDietcodeCep` registra o ViaCEP e a BrasilAPI e expõe `ICepProvider` como
um `ContingencyCepProvider`: a consulta é feita no provedor primário
(`CepProviderPrimario.ViaCep` ou `CepProviderPrimario.BrasilApi`); se ele
estiver indisponível (HTTP de erro, timeout, falha de rede), o outro é
consultado automaticamente, sem que quem chamou `GetAddressAsync` precise
saber disso. Também aceita personalizar as opções de cada provedor:

```csharp
builder.Services.AddDietcodeCep(
    CepProviderPrimario.BrasilApi,
    configureViaCep: options => options.TimeoutSeconds = 10,
    configureBrasilApi: options => options.TimeoutSeconds = 10);
```

Um "CEP não encontrado" (CEP válido no formato, mas inexistente) **não**
aciona o failover — só uma falha real do serviço primário aciona.

## Uso

O provedor é informado no construtor da classe que consome a busca de CEP —
quem recebe `ICepProvider` não sabe (nem precisa saber) qual provedor está
por trás, nem se houve failover:

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

`GetAddressAsync` nunca lança exceção para CEP inválido, CEP não encontrado ou
falha de serviço (quando há failover configurado) — sempre retorna
`CepLookupResult`:

- `IsSuccess`: indica se o CEP foi encontrado.
- `Address`: dados do endereço (`CepAddress`) quando `IsSuccess` é `true`.
- `Error`: mensagem de erro quando `IsSuccess` é `false` (CEP inválido, CEP
  não encontrado ou falha de comunicação com o provedor).
- `Provider`: nome do provedor que efetivamente respondeu a consulta
  (`"ViaCEP"` ou `"BrasilAPI"`) — o mesmo tanto em sucesso quanto em falha.
- `Contingencia`: `true` quando o provedor primário falhou (por falha de
  serviço, não por "CEP não encontrado") e o secundário respondeu no lugar
  dele; `false` no caso normal.

`CepAddress` traz `Cep`, `Logradouro`, `Complemento`, `Bairro`, `Localidade`,
`Uf`, `Ibge`, `Gia`, `Ddd` e `Siafi` — sempre com esses mesmos campos,
independente do provedor. A BrasilAPI não retorna `Complemento`/`Ddd`/`Gia`/
`Siafi`, então esses campos ficam vazios quando ela responde.

## Provedores disponíveis

| Provedor | Endpoint | Observação |
|---|---|---|
| ViaCEP (`ViaCepProvider`) | `GET https://viacep.com.br/ws/{cep}/json/` | CEP inexistente responde HTTP 200 com `{"erro": true}` (às vezes `"erro": "true"`, como string — a lib trata os dois casos). |
| BrasilAPI (`BrasilApiProvider`) | `GET https://brasilapi.com.br/api/cep/v1/{cep}` | CEP inexistente responde HTTP 404. |

## Preparado para novos provedores (v3, v4...)

Adicionar um novo provedor é só criar uma nova implementação de
`ICepProvider` que:

- devolve `CepLookupResult.Failure(...)` para CEP inválido ou não encontrado
  (resposta de negócio, não deve acionar failover);
- lança `CepProviderUnavailableException` (ou deixa propagar
  `HttpRequestException`/`TaskCanceledException`) quando o serviço em si
  falhar (o que aciona o failover, se `ContingencyCepProvider` estiver em
  uso).

```csharp
public sealed class OutroCepProvider : ICepProvider
{
    public Task<CepLookupResult> GetAddressAsync(string cep, CancellationToken cancellationToken = default)
    {
        // implementação do novo provedor
    }
}
```

Nenhum código em `ICepProvider`, `CepAddress`, `CepLookupResult` ou em quem já
consome a lib precisa mudar.

## Licença

MIT
