# Dietcode.Classic.Lib

Biblioteca de utilitarios para projetos .NET Framework 4.8.

Este pacote e a versao Classic de `Dietcode.Core.Lib`, com extensoes e helpers compatíveis com .NET Framework.

## Instalacao

```bash
dotnet add package Dietcode.Classic.Lib --version 4.8.0
```

## Recursos

- extensoes para string, numeros, datas, JSON e enums
- validacao e formatacao de CPF/CNPJ
- validacao de e-mail, telefone, cartao e boletos
- helper REST simples com `HttpService`
- `ApiResult<TResponse>`
- mascaramento de dados sensiveis
- analise de forca de senha
- paginacao
- criptografia AES compativel com .NET Framework

## Observacoes

- APIs baseadas em `DateOnly` nao foram portadas, pois `DateOnly` nao existe no .NET Framework 4.8.
- A criptografia AES usa implementacao compativel com .NET Framework.
- O helper REST preserva o contrato do Core, com ajustes para APIs de `HttpClient` disponiveis no net48.

## Licenca

MIT
