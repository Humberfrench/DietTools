namespace Dietcode.Core.Lib.Boletos;

/// <summary>
/// O que um validador de formato extrai do código, antes das políticas de
/// vencimento e valor que o <see cref="BoletoValidator"/> aplica.
/// </summary>
internal sealed class FormatoBoletoResultado
{
    public List<BoletoValidacaoError> Errors { get; } = [];

    /// <summary>Código de barras de 44 dígitos, quando obtido.</summary>
    public string? CodigoBarras { get; set; }

    /// <summary>Linha digitável de 47/48 dígitos, quando obtida.</summary>
    public string? LinhaDigitavel { get; set; }

    public string? BancoCodigo { get; set; }

    public SegmentoArrecadacao? Segmento { get; set; }

    /// <summary>Valor em centavos; 0 se ausente ou se a arrecadação for por referência.</summary>
    public long ValorCentavos { get; set; }

    /// <summary>Fator de vencimento (4 dígitos) do boleto bancário.</summary>
    public int? Fator { get; set; }

    /// <summary>Data AAAAMMDD válida no campo livre da arrecadação (posições 20–27), se houver.</summary>
    public DateOnly? DataCampoLivre { get; set; }

    /// <summary>Bancário com fator e valor zerados (hipótese de boleto de cartão).</summary>
    public bool PossivelBoletoCartao { get; set; }
}
