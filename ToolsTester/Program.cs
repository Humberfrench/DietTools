using Dietcode.Core.Lib;
using Dietcode.Core.Lib.Rest;
using static Dietcode.Core.Lib.Rest.HttpService;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


var request = new SimulatorRequest
{
    ValorPrincipal = 1000m,
    ValorR = 0m,
    ValorHonorarios = 0m,
    ServicoId = 1,
    SubDominioId = 28,
    UsuarioId = 1695
};

var payload = @"{
  ""objective"": ""NEW_UNIFIED_TRANSACTION"",
  ""data"": {
    ""originId"": 16,
    ""companyId"": 1,
    ""organizationId"": 22007,
    ""clientId"": 22289,
    ""sellType"": 1,
    ""serviceTypeId"": 14,
    ""honorariumFeeValue"": 0,
    ""returnPercentage"": 0,
    ""status"": 0,
    ""client"": {
      ""name"": ""renilson  a de souza "",
      ""identifier"": ""523.440.938-42"",
      ""email"": ""vcarolyne37.vc@gmail.com""
    },
    ""payments"": [
      {
        ""identifier"": ""624280"",
        ""type"": 1,
        ""value"": 29999.00000,
        ""datetime"": ""09/12/2025 10:44:05"",
        ""debtValue"": 29999.00000,
        ""originalValue"": 0,
        ""refundedValue"": 0,
        ""installments"": 1,
        ""installmentsValue"": 29999.00000,
        ""merchantId"": ""1331"",
        ""chargeStatus"": ""CHARGED"",
        ""hash"": ""693827a403580f1215688c4f"",
        ""has3ds"": false,
        ""hasFraudPrevention"": false,
        ""status"": 0,
        ""acquirer"": {
          ""name"": ""GETNET"",
          ""brand"": ""MASTERCARD"",
          ""nsu"": """",
          ""authorization"": """",
          ""authorizationResponseCode"": ""00"",
          ""terminal"": """",
          ""softDescriptor"": ""CP *estopimdafiel"",
          ""taxRate"": 0,
          ""costValue"": 0,
          ""taxValue"": 0,
          ""anticipationTax"": 0,
          ""automaticTransfer"": false
        },
        ""card"": {
          ""brand"": ""MASTERCARD"",
          ""holderName"": ""renilson  a de souza"",
          ""internationalCard"": false
        }
      }
    ],
    ""products"": [
      {
        ""operationId"": ""659294"",
        ""subDomainId"": 1331,
        ""merchantId"": 28,
        ""merchantDescription"": ""estopimdafiel"",
        ""contract"": """",
        ""creditor"": ""P80011444710"",
        ""payerEmail"": ""vcarolyne37.vc@gmail.com"",
        ""operator"": ""5582"",
        ""reference"": ""CP-vYaMJyGG0g"",
        ""value"": 100.00,
        ""description"": ""estopimdafiel"",
        ""honorariumValue"": 0,
        ""commissionValue"": 0,
        ""dueDateTime"": ""2025-12-09T10:57:13"",
        ""status"": ""ACTIVE"",
        ""detran"": {
          ""digitalAuthentication"": """",
          ""carPlate"": """",
          ""renavam"": """"
        },
        ""barCode"": """",
        ""detranRenavam"": """"
      }
    ]
  }
}";
var url = "https://cloudfleet-homolog.credpay.com.br/transactions/new";
var result = await Post<object, object>(url, payload, EnumApiRest.Bearer, "Kj7pQ2sR8tZxY3wV6uAb9cDe5fGhNm4kL1pT7rS2zX3yW8vU5bA6cD9eF2gH3jK4");

Console.WriteLine("Processei!");

Console.WriteLine(result.Data.ToJson());

Console.Beep();
Console.ReadKey();

internal class SimulatorRequest
{
    public decimal ValorPrincipal { get; set; }
    public decimal ValorR { get; set; }
    public decimal ValorHonorarios { get; set; }
    public int ServicoId { get; set; }
    public int SubDominioId { get; set; }
    public int UsuarioId { get; set; }
}

public class RootSimulacaoResponse
{
    public RetornoSimulacao Retorno { get; set; }
    public string Mensagem { get; set; }
    public List<string> Erros { get; set; }
    public bool Valid { get; set; }
}

public class RetornoSimulacao
{
    public List<SimulacaoParcela> SimulacaoParcelas { get; set; }
    public decimal Principal { get; set; }
    public decimal ValorV0 { get; set; }
}

public class SimulacaoParcela
{
    public int Parcela { get; set; }
    public int ServicoId { get; set; }
    public decimal Valor { get; set; }
    public decimal ValorLiquido { get; set; }
    public decimal ValorDaParcelaFixa { get; set; }
    public string TipoOperacao { get; set; }
    public int TipoOperacaoId { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal TaxaFixa { get; set; }
    public decimal? TaxaMensal { get; set; }
    public decimal TaxaCobrada { get; set; }
}

