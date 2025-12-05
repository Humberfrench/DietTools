using Dietcode.Core.Lib;
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

var url = "https://api-pagamento.credpay.com.br/Simulator";
var result = await Post<SimulatorRequest, RootSimulacaoResponse>(url, request);

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
