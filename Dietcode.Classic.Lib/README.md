# Dietcode.Classic.Lib

Biblioteca de utilitários para projetos **.NET Framework 4.8**, reunindo extensões, validadores de documentos brasileiros, criptografia, mascaramento de dados, paginação, análise de força de senha, helper REST e componentes auxiliares para aplicações ASP.NET MVC clássico.

Este pacote é a versão *Classic* de `Dietcode.Core.Lib`. Os dois compartilham boa parte da API (extensões de string/data, validação de CPF/CNPJ, mascaramento, senhas, paginação e helper REST), mas não são idênticos: `Dietcode.Classic.Lib` usa `Newtonsoft.Json` (em vez de `System.Text.Json`), não possui suporte a `DateOnly` (inexistente no .NET Framework) nem ao módulo de localização (`Langs`), e em contrapartida traz recursos próprios para aplicações .NET Framework clássicas, como `AppSettings` (baseado em `ConfigurationManager`), `AsIsBundleOrderer` (bundling do ASP.NET MVC) e modelos de view como `Aviso`, `BreadCrumb` e `BreadCrumbETitulo`.

## Instalação

```bash
dotnet add package Dietcode.Classic.Lib --version 4.8.1
```

## Funcionalidades

- Extensões de string, número, data, enum e JSON (`Newtonsoft.Json`).
- Validação e formatação de CPF e CNPJ, com validadores adicionais de RG, CNH e Renavam.
- Validação de cartão de crédito (algoritmo de Luhn) e identificação de bandeira.
- Mascaramento de número de cartão e de dados sensíveis em objetos/JSON.
- Validação de telefone fixo e celular no padrão brasileiro.
- Análise de força de senha.
- Paginação de coleções em memória e de `IQueryable`.
- Helper REST simples (`HttpService`) com retorno padronizado (`ApiResult<TResponse>`).
- Criptografia AES (ECB) compatível com .NET Framework, incluindo versão legada obsoleta.
- Geração de strings/senhas/números aleatórios com RNG criptográfico.
- Cálculo financeiro simples (juros, valor líquido, valor total).
- Leitura tipada de `AppSettings`/`ConnectionStrings` via `ConfigurationManager`.
- Componentes auxiliares para views ASP.NET MVC (`Aviso`, `BreadCrumb`, `BreadCrumbETitulo`, `AsIsBundleOrderer`).

## Extensões de string

```csharp
using Dietcode.Classic.Lib;

var primeiroNome = "Maria Silva".GetFirstName();
var primeiroEUltimo = "Maria Aparecida Silva".GetFirstAndLastName();
var temValor = "texto".HasValue();
var somenteNumeros = "(11) 99999-9999".OnlyNumbers();
var semAcentos = "Ação".RemoveAccents();
var email = "maria@exemplo.com".IsValidEmail();
```

Também há conversores de caixa (`ToSnakeCase()`, `ToCamelCase()`, `ToKebabCase()`, incluindo variantes em `ReadOnlySpan<char>`), formatação de moeda (`ToMoeda()`), texto Sim/Não (`ToSimNao()`) e formatação de telefone (`ToPhoneFormated()`).

## Datas

```csharp
using Dietcode.Classic.Lib;

var data = DateTime.Now;

var dataFormatada = data.ToDateFormatted();          // dd/MM/yyyy
var dataHora = data.ToDateTimeFormatted();            // dd/MM/yyyy HH:mm
var proximoDiaUtil = data.ProximoDiaUtil();
var diaUtil = data.IsDiaUtil();
```

`IsDiaUtil()`/`IsDiaNaoUtil()` consideram sábados, domingos e uma lista fixa de feriados nacionais brasileiros (`IsFeriadoFixo()`).

## Documentos (CPF, CNPJ, RG, CNH, Renavam)

```csharp
using Dietcode.Classic.Lib;

var cpfFormatado = "12345678909".ToCpf();
var cnpjFormatado = "12345678000195".ToCnpj();
var documento = "12345678909".FormatoCpfouCnpj();

var rgValido = "123456789".IsValidRg();
var cnhValida = "12345678900".IsValidCnh();
var renavamValido = "12345678901".IsValidRenavam();
```

A classe estática `Documento` oferece as mesmas operações sem extension methods (`Documento.TratarDocumento`, `Documento.TratarDocumentoCpf`, `Documento.TratarDocumentoCnpj`, `Documento.SemFormatacao`), além das versões otimizadas com `ReadOnlySpan<char>` (`ToCpfSpan()`, `ToCnpjSpan()`).

## Validação (`Validacao`)

```csharp
using Dietcode.Classic.Lib;

var cpfValido = Validacao.IsCpf("12345678909");
var cnpjValido = Validacao.IsCnpj("12345678000195");
var pisValido = Validacao.IsPis("12345678901");

var documentoCorrigido = Validacao.CorrigirDocumento("123456789"); // completa com zeros à esquerda

var rangeValido = Validacao.DataRangeValido(DateTime.Now.AddYears(-30)); // "true" ou mensagem de erro
```

`IsCpf`, `IsCnpj` e `IsPis` calculam os dígitos verificadores reais (não apenas o tamanho da string). `DataRangeValido` e `DataDocumentoRangeValido` retornam `"true"` ou uma mensagem de erro em português, pensadas para uso direto em validações de formulário.

## Cartão de crédito

```csharp
using Dietcode.Classic.Lib;

var valido = CreditCardValidator.IsValidCreditCardNumber("4111111111111111");
var bandeira = CreditCardValidator.ValidaBandeira("4111111111111111"); // "VISA"
var bin = CreditCardValidator.ObterBinUnico("4111111111111111");       // 6 primeiros + 4 últimos dígitos

var mascarado = Formatacao.MascararCartaoDeCredito("4111111111111111"); // "411111******1111"
```

`ValidaBandeira` reconhece Visa, Mastercard, Elo, Amex, Hipercard, Diners, JCB e Discover a partir das faixas de BIN. `Formatacao` também expõe `ObterBinInicio`/`ObterBinFim` para obter apenas o início ou o fim do número mascarado.

## Telefone

```csharp
using Dietcode.Classic.Lib;

var fixo = PhoneValidator.IsValidPhoneNumber("1123456789");   // 10 dígitos: DDD + número
var celular = PhoneValidator.IsValidCellNumber("11912345678"); // 11 dígitos: DDD + 9 + número

if (!celular.Valid)
{
    var mensagem = celular.Message;
}
```

Ambos os métodos retornam `PhoneValidatorData` (`Valid`, `Message`).

## Senhas

```csharp
using Dietcode.Classic.Lib.Passwords;

var resultado = "Senha@123".AsSpan().AnalyzePassword();

if (resultado.MeetsMinimumRules)
{
    // Senha atende as regras mínimas (8+ caracteres, maiúscula, minúscula, dígito e símbolo).
}
```

`PasswordStrengthResult` traz `Length`, `HasUppercase`, `HasLowercase`, `HasDigit`, `HasSymbol`, `Entropy` (em bits) e `Level` (`PasswordStrengthLevel`: `Invalid`, `VeryWeak`, `Weak`, `Medium`, `Strong`, `VeryStrong`). Espaços em branco e caracteres fora do intervalo ASCII invalidam a senha (`Level = Invalid`).

## Mascaramento de dados sensíveis

```csharp
using Dietcode.Classic.Lib.Masking;

var mascarado = SensitiveDataMasker.Mask(new
{
    Email = "user@exemplo.com",
    Password = "123456",
    Token = "abc"
});
```

`SensitiveDataMasker.Mask` aceita objetos ou strings JSON e substitui por `"***"` os campos `password`, `senha`, `token`, `accessToken`, `refreshToken`, `authorization` e `apiKey` (comparação sem diferenciar maiúsculas/minúsculas), inclusive em objetos e arrays aninhados.

## Paginação

```csharp
using Dietcode.Classic.Lib.Pagging;

var parametro = new PageParameter { PageNumber = 1, PageSize = 20 };

PagedCollection<Produto> pagina = produtos.ToPaged(parametro);

var total = pagina.TotalItems;
var totalPaginas = pagina.TotalPages;
var temProxima = pagina.HasNext;
```

`ToPaged` está disponível tanto para `IEnumerable<T>` quanto para `IQueryable<T>` (útil com Entity Framework, pois `Skip`/`Take`/`Count` viram SQL). `PageParameter` limita `PageSize` entre 1 e `MaxPageSize` (500) e garante `PageNumber` mínimo de 1.

## JSON

```csharp
using Dietcode.Classic.Lib;

var json = new { Id = 1, Nome = "Maria" }.ToJson();
var objeto = json.ToObject<MinhaClasse>();

var jObject = json.ToJObject();
var jToken = json.ToJToken();

var outro = objeto.ConvertObjects<OutraClasse>();
```

Internamente usa `Newtonsoft.Json` com configuração padrão (`NullValueHandling.Ignore`, `ReferenceLoopHandling.Ignore`, profundidade máxima de 128), podendo ser sobrescrita via overloads que aceitam `JsonSerializerSettings`.

## REST

A pasta `Rest` oferece um helper estático para chamadas HTTP simples, com a mesma API do helper equivalente em `Dietcode.Core.Lib`, porém usando `Newtonsoft.Json` para serialização.

```csharp
using Dietcode.Classic.Lib.Rest;

ApiResult<UserResponse> result = await HttpService.Get<UserResponse>(
    url: "https://api.exemplo.com/users/1",
    enumApiRest: EnumApiRest.Bearer,
    token: accessToken);

if (result.IsSuccess)
{
    var user = result.Data;
}
```

`HttpService` também expõe `Post`, `Put`, `Patch` e `Delete` (com e sem corpo de requisição). `EnumApiRest` define o tipo de autenticação (`None`, `Basic`, `Bearer`, `XApiKey`). `ApiResult<TResponse>` exige `TResponse : class, new()` e traz `Data`, `StatusCode`, `IsSuccess`, `Content` (corpo bruto), `ContentType`, `ContentLength` e `Error`.

## Criptografia

```csharp
using Dietcode.Classic.Lib.Cryptography;

var criptografado = AES.Encrypt("texto secreto", "minha-chave");
var original = AES.Decrypt(criptografado, "minha-chave");
```

`AES` usa `Aes` em modo ECB com padding PKCS7; a chave é normalizada para 16 bytes (truncada ou preenchida com espaços). Existe também `Dietcode.Classic.Lib.Cryptography.V1.AES`, marcada `[Obsolete]`, mantida apenas para compatibilidade com dados já criptografados por versões antigas.

## Utilidades diversas

```csharp
using Dietcode.Classic.Lib;

var senhaAleatoria = Util.GerarSenhaAleatoria(12);
var numeroUnico = Util.GerarNumeroUnico(6);
var referenceId = Util.GerarReferenceId(8, "PED");

var nomeMes = Month.Name(3);   // "Março"
var mesAbrev = Month.Short(3); // "Mar"

var jurosCalculados = new Calculos().CalculaJuros(taxa: 2.5, parcelas: 12, valoroperacao: 1000, decimais: 0, tarifa: 0);
```

`ObjectExtensions` oferece `OrThisValue()` (retorna um valor alternativo/nova instância quando nulo) e `OrEmpty()` para `List<T>`, `Dictionary<TKey, TValue>`, arrays, `IEnumerable<T>`, `IReadOnlyList<T>` e `ICollection<T>`. `GetDescription()` (extensão de `Enum`) lê o atributo `[Description]`; `GetCode()` retorna o nome do valor em `UPPER_SNAKE_CASE`. `EnumTipoBoleto` e `EnumTipoCodigo` são enums prontos, decorados com `[Description]`, para uso em integrações de cobrança.

`AppSettings` lê valores tipados de `web.config`/`app.config` (`Get`, `GetUri`, `GetGuid`, `GetBoolean`, `GetInt`, `GetLong`, `GetByte`, `GetConnectionString`), usando `System.Configuration.ConfigurationManager`.

## Componentes para ASP.NET MVC clássico

- `Aviso`: modelo de mensagem de alerta (`TipoMensagem`: Atencao, Sucesso, Informacao, Erro) com lista de mensagens e de alertas de campo.
- `BreadCrumb` / `BreadCrumbETitulo`: modelos simples para trilha de navegação (breadcrumb) em views.
- `AsIsBundleOrderer`: implementação de `IBundleOrderer` (`System.Web.Optimization`) que preserva a ordem original dos arquivos de um bundle.

## Pacotes relacionados

- `Dietcode.Core.Lib`: versão para .NET moderno (com `System.Text.Json`, suporte a `DateOnly` e módulo de localização), com API equivalente na maior parte dos recursos.

## Licença

MIT
