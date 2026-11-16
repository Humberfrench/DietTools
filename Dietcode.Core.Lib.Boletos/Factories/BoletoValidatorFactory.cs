namespace Dietcode.Core.Lib.Boletos;

/// <summary>Escolhe o validador de formato pelo <see cref="TipoCodigoBoleto"/>.</summary>
internal sealed class BoletoValidatorFactory
{
    private readonly BoletoBancarioValidator _bancario = new();
    private readonly BoletoArrecadacaoValidator _arrecadacao = new();

    /// <exception cref="ArgumentOutOfRangeException">Para <see cref="TipoCodigoBoleto.NaoSuportado"/>.</exception>
    public IFormatoBoletoValidator ObterValidador(TipoCodigoBoleto tipo)
        => tipo switch
        {
            TipoCodigoBoleto.CodigoBarrasBancario or TipoCodigoBoleto.LinhaDigitavelBancaria => _bancario,
            TipoCodigoBoleto.CodigoBarrasArrecadacao or TipoCodigoBoleto.LinhaDigitavelArrecadacao => _arrecadacao,
            _ => throw new ArgumentOutOfRangeException(nameof(tipo), tipo, "Tipo de código sem validador.")
        };
}
