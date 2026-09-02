# Dietcode.Core.Security

Biblioteca de criptografia simétrica (AES) para aplicações .NET. Fornece criptografia autenticada moderna (AES-GCM) e mantém compatibilidade de leitura com o formato legado (AES-ECB) usado em versões anteriores do Dietcode.

Este pacote substitui a criptografia AES que antes vivia em `Dietcode.Core.Lib` (pasta `Cryptography`), agora removida daquele projeto.

## Instalação

```bash
dotnet add package Dietcode.Core.Security --version 10.0.0
```

## Funcionalidades

- `AES`: criptografia/decriptografia atual, baseada em AES-GCM (autenticada).
- `V1.AES`: implementação legada baseada em AES-ECB, mantida `[Obsolete]` apenas para compatibilidade de leitura de dados antigos.

## AES (atual)

```csharp
using Dietcode.Core.Security;

string? cifrado = AES.Encrypt("dado sensivel", "minha-chave");
string? texto = AES.Decrypt(cifrado, "minha-chave");
```

Detalhes de implementação:

- A chave informada passa por derivação PBKDF2 (SHA-256, 100.000 iterações, salt fixo interno) para gerar uma chave AES-256 de 32 bytes.
- `Encrypt` usa AES-GCM: gera um nonce aleatório de 12 bytes, produz o texto cifrado e uma tag de autenticação de 16 bytes, empacota `nonce + tag + cipher` em Base64 e prefixa o resultado com `v2:`.
- `Decrypt` identifica o prefixo `v2:` e decripta via AES-GCM. Se o texto não tiver esse prefixo, cai automaticamente para `DecryptLegacyEcb`, permitindo ler dados cifrados pela versão anterior (AES-ECB) sem precisar migrar tudo de uma vez.
- Um `Ciphertext inválido.` (`CryptographicException`) é lançado se o payload `v2:` estiver truncado (menor que nonce + tag).

## V1.AES (legado, obsoleto)

```csharp
using Dietcode.Core.Security.V1;

#pragma warning disable CS0618
string? cifrado = AES.Encrypt("dado sensivel", "minha-chave");
string? texto = AES.Decrypt(cifrado, "minha-chave");
#pragma warning restore CS0618
```

- Usa AES em modo ECB com padding PKCS7 — sem autenticação e sem nonce, portanto **não recomendado para novos dados**.
- A chave é normalizada para exatamente 16 caracteres (truncada ou preenchida com espaços à esquerda).
- Mantido apenas para não quebrar a leitura de valores já persistidos com essa versão. Para gravar novos dados, use `AES` (namespace raiz).

## Licença

MIT
