# Dietcode.Core.Lib.Helpers

Projeto auxiliar interno, compartilhado por `Dietcode.Core.Lib` e `Dietcode.Core.Lib.Rest`. Hoje contém apenas `JsonOptionsFactory` e os conversores JSON flexíveis usados por ambos.

**Não é publicado como pacote NuGet** (`IsPackable=false`, `GeneratePackageOnBuild=false`). Quem referencia este projeto via `ProjectReference` e empacota (`dotnet pack`) — hoje, `Dietcode.Core.Lib` e `Dietcode.Core.Lib.Rest` — tem o binário deste projeto **embutido diretamente** no próprio `.nupkg` (pasta `lib/<tfm>/`), em vez de virar uma dependência externa no `.nuspec`.

Isso depende de **duas coisas juntas** no projeto consumidor, não só de `IsPackable=false` aqui:

1. `IsPackable=false` neste projeto — impede que o `dotnet pack` do consumidor gere uma `<dependency>` para `Dietcode.Core.Lib.Helpers` no `.nuspec` (senão: restore falha com `NU1101`, pacote nunca existe em nenhum feed).
2. O target `CopyProjectReferencesToPackage` no `.csproj` do consumidor (presente em `Dietcode.Core.Lib.csproj` e `Dietcode.Core.Lib.Rest.csproj`) — o SDK **não** embute automaticamente a DLL de uma `ProjectReference` não empacotável só por causa do item 1; sem esse target, o restore passaria mas o consumo falharia em runtime (`FileNotFoundException`/`TypeLoadException`) ao usar `JsonOptionsFactory` ou os conversores.

**Se criar um terceiro projeto que referencie este Helpers e vire pacote NuGet, copie o mesmo target `CopyProjectReferencesToPackage` pra ele.**

Se um dia este projeto precisar ser consumido diretamente por outro pacote (não só embutido), aí sim ele precisa virar um pacote publicado de verdade — com todos os metadados (`PackageId`, `Version`, `README`, ícone etc.) que os demais projetos do repositório têm.

## Licença

MIT
