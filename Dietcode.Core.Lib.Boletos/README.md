# Dietcode.Core.Lib.Boletos

Engine de validação de boletos: identifica, normaliza, converte e valida
**boleto bancário** (código de barras de 44 e linha digitável de 47 dígitos) e
**arrecadação/convênios** (código de barras de 44 iniciado em `8` e linha
digitável de 48). Devolve valor, vencimento, multa e desconto.

Não executa pagamento nem consulta banco/convênio: valida a estrutura do
código (DVs, moeda, fator), não a existência nem a situação do título.

Substitui `Dietcode.Core.Lib.Validator` (`Boletos/Validator.cs`), que sai do
`Dietcode.Core.Lib` depois da migração dos consumidores. A especificação está
em [`Specs/SPEC-ValidacaoBoletos.md`](Specs/SPEC-ValidacaoBoletos.md).

## Instalação

```bash
dotnet add package Dietcode.Core.Lib.Boletos --version 10.2.0
```

## Uso

```csharp
using Dietcode.Core.Lib.Boletos;

IBoletoValidator validador = new BoletoValidator();

var r = validador.Validar("03399.50461 76900.419847 44270.601022 4 16290000711270");

if (r.IsValid)
{
    Console.WriteLine($"{r.Valor:C} vence em {r.DataVencimento:dd/MM/yyyy}");
}
else
{
    foreach (var erro in r.Errors.Where(e => e.Bloqueante))
        Console.WriteLine($"{erro.Code}: {erro.Message}");
}
```

Com injeção de dependência:

```csharp
using Dietcode.Core.Lib.Boletos.DependencyInjection;

builder.Services.AddDietcodeBoletos(o =>
{
    o.BloquearVencido = true; // padrão
});
```

O validador é _singleton_ e usa o `TimeProvider` registrado (ou o do sistema).

## Formatos aceitos

| Tamanho | Primeiro dígito | `TipoCodigo` |
|---:|---|---|
| 44 | diferente de `8` | `CodigoBarrasBancario` |
| 44 | `8` | `CodigoBarrasArrecadacao` |
| 47 | qualquer | `LinhaDigitavelBancaria` |
| 48 | `8` | `LinhaDigitavelArrecadacao` |

Qualquer outro tamanho (inclusive 46) é rejeitado. Espaço, ponto e hífen são
removidos; qualquer outro caractere que não seja dígito é erro. Zeros à
esquerda são preservados.

## O que volta

| Propriedade | Conteúdo |
|---|---|
| `IsValid` | `true` se não houver erro **bloqueante** em `Errors` |
| `Valor` | Valor em reais; `0` quando o código não informa (`ValorInformadoNoCodigo = false`) |
| `ValorMulta`, `ValorDesconto` | Sempre `0`: o código de barras do padrão não carrega esses valores |
| `DataVencimento` | Vencimento efetivo; sábado e domingo vão para a segunda-feira |
| `DataVencimentoOriginal`, `VencimentoAjustadoDiaUtil` | Data como veio do código e se foi ajustada. Feriados não são tratados |
| `DataHoraVencimento`, `DataHoraVencimentoOriginal` | As mesmas duas datas como `DateTime`, calculadas a partir delas (00:00:00, `Kind` não especificado, data local do negócio, sem UTC); `null` sem vencimento |
| `OrigemVencimento` | `FatorVencimento`, `CampoLivreArrecadacao`, `RegraDMaisUm` ou `NaoInformado` |
| `VencimentoEstimado` | `true` quando a data é D+1 atribuída pelo gateway, não o vencimento do título |
| `Vencido` | `DataVencimento` anterior a hoje (data local) |
| `CodigoBarras`, `LinhaDigitavel` | Os dois formatos, convertidos um do outro |
| `BancoCodigo`, `Segmento` | Instituição (bancário) e segmento (arrecadação) |
| `PossivelBoletoCartao` | Boleto bancário com fator e valor zerados (hipótese de produto) |
| `Errors` | Erros e avisos, com `Code` estável e `Bloqueante` |

Erros **bloqueantes** (derrubam `IsValid`): `CODIGO_VAZIO`, `CODIGO_INVALIDO`,
`TAMANHO_INVALIDO`, `TIPO_NAO_SUPORTADO`, `MOEDA_INVALIDA`, `DV_INVALIDO`,
`CAMPO_1/2/3_INVALIDO`, `ARRECADACAO_DV_INVALIDO`, `ARRECADACAO_CAMPO_1..4_INVALIDO`,
`ARRECADACAO_INDICADOR_INVALIDO`, `FATOR_VENCIMENTO_INVALIDO`, e
`BOLETO_VENCIDO` (enquanto `BloquearVencido` for `true`).

**Avisos** (não bloqueiam): `VALOR_NAO_INFORMADO`, `SEGMENTO_DESCONHECIDO`, `ARRECADACAO_DV_NAO_CONFERIDO`,
`VENCIMENTO_INDETERMINADO` (quando vale a regra D+1) e `BOLETO_VENCIDO` com
`BloquearVencido = false`.

## Políticas (`BoletoValidacaoOptions`)

| Opção | Padrão | Efeito |
|---|---|---|
| `BloquearVencido` | `true` | `BOLETO_VENCIDO` bloqueante |
| `AtribuirDMaisUmQuandoVencimentoIndeterminado` | `true` | Sem data no código: D+1, marcado como estimativa |
| `InterpretarDataDoCampoLivreDeArrecadacao` | `true` | Lê AAAAMMDD nas posições 20–27 da arrecadação |
| `ValidarModulo11Arrecadacao` | `false` | DV divergente nos indicadores 8 e 9 bloqueia (`true`) ou só avisa (`false`) |
| `JanelaFuturaFatorDias` | `5500` | Ver abaixo. Negativo volta ao padrão; acima de 36500 é limitado |
| `FusoHorarioId` | `null` | Fuso do negócio (ex.: `America/Sao_Paulo`) para a data de hoje. `null` usa a data local do `TimeProvider` |

### Fator de vencimento

O fator reiniciou em `1000` em **22/02/2025**, então todo fator ≥ 1000 tem duas
datas possíveis. A política é a de só avaliar boletos futuros: usa-se o **ciclo
novo** (`22/02/2025 + fator − 1000`) enquanto a data não passar de hoje + 5500
dias; além disso o fator é lido no **ciclo original** (`07/10/1997 + fator`), que é
sempre passado. O ciclo escolhido vem em `CicloFator`. Fator `0000` significa sem
vencimento; `0001` a `0999` são inválidos.

### Data de hoje

Por padrão, a data **local** (sem UTC) de `TimeProvider.GetLocalNow()`; só a data é comparada.
Um boleto que vence hoje não está vencido. **Em host configurado em UTC** (contêiner, nuvem) a
data local já vira o dia seguinte no fim da tarde de Brasília: defina `FusoHorarioId`
(`America/Sao_Paulo`) ou registre um `TimeProvider` no fuso do negócio.

### Entrada

O texto informado pode ter no máximo 256 caracteres (com separadores); acima disso volta
`TAMANHO_INVALIDO` sem processar.

## Banco, moeda e cartão

O código do banco não é conferido em catálogo: banco desconhecido não bloqueia,
o fluxo segue para valor e vencimento. Só a moeda `9` (Real) é aceita.

## Arrecadação

O 3º dígito define o valor e o módulo dos DVs: `6` valor + Módulo 10, `7`
referência + Módulo 10, `8` valor + Módulo 11, `9` referência + Módulo 11.
Nos indicadores `7` e `9` o valor volta `0`. Nos módulos 11 vale resto 0 ou 1 → DV 0.

> O Módulo 11 da arrecadação (indicadores 8 e 9) foi implementado pela regra
> corrente nas bibliotecas de boleto, mas ainda **não foi conferido com um boleto
> real nem com o leiaute oficial da FEBRABAN**: os 7 exemplos reais usados nos
> testes são todos de indicador `6`. Por isso, **por padrão** um DV divergente nos
> indicadores 8 e 9 não bloqueia: vira o aviso `ARRECADACAO_DV_NAO_CONFERIDO` e
> valor e vencimento são extraídos normalmente. Os indicadores 6 e 7 (Módulo 10)
> sempre validam. Quando houver um boleto real de indicador 8 ou 9 conferido, o
> padrão de `ValidarModulo11Arrecadacao` passa a `true`.

## Testes

`Dietcode.UnitTests/CoreLibBoletos`: 9 linhas digitáveis bancárias e 7 códigos de
barras de arrecadação reais, mais vetores sintéticos (marcados nos testes) para o
que não há exemplo real.
