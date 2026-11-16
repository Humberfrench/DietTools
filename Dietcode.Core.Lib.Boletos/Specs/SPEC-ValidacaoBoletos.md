# SPEC — Engine de Validação de Boletos

**Arquivo:** `SPEC-ValidacaoBoletos.md`  
**Data:** 16/11/2026  
**Destino:** `Dietcode.Core.Lib.Boletos` (novo projeto/biblioteca)  
**Legado a migrar:** `Dietcode.Core.Lib/Boletos.Validator.cs`, classe `Dietcode.Core.Lib.Validator`  
**Status:** especificação para implementação e migração controlada

## 1. Objetivo e escopo

Criar uma engine independente, testável e reutilizável para identificar, normalizar, converter, interpretar e validar boletos bancários e de arrecadação. A engine não executa pagamentos nem substitui a consulta ao banco/convênio sobre existência, situação, encargos ou possibilidade de liquidação.

A nova implementação deve **avaliar e aproveitar as rotinas existentes** em `Dietcode.Core.Lib`, corrigindo limitações, antes de remover o código legado. Não copiar os métodos sem testes de regressão.

## 2. Quatro correções obrigatórias do texto original

1. **Arrecadação:** uma **linha digitável** de arrecadação tem **48 dígitos**; seu **código de barras** tem **44 dígitos**, geralmente iniciados em `8`.
2. **Posição do fator:** no **código de barras bancário** de 44 dígitos, o fator de vencimento ocupa as **posições 6–9**; na **linha digitável** de 47 dígitos, as **posições 34–37**. A menção às posições 34–37 *do código de barras* estava incorreta.
3. **Campos 2 e 3 da linha bancária:** são **10 dígitos do campo livre + 1 DV Módulo 10**, cada; não se devem usar, literalmente, as posições 11–20/21–31 do código de barras como se tivessem a mesma ordem da linha digitável.
4. **Reinício do fator em 2025:** em **22/02/2025**, fator **1000** inaugura o novo ciclo. A conversão depende do ciclo correto; o mesmo fator também existe historicamente no ciclo anterior. É necessário definir uma política explícita para resolver ambiguidades.

## 3. Formatos e classificação

| Comprimento normalizado | Primeiro dígito | Formato | Tratamento |
|---:|---|---|---|
| 44 | `8` | Código de barras de arrecadação | Validator de arrecadação |
| 44 | outro | Código de barras bancário | Validator bancário |
| 47 | qualquer | Linha digitável bancária | Validator bancário |
| 48 | `8` | Linha digitável de arrecadação | Validator de arrecadação |
| qualquer outro | — | Não suportado | Erro estruturado |

**Compatibilidade:** o legado aceita 46 dígitos como linha digitável; a nova engine **não deve aceitar silenciosamente** 46 dígitos como padrão. Investigar consumidores e introduzir eventual modo legado *opt-in*, devidamente documentado e testado.

### 3.1 Normalização

Aceitar exclusivamente números (`0`–`9`) e separadores permitidos (espaço, ponto, hífen); remover apenas os separadores. Rejeitar letras, outros símbolos, entrada nula/vazia e quantidades inválidas. Preservar zeros à esquerda. Não converter o código inteiro em tipos numéricos.

## 4. Leiaute bancário

### 4.1 Código de barras: 44 dígitos

| Posições (1-based) | Comprimento | Conteúdo |
|---|---:|---|
| 01–03 | 3 | Identificador da instituição |
| 04 | 1 | Moeda; no escopo inicial, `9` (BRL) |
| 05 | 1 | DV geral, conforme Módulo 11 bancário |
| 06–09 | 4 | Fator de vencimento |
| 10–19 | 10 | Valor em centavos |
| 20–44 | 25 | Campo livre |

Validar instituição usando catálogo atualizável/serviço de referência, não apenas o formato de 3 dígitos; manter distinção entre instituição desconhecida e validação matemática. Validar DV geral excluindo o dígito da posição 5 e aplicando as regras do leiaute bancário. Usar `decimal` para o valor (`10 dígitos / 100m`); valor zerado deve ser sinalizado como *não informado/variável*, sem inventar um preço.

### 4.2 Linha digitável bancária: 47 dígitos

| Campo | Posições (1-based) | Composição |
|---|---|---|
| 1 | 01–10 | Banco(3) + moeda(1) + campo livre inicial(5) + DV Módulo 10(1) |
| 2 | 11–21 | Próximos 10 dígitos do campo livre + DV Módulo 10 |
| 3 | 22–32 | Últimos 10 dígitos do campo livre + DV Módulo 10 |
| 4 | 33 | DV geral do código de barras |
| 5 | 34–47 | Fator de vencimento(4) + valor em centavos(10) |

Validar os três DVs de Módulo 10, reconstruir a sequência bancária de 44 dígitos e validar também o DV geral. Conversões 44↔47 devem realizar *round-trip* sem perder dígitos.

## 5. Fator e vencimento

- Ciclo original: base **07/10/1997**; fator `9999` = **21/02/2025**.
- Ciclo novo: fator `1000` = **22/02/2025**; regra `22/02/2025 + (fator - 1000)` dias.
- Fator `0000`: sem vencimento determinado pelo campo; **não** interpretar como 07/10/1997.
- Para fatores ambíguos, a escolha do ciclo exige contexto ou política declarada; registrar a origem da decisão.
- Regra **de negócio do gateway**: quando a data for inexistente ou indeterminável e a política permitir, atribuir **D+1** da data de processamento; marcar `VencimentoEstimado=true` e `OrigemVencimento=RegraDMaisUm`. **Não declarar D+1 como vencimento original do título.**
- Regra de produto solicitada: data vencida (`DataVencimento < Hoje`) bloqueia avanço automático. Esse bloqueio é configurável, pois boletos vencidos podem ser pagáveis em canais específicos, com atualização de encargos.
- Considerar data local do negócio via `TimeProvider`/abstração equivalente, sem comparar horários do `DateTime` e sem hardcode de `DateTime.Now`.

## 6. Arrecadação

- Código de barras: **44 dígitos**; linha digitável: **48 dígitos** (4 blocos de 11 + DV).
- Posição 1: identificador `8`; posição 2: segmento; posição 3: identificador de valor/referência; posição 4: DV geral; posições 5–15: valor ou referência; posições 16–44: dados específicos.
- Indicador `6`: valor efetivo, Módulo 10; `7`: referência, Módulo 10; `8`: valor efetivo, Módulo 11; `9`: referência, Módulo 11.
- Aplicar o algoritmo indicado **ao DV geral e aos DVs dos blocos**; conferir as particularidades de Módulo 11 da arrecadação com o leiaute adotado.
- Valor monetário só é extraído automaticamente para indicadores `6` e `8`; para `7`/`9`, retornar `Valor=null` e `ValorInformadoNoCodigo=false`.
- Vencimento não é inferido pelo fator bancário. Datas específicas exigem interpretação por convênio/segmento ou serviço externo; caso contrário, a regra D+1 é apenas política operacional, marcada como estimativa.
- Segmentar corretamente prefeitura, saneamento, energia/gás, telecomunicações, órgãos governamentais, trânsito e outros, permitindo expansão conforme padrões oficiais.

## 7. Projeto e classes a criar

**Novo projeto:** `Dietcode.Core.Lib.Boletos`  
**Namespace raiz:** `Dietcode.Core.Lib.Boletos`  
**Referências:** evitar dependência circular com `Dietcode.Core.Lib`; preferir biblioteca independente. A camada de API consome os contratos da biblioteca.

Estrutura proposta:

```text
Dietcode.Core.Lib.Boletos/
├── Dietcode.Core.Lib.Boletos.csproj
├── Contracts/
│   ├── IBoletoValidator.cs
│   ├── IFormatoBoletoValidator.cs
│   └── IBancoCodigoValidator.cs
├── Models/
│   ├── BoletoValidacaoResult.cs
│   ├── BoletoValidacaoError.cs
│   └── BoletoValidacaoOptions.cs
├── Enums/
│   ├── TipoCodigoBoleto.cs
│   ├── SegmentoArrecadacao.cs
│   └── OrigemVencimento.cs
├── Validators/
│   ├── BoletoValidator.cs
│   ├── BoletoBancarioValidator.cs
│   └── BoletoArrecadacaoValidator.cs
├── Factories/
│   └── BoletoValidatorFactory.cs
├── Helpers/
│   ├── CodigoBoletoNormalizer.cs
│   ├── DigitoVerificadorHelper.cs
│   ├── ConversorBoletoBancario.cs
│   ├── ConversorBoletoArrecadacao.cs
│   └── FatorVencimentoHelper.cs
└── DependencyInjection/
    └── ServiceCollectionExtensions.cs
```

### Responsabilidades e contratos

| Classe/interface | Responsabilidade | Operações principais |
|---|---|---|
| `IBoletoValidator` | Ponto único de entrada | `Validar(string codigo, BoletoValidacaoOptions? options = null)` |
| `BoletoValidator` | Orquestra normalização, classificação, factory e resultado | `Validar`, `IdentificarTipo` |
| `IFormatoBoletoValidator` | Contrato das implementações | `ValidarCodigoBarras`, `ValidarLinhaDigitavel` |
| `BoletoBancarioValidator` | Regras bancárias | DVs, banco, moeda, valor, vencimento |
| `BoletoArrecadacaoValidator` | Regras arrecadação | segmento, indicador, DVs, valor |
| `BoletoValidatorFactory` | Seleciona validador | `ObterValidador(TipoCodigoBoleto tipo)` |
| `CodigoBoletoNormalizer` | Remoção segura de separadores | `Normalizar` |
| `DigitoVerificadorHelper` | Algoritmos puros | `Modulo10`, `Modulo11Bancario`, `Modulo11Arrecadacao` |
| `ConversorBoletoBancario` | Conversões 44↔47 | `ParaCodigoBarras`, `ParaLinhaDigitavel` |
| `ConversorBoletoArrecadacao` | Conversões 44↔48 | `ParaCodigoBarras`, `ParaLinhaDigitavel` |
| `FatorVencimentoHelper` | Cálculo do fator e datas | `CalcularVencimento`, `ValidarVencimento` |
| `IBancoCodigoValidator` | Fonte atualizável para código de banco | `EhConhecido` |

Evitar overengineering: classes/helpers podem ser `internal`; expor interfaces apenas onde DI/testes/consumidores justificarem. DTOs de retorno não são entidades EF.

### Resultado

`BoletoValidacaoResult` deverá conter:

- `bool IsValid`
- `TipoCodigoBoleto TipoCodigo`
- `string CodigoOriginal`
- `string CodigoNormalizado`
- `string? CodigoBarras`
- `string? LinhaDigitavel`
- `string? BancoCodigo`
- `SegmentoArrecadacao? Segmento`
- `decimal? Valor`
- `bool ValorInformadoNoCodigo`
- `DateOnly? DataVencimento`
- `bool VencimentoEstimado`
- `OrigemVencimento OrigemVencimento`
- `IReadOnlyList<BoletoValidacaoError> Errors`

Erros com código estável e mensagem: `CODIGO_VAZIO`, `CODIGO_INVALIDO`, `TAMANHO_INVALIDO`, `TIPO_NAO_SUPORTADO`, `BANCO_INVALIDO`, `MOEDA_INVALIDA`, `DV_INVALIDO`, `CAMPO_1_INVALIDO`, `CAMPO_2_INVALIDO`, `CAMPO_3_INVALIDO`, `ARRECADACAO_DV_INVALIDO`, `FATOR_VENCIMENTO_INVALIDO`, `VENCIMENTO_INDETERMINADO`, `BOLETO_VENCIDO`, `VALOR_NAO_INFORMADO`.

## 8. Inventário documentado da implementação legada

**Arquivo fornecido:** `Dietcode.Core.Lib/Boletos.Validator.cs`  
**Classe:** `public static class Validator`, namespace `Dietcode.Core.Lib`.

| Método atual | Comportamento observado | Destino/recomendação |
|---|---|---|
| `IdentificarTipoCodigo` | Converte enum em constantes string | Substituir por enum tipado; adaptador de compatibilidade se necessário |
| `IdentificarTipoDeCodigo` | Tamanho 44 ou 46/47/48; não classifica segmento junto com tamanho | Reaproveitar ideia de switch, corrigir formatos |
| `IdentificarTipoBoleto` | Converte enum em rótulos string | Manter mapeamento em adaptador, quando necessário |
| `IdentificarTipoDeBoleto` | Detecta cartão por partes zeradas e arrecadação por prefixo `8` e segmento | Aproveitar mapa de segmentos, corrigir ordem/critério de classificação e testar heurística de cartão |
| `ObterData` / `ObterData2` | Base fixa de 07/10/1997 + fator; devolve data-base quando fator inválido | Substituir pela lógica com reinício de 2025, ausência e ambiguidade |
| `ObterFatorData` | Extrai quatro dígitos conforme formato | Aproveitar índices (`Substring(5,4)` e `(33,4)`), com pré-validação |
| `ObterValor` | Extrai valor bancário; para arrecadação usa auxiliar | Reaproveitar índices, substituir `int` por parser robusto (`long`/`decimal`) e distinguir ausência de zero |
| `CodigoDeBarras2LinhaDigitavel` | Converte 44→47 e calcula DV dos três blocos | Bom candidato a reaproveitamento após testes e validação de entrada |
| `LinhaDigitavel2CodigoDeBarras` | Reorganiza 47→44, arrecadação via auxiliar | Aproveitar mapeamento, exigir tamanho exato e validações |
| `ValidarLinhaDigitavelBoletoArrecadacao` | Verifica quatro blocos com **Módulo 10 fixo** | Corrigir seleção dinâmica de Módulo 10/11 conforme indicador |
| `ObterCodigodeBarrasCorreto` | Tenta converter arrecadação removendo separadores/blocos | Reescrever com normalização única e tamanho estrito |
| `CalculaModulo10` | Cálculo por pesos alternados 2/1 | Reaproveitar algoritmo com testes vetoriais; rejeitar caracteres inválidos em vez de ignorá-los |
| `ObterValorArrecadacao` | Extrai posições 5–15 para indicadores de valor | Reaproveitar índices/conceito, corrigir overflow e indicadores |
| `IdentificarReferencia` | `6`/`8` = valor; `7`/`9` = referência | **Reaproveitar regra**, integrando seleção de módulo |
| `TratarCodigo` | Remove espaços, pontos e hífens | Reaproveitar política, mas rejeitar outros caracteres |
| `FormatarBarrasCorretoArrecadacao` | Formatação com heurísticas de 52 dígitos e separadores | Substituir por conversor validado 44↔48 |
| `NormalizarCodigoParaDataEValor` | Seleciona normalização por tipo | Consolidar com normalizador + conversores explícitos |

**Dependências legadas a inventariar no repositório:** `EnumTipoCodigo`, `EnumTipoBoleto`, chamadas a `Validator.*`, consumidores de `ObterData2`, serializações/retornos antigos e testes existentes.

### 8.1 Defeitos e comportamentos que NÃO devem ser copiados

1. **Fator 2025:** `ObterData` e `ObterData2` só consideram a base de 1997; falham para o novo ciclo.
2. **Falha mascarada:** fator não numérico vira `0` e a data retorna 07/10/1997; códigos malformados devem devolver erro.
3. **Classificação de cartão:** heurística por trechos zerados é aplicada antes da identificação de arrecadação; pode classificar incorretamente. Separar classificação estrutural de hipótese de produto.
4. **Aceitação de 46 dígitos:** não é um dos quatro formatos previstos na SPEC.
5. **Arrecadação Módulo 11:** validador de 48 dígitos calcula sempre Módulo 10, mesmo quando a posição 3 exige Módulo 11.
6. **Normalização incompleta:** métodos de arrecadação removem separadores em momentos diferentes; alguns ramos tornam o `Split(' ')` ineficaz após remover espaços.
7. **Possível overflow:** `ObterValorArrecadacao` interpreta 11 dígitos como `int`, cujo máximo é menor que 11 dígitos genéricos; `ObterValor` também usa `int` para campos de 10 dígitos.
8. **Falha silenciosa:** `CalculaModulo10` ignora dígitos não numéricos; a engine nova deve rejeitar entrada inválida.
9. **Sem validação completa de DV bancário:** conversões existentes calculam DV de campos, mas não há no código fornecido uma verificação de ponta a ponta de todas as regras.
10. **Retorno permissivo:** conversores legados frequentemente retornam a própria entrada quando o comprimento é insuficiente; nova API deve sinalizar erro, não retornar valor aparentemente válido.

### 8.2 Estratégia de reutilização

**Reaproveitar com testes:** `CalculaModulo10`; índices e reorganização de `CodigoDeBarras2LinhaDigitavel`/`LinhaDigitavel2CodigoDeBarras`; mapa de segmentos; `IdentificarReferencia`; lógica básica de extração de valor e fator.

**Reescrever ou corrigir:** cálculo de vencimento, módulo 11, validação de arrecadação, checagem de banco, normalização estrita, DTO de resultado e classificação de tipos.

É obrigatório documentar nos novos métodos o propósito, entradas, saídas, índices 0-based vs posições 1-based, regras de DV e erros esperados em XML Documentation (`/// <summary>`, `param`, `returns`).

## 9. Migração e remoção do código antigo

1. Criar `Dietcode.Core.Lib.Boletos` e seu projeto de testes `Dietcode.Core.Lib.Boletos.Tests` (xUnit).
2. Levantar chamadas a `Dietcode.Core.Lib.Validator` em **toda a solução e nos demais consumidores**, não só nas três APIs REST.
3. Congelar comportamento atual em testes caracterizadores; criar vetores conhecidos válidos/inválidos para todos os tipos.
4. Implementar e testar engine nova sem remover a antiga.
5. Atualizar referências de projeto/pacote, DI, namespaces e chamadas dos consumidores para `Dietcode.Core.Lib.Boletos.*`.
6. Para APIs incompatíveis, criar **adaptador de transição temporário** apenas se necessário; marcar como obsoleto e estabelecer prazo de retirada.
7. Testar integração e regressão (valores, vencimento, prefixos, DVs, linhas bancárias e arrecadação).
8. **Somente depois** de migrar referências e concluir testes, remover `Boletos.Validator.cs` e tipos/auxiliares exclusivos de boleto do **projeto `Dietcode.Core.Lib`**. Não remover enums ou classes ainda usados por outras áreas.
9. Compilar todas as soluções e consumidores e validar publicação/empacotamento.
10. Atualizar changelog e documentação; publicar nova versão de acordo com a política do item 10.

**Importante:** esta SPEC planeja alterações; não modifica automaticamente o repositório existente.

## 10. Versionamento solicitado

Ao concluir a migração, atualizar a versão **`x.y.z` → `x.(y+1).0`** (*minor bump*, zerando o patch).

Exemplos: `1.4.7 → 1.5.0` e `2.9.3 → 2.10.0`.

- Conferir a versão atual real do(s) `.csproj`, `Directory.Build.props`, `Directory.Packages.props` ou pipeline de empacotamento; **não inventar a versão**.
- Atualizar `Version`, `PackageVersion`, `AssemblyVersion` e `FileVersion` **conforme a estratégia de versionamento já adotada**, sem alterar propriedades indevidamente.
- Como remover uma API pública pode ser *breaking change*, inspecionar contratos/consumidores antes do release; se houver consumidores externos dependentes da classe removida, avaliar **major bump** ou período de compatibilidade, registrando exceção à regra `x.(y+1).0` em vez de quebrar contratos inadvertidamente.
- Se os projetos geram pacotes distintos, explicitar quais pacotes recebem a atualização e quais dependências precisam ser elevadas.

## 11. Testes de aceite

| ID | Cenário | Esperado |
|---|---|---|
| T01 | 44 bancário válido | Identifica, valida DV, extrai valor/fator |
| T02 | 47 bancário válido | Valida DVs, converte para 44 |
| T03 | 44 arrecadação válido | Identifica segmento/indicador e valida DV |
| T04 | 48 arrecadação válido | Valida quatro blocos e converte para 44 |
| T05 | Tamanhos 43, 45, 46, 49 | Rejeita fora de modo legado explícito |
| T06 | Letras e outros caracteres | Rejeita, não ignora |
| T07 | Espaços, hífens, pontos | Normaliza sem perder zeros |
| T08 | Banco não reconhecido | Erro de banco, distinto do DV |
| T09 | Moeda não suportada | Erro de moeda |
| T10 | DV geral bancário alterado | Rejeita |
| T11–T13 | DV dos campos 1/2/3 alterado | Rejeita cada campo |
| T14 | Fator 9999 histórico | 21/02/2025 no ciclo original |
| T15 | Fator 1000 novo | 22/02/2025 no novo ciclo |
| T16 | Fator 1001 novo | 23/02/2025 no novo ciclo |
| T17 | Fator 0000 | Data não codificada; D+1 apenas por política |
| T18 | Fator ambíguo sem contexto | Resultado indeterminado/erro configurado |
| T19 | Documento vencido | Bloqueia pela política do produto |
| T20 | Documento vencendo hoje | Aceita pela política do produto |
| T21 | Valor bancário 0000012345 | R$ 123,45 |
| T22 | Valor zero | Sinaliza ausência/variabilidade |
| T23 | Arrecadação indicador 6 | Valor + Módulo 10 |
| T24 | Arrecadação indicador 7 | Referência + Módulo 10, sem valor inferido |
| T25 | Arrecadação indicador 8 | Valor + Módulo 11 |
| T26 | Arrecadação indicador 9 | Referência + Módulo 11 |
| T27 | Valor de arrecadação com 11 dígitos acima de `int.MaxValue` | Sem overflow |
| T28 | Round-trip bancário 44→47→44 | Igual ao original |
| T29 | Round-trip arrecadação 44→48→44 | Igual ao original |
| T30 | Integração de todos os consumidores migrados | Compilação e regressão aprovadas |
| T31 | Remoção do legado | Nenhuma referência remanescente indevida |
| T32 | Versionamento | Minor incrementado, patch zerado, ou exceção documentada |

### Critérios de conclusão

- Engine independente com contratos públicos mínimos e XML docs.
- Cobertura de testes para quatro formatos, algoritmos, ciclos, valor e conversões.
- Reutilização do legado avaliada método a método (conforme inventário).
- Todas as referências migradas; classe antiga removida só após testes.
- Versão atualizada e changelog publicado.

## 12. Decisões e observações de implementação (16/11/2026)

Registro do que foi decidido na implementação do item 7 em diante. Onde houver conflito com as seções acima, vale esta.

### 12.1 Formatos e entrada

- Formatos suportados: **44** (código de barras: bancário, ou arrecadação quando inicia em `8`), **47** (linha digitável bancária) e **48** iniciado em `8` (linha digitável de arrecadação). **46 dígitos não é suportado**, sem modo legado.
- Os 7 códigos de arrecadação reais fornecidos têm 44 dígitos: o código de barras de arrecadação (44 iniciado em `8`) faz parte do escopo, como na tabela da §3.
- A normalização também descarta espaços e quebras de linha nas pontas (comum ao colar); no meio só espaço, ponto e hífen são aceitos.

### 12.2 Resultado

- `Valor` é `decimal` e vem `0` quando não informado (`ValorInformadoNoCodigo = false`), em vez de `decimal?`.
- Novos campos: `ValorMulta` e `ValorDesconto` (sempre `0`; o código de barras do padrão não os carrega), `DataVencimentoOriginal`, `VencimentoAjustadoDiaUtil`, `Vencido`, `CicloFator` e `PossivelBoletoCartao`.
- `BoletoValidacaoError` ganhou `Bloqueante`. `IsValid` considera só os bloqueantes. Avisos não bloqueantes: `VALOR_NAO_INFORMADO`, `SEGMENTO_DESCONHECIDO`, `ARRECADACAO_DV_NAO_CONFERIDO`, `VENCIMENTO_INDETERMINADO` quando vale D+1. `BOLETO_VENCIDO` é bloqueante conforme `BloquearVencido` (padrão `true`).
- Códigos de erro acrescentados à lista da §7: `ARRECADACAO_CAMPO_1..4_INVALIDO`, `ARRECADACAO_INDICADOR_INVALIDO` e `SEGMENTO_DESCONHECIDO`. `BANCO_INVALIDO` não é produzido (ver 12.4).
- `OrigemVencimento`: `NaoInformado`, `FatorVencimento`, `CampoLivreArrecadacao`, `RegraDMaisUm`.

### 12.3 Fator, vencimento e data

- **Política do ciclo** (só boletos futuros são avaliados): fator ≥ 1000 usa o ciclo novo (`22/02/2025 + fator − 1000`) enquanto a data não passar de hoje + `JanelaFuturaFatorDias` (5500; confirmar no leiaute oficial); além disso lê o ciclo original, sempre passado. Isso resolve T14 a T16. O T18 deixa de existir como "ambíguo": o ciclo escolhido vem em `CicloFator`.
- Fatores `0001` a `0999` são inválidos (`FATOR_VENCIMENTO_INVALIDO`); `0000` não tem data (D+1 por política).
- Vencimento em sábado ou domingo é levado para a **segunda-feira**, inclusive a estimativa D+1. Feriados **não** são tratados.
- "Hoje" é a data **local** (sem UTC) de `TimeProvider.GetLocalNow()`. Vencendo hoje não é vencido.
- Arrecadação: nos 7 exemplos reais o campo livre traz o vencimento em AAAAMMDD nas posições 20–27 do código de barras. É lido como `CampoLivreArrecadacao` quando for data válida (anos 2000–2099). É **convenção dos convênios**, não regra do padrão; pode ser desligado em `InterpretarDataDoCampoLivreDeArrecadacao`. Sem data, vale D+1.

### 12.4 Banco, moeda e cartão

- Banco desconhecido é **ignorado**: não há catálogo, `IBancoCodigoValidator` não foi criado e o fluxo segue para valor e data. `BancoCodigo` é só devolvido.
- Só a moeda `9` é aceita (T09).
- A heurística de cartão do legado foi mantida como **opcional**: `PossivelBoletoCartao = true` quando o bancário tem fator e valor zerados. Não interfere em `IsValid`.

### 12.5 Arrecadação

- DV geral: posições 1–3 e 5–44, módulo do indicador (`6`/`7` Módulo 10, `8`/`9` Módulo 11). Linha de 48: cada bloco de 11 dígitos usa o mesmo módulo.
- Módulo 11 da arrecadação: pesos 2 a 9 da direita para a esquerda, resto 0 ou 1 → DV 0, senão `11 − resto`. **Pendente de conferência** com o leiaute oficial e com boletos reais de indicador `8` e `9` (os 7 reais são indicador `6`, Módulo 10, e todos conferem).
- **Decisão de 16/11/2026:** enquanto não houver conferência, o DV divergente nos indicadores `8` e `9` **não bloqueia** (`ValidarModulo11Arrecadacao = false`, padrão): vira o aviso `ARRECADACAO_DV_NAO_CONFERIDO` (um só por código) e valor e vencimento são extraídos normalmente. Os indicadores `6` e `7` (Módulo 10, conferidos com boletos reais) sempre bloqueiam. Ao conferir um boleto real de `8` ou `9`, o padrão passa a `true`.
- Módulo 11 bancário: `11 − resto`, com 0, 10 e 11 virando 1. Conferido com os 9 boletos bancários reais.
- Valor só nos indicadores `6` e `8` (11 dígitos, `long`); em `7` e `9` o valor é `0`.
- Segmentos: mapa herdado do legado (6 e 9 = `Outros`, 7 = trânsito). `0` e `8` viram `Desconhecido` com aviso, em vez de "Banco" como no legado.

### 12.6 Legado: defeitos adicionais encontrados

Além dos de §8.1:

11. `ObterValor` de arrecadação a partir da **linha digitável de 48 dígitos** devolvia valor errado: `FormatarBarrasCorretoArrecadacao` não tira os DVs dos blocos e o `Substring(4, 11)` pegava o DV do bloco 1 no meio.
12. `IdentificarTipoDeBoleto` mapeava segmento desconhecido para `Banco`.
13. `TratarCodigo` remove `.`, mas `ObterCodigodeBarrasCorreto` e `FormatarBarrasCorretoArrecadacao` não.
14. Não há nenhum consumidor do `Validator` dentro do repositório DietTools (sem chamadas além da própria classe); os consumidores citados na §9 estão em outros repositórios.

### 12.7 Projeto, testes e versão

- Biblioteca independente (sem referência a `Dietcode.Core.Lib`), pacote `Dietcode.Core.Lib.Boletos` **10.0.0**, com `AddDietcodeBoletos(...)` no namespace `Dietcode.Core.Lib.Boletos.DependencyInjection`.
- Os testes ficam no projeto central `Dietcode.UnitTests` (pasta `CoreLibBoletos`), não em um `Dietcode.Core.Lib.Boletos.Tests` separado. Vetores reais fornecidos pelo time em 16/11/2026: 9 bancários e 7 de arrecadação; os demais são sintéticos e identificados como tais.
- `Dietcode.Core.Lib`: ao concluir a migração e remover o legado, sobe de 10.12.0 para **10.14.0** (a 10.13.0 é pulada de propósito). A remoção ainda **não** foi feita; depende da migração dos consumidores.

