# Dietcode.Core.Lib.Helpers

Projeto auxiliar interno, compartilhado por `Dietcode.Core.Lib` e `Dietcode.Core.Lib.Rest`. Hoje contém apenas `JsonOptionsFactory` e os conversores JSON flexíveis usados por ambos.

**Não é publicado como pacote NuGet** (`IsPackable=false`, `GeneratePackageOnBuild=false`). Quem referencia este projeto via `ProjectReference` e empacota (`dotnet pack`) — hoje, `Dietcode.Core.Lib` e `Dietcode.Core.Lib.Rest` — tem o binário deste projeto **embutido diretamente** no próprio `.nupkg` (pasta `lib/<tfm>/`), em vez de virar uma dependência externa no `.nuspec`.

`IsPackable=false` é o que garante esse comportamento. Sem ele, o `dotnet pack` do projeto consumidor adiciona `Dietcode.Core.Lib.Helpers` como dependência de pacote — e como ele nunca é publicado em nenhum feed, qualquer projeto que instale `Dietcode.Core.Lib`/`Dietcode.Core.Lib.Rest` falha o restore com `NU1101` ("Unable to find package Dietcode.Core.Lib.Helpers").

Se um dia este projeto precisar ser consumido diretamente por outro pacote (não só embutido), aí sim ele precisa virar um pacote publicado de verdade — com todos os metadados (`PackageId`, `Version`, `README`, ícone etc.) que os demais projetos do repositório têm.

## Licença

MIT
