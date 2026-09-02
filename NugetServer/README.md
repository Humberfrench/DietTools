# NugetServer

Servidor NuGet privado local, montado com o pacote [NuGet.Server](https://www.nuget.org/packages/NuGet.Server) (v3.4.2) sobre um projeto ASP.NET (Web Forms + Web API/OData, `.NET Framework 4.8`). Não é uma biblioteca nem um app de console — é uma aplicação web (IIS/IIS Express) usada para hospedar e servir localmente os pacotes `.nupkg` das bibliotecas Dietcode (e de outros pacotes internos, como `Credpay.Tools.*`) durante o desenvolvimento, sem depender do NuGet.org ou de um feed corporativo externo.

## O que é

- `Default.aspx`: página inicial padrão do NuGet.Server, mostra a versão instalada e a URL do feed para configurar como *Package Source*.
- `App_Start/NuGetODataConfig.cs`: configura as rotas OData (`nuget/...`) e a rota de limpeza de cache (`nuget/clear-cache`) usando `NuGetV2WebApiEnabler`.
- `Web.config`: configurações do feed, como `requireApiKey`, `packagesPath`, `allowOverrideExistingPackageOnPush` e `enableFileSystemMonitoring`.
- `Packages/`: pasta física onde os arquivos `.nupkg` publicados ficam armazenados e são servidos.

Não há código de aplicação próprio além da configuração de inicialização acima — toda a lógica do feed vem do pacote `NuGet.Server`/`NuGet.Server.V2`.

## Como subir o servidor

1. Restaurar os pacotes NuGet do projeto (`packages.config`, formato clássico).
2. Abrir a solução `NugetServer.sln` no Visual Studio e rodar com IIS Express (porta configurada em `NugetServer.csproj`, `IISExpressSSLPort` 44358), ou publicar em um IIS local/servidor.
3. Acessar a raiz do site (`Default.aspx`) para ver a URL do feed a ser adicionada como *Package Source* no Visual Studio/`nuget.exe`.

## Publicar pacotes no feed

Com uma `apiKey` configurada em `Web.config` (`appSettings/apiKey`):

```bash
nuget.exe push {arquivo.nupkg} {apiKey} -Source {url-do-feed}/nuget
```

Se `apiKey` estiver vazia, o push funciona sem chave (uso local/confiança total na rede).

## Observações

- Projeto usa o formato clássico de `.csproj` (`packages.config`), não o SDK-style.
- `requireApiKey` está habilitado (`true`) no `Web.config`, mas o valor de `apiKey` está em branco por padrão — deve ser preenchido antes de expor o servidor além do ambiente local.
