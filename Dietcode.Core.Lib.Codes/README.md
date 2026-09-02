# Dietcode.Core.Lib.Codes

Biblioteca de geração de QR Code para .NET 10. Combina um motor de codificação de QR Code completo (portado do projeto [QRCoder](https://github.com/codebude/QRCoder), MIT) com uma camada de renderização própria — sem dependência de `System.Drawing`, portanto multiplataforma — e uma API simplificada (`QrEncoder`) para os casos de uso mais comuns.

Não é uma biblioteca de Pix ou boleto bancário: não existe, no código atual, nenhum gerador de payload específico para os padrões de pagamento brasileiros. O que ela oferece são mais de vinte geradores de payload prontos no estilo QRCoder (Wi-Fi, vCard, e-mail, SMS, criptomoedas, pagamentos bancários europeus, etc.) descritos abaixo, além do motor genérico de codificação de texto em QR Code.

## Instalação

Este projeto não gera pacote NuGet (`GeneratePackageOnBuild` não está habilitado no `.csproj`). Ele é consumido via referência de projeto dentro da solução:

```bash
dotnet add reference ../Dietcode.Core.Lib.Codes/Dietcode.Core.Lib.Codes.csproj
```

## Funcionalidades

- Motor de codificação de QR Code (`QRCodeGenerator`) suportando versões 1–40 e Micro QR, detecção automática de modo (Numérico, Alfanumérico, Byte/UTF‑8) e níveis de correção de erro L/M/Q/H.
- API simplificada e recomendada (`QrEncoder`) para codificar texto em `QRCodeData`, PNG (`byte[]`) ou PNG em Base64, sem precisar lidar com os detalhes do motor.
- Renderizadores próprios da Dietcode (não fazem parte do QRCoder original): `AbstractQRCode` (classe base), `PngByteQRCode` (gera PNG manualmente, sem `System.Drawing`) e `Base64QRCode` (PNG codificado em Base64, com cores customizáveis).
- `QRCodeData`: representa a matriz de módulos do QR Code, com serialização/desserialização em formato binário compacto próprio (`.qrr`), com compressão opcional (`Deflate` ou `GZip`).
- Mais de vinte geradores de payload prontos (`PayloadGenerator`), portados do QRCoder — ver lista completa abaixo.
- `DataTooLongException`: lançada quando o conteúdo não cabe em nenhuma versão do QR Code para o nível de correção de erro escolhido.

## Gerando um QR Code (API simplificada)

```csharp
using Dietcode.Core.Lib.Codes;

using var encoder = new QrEncoder();

// PNG em Base64, pronto para embutir em HTML/JSON.
string base64Png = encoder.EncodeToBase64Png(
    text: "https://www.dietcode.com.br",
    level: QrErrorCorrectionLevel.M,
    pixelsPerModule: 10);

// PNG em memória.
byte[] pngBytes = encoder.EncodeToPngBytes("https://www.dietcode.com.br");

// Apenas os dados/matriz do QR Code, para renderizar separadamente.
QRCodeData dados = encoder.Encode("texto qualquer", QrErrorCorrectionLevel.H, forceUtf8: true);
```

`QrErrorCorrectionLevel` (`L`, `M`, `Q`, `H`) é o enum simplificado usado por `QrEncoder`. Internamente ele é convertido para o `ECCLevel` do motor original.

## Renderizando a partir de `QRCodeData`

```csharp
using Dietcode.Core.Lib.Codes;

using var png = new PngByteQRCode(dados);
byte[] bytesPng = png.GetGraphic(pixelsPerModule: 8, drawQuietZones: true);

using var base64 = new Base64QRCode(dados);
string base64Customizado = base64.GetGraphic(
    pixelsPerModule: 8,
    darkColorHex: "#0B5FFF",
    lightColorHex: "#FFFFFF");
```

## Persistindo os dados do QR Code

```csharp
using Dietcode.Core.Lib.Codes;

dados.SaveRawData("codigo.qrr", QRCodeData.Compression.GZip);

var carregado = new QRCodeData("codigo.qrr", QRCodeData.Compression.GZip);
```

## Usando os geradores de payload (`PayloadGenerator`)

Cada tipo de payload monta a string no formato esperado por leitores de QR Code (ex.: `WIFI:T:WPA2;S:...`, `mailto:...`, `BEGIN:VCARD...`). Para gerar o QR Code, use `payload.ToString()` como entrada de `QrEncoder.Encode(...)` ou de `QRCodeGenerator.CreateQrCode(payload, ...)`:

```csharp
using Dietcode.Core.Lib.Codes;
using static Dietcode.Core.Lib.Codes.QRCodeGenerator;

var wifiPayload = new PayloadGenerator.WiFi(
    ssid: "MinhaRede",
    password: "minhaSenha123",
    authenticationMode: PayloadGenerator.WiFi.Authentication.WPA2);

using var generator = new QRCodeGenerator();
QRCodeData qrData = generator.CreateQrCode(wifiPayload, ECCLevel.M);
```

Ou, de forma mais simples, com `QrEncoder`:

```csharp
using var encoder = new QrEncoder();
var mail = new PayloadGenerator.Mail("contato@dietcode.com.br", subject: "Assunto", message: "Mensagem");

string base64Png = encoder.EncodeToBase64Png(mail.ToString());
```

### Payloads disponíveis

- `Url` — link/URL (adiciona `http://` automaticamente se o protocolo não for informado).
- `Mail` — e-mail, nos formatos `MAILTO`, `MATMSG` ou `SMTP`.
- `SMS` / `MMS` — envio de mensagem de texto/multimídia para um número.
- `PhoneNumber` — chamada telefônica.
- `WhatsAppMessage` — abre uma conversa do WhatsApp com mensagem pré-preenchida.
- `SkypeCall` — inicia uma chamada no Skype.
- `WiFi` — conecta o dispositivo a uma rede Wi-Fi (WEP, WPA, WPA2 ou sem senha).
- `Bookmark` — cria um favorito de navegador.
- `ContactData` — contato em formato vCard ou MeCard.
- `CalendarEvent` — evento de calendário.
- `Geolocation` — localização geográfica (coordenadas ou link do Google Maps).
- `OneTimePassword` — código TOTP/HOTP compatível com apps autenticadores (ex.: Google Authenticator).
- `ShadowSocksConfig` — configuração de proxy ShadowSocks.
- `BitcoinAddress`, `BitcoinCashAddress`, `LitecoinAddress`, `BitcoinLikeCryptoCurrencyAddress`, `MoneroTransaction` — pagamentos em criptomoedas.
- `Girocode` — transferência bancária europeia (SEPA/EPC QR).
- `SwissQrCode` — QR-bill suíço.
- `BezahlCode` — formato de pagamento bancário alemão.
- `SlovenianUpnQr` — pagamento UPN QR esloveno.
- `RussiaPaymentOrder` — ordem de pagamento russa.

## Exceções

- `Dietcode.Core.Lib.Codes.Exceptions.DataTooLongException`: lançada pelo motor de codificação quando o payload excede o tamanho máximo suportado para o nível de correção de erro (e, opcionalmente, a versão fixa) escolhidos.

## Proveniência do código e licença

O motor de geração de QR Code (`QRCodeGenerator.cs`, pasta `QRCodeGenerator/`, `PayloadGenerator.cs`, pasta `PayloadGenerator/`, pastas `Extensions/`, `Exceptions/` e `Attributes/`) é portado do projeto [QRCoder](https://github.com/codebude/QRCoder), com o namespace ajustado para `Dietcode.Core.Lib.Codes`. Os detalhes da licença original (MIT) estão em [`THIRD-PARTY-NOTICES.md`](./THIRD-PARTY-NOTICES.md).

O restante do projeto — `AbstractQRCode.cs`, `Base64QRCode.cs`, `PngByteQRCode.cs`, `QRCodeData.cs`, `QrEncoder.cs` e `QrErrorCorrectionLevel.cs` — é implementação própria da Dietcode.

### Licença

MIT
