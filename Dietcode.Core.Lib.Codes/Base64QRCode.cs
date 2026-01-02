namespace Dietcode.Core.Lib.Codes;

/// <summary>
/// Gera QR Codes em formato Base64 (imagem PNG),
/// compatível com .NET 8/9 e sem dependências de System.Drawing.
/// </summary>
public sealed class Base64QRCode : AbstractQRCode, IDisposable
{
    /// <summary>
    /// Construtor padrão — útil para DI ou interoperabilidade COM.
    /// </summary>
    public Base64QRCode() { }

    /// <summary>
    /// Inicializa usando dados já gerados do QRCode.
    /// </summary>
    public Base64QRCode(QRCodeData data) : base(data) { }

    /// <summary>
    /// Gera o QR Code como Base64 em PNG, usando cores padrão (preto e branco).
    /// </summary>
    public string GetGraphic(int pixelsPerModule)
        => GetGraphic(pixelsPerModule, "#000000", "#FFFFFF", true);

    /// <summary>
    /// Gera o QR Code como Base64 em PNG com opções de cor e quiet zone.
    /// </summary>
    public string GetGraphic(
        int pixelsPerModule,
        string darkColorHex,
        string lightColorHex,
        bool drawQuietZones = true)
    {
        if (QrCodeData == null)
            throw new InvalidOperationException("QR Code data não foi definido. Utilize SetQRCodeData().");

        var pngRenderer = new PngByteQRCode(QrCodeData);

        var dark = ParseHexColor(darkColorHex);
        var light = ParseHexColor(lightColorHex);

        byte[] pngBytes = pngRenderer.GetGraphic(
            pixelsPerModule,
            dark,
            light,
            drawQuietZones
        );

        return Convert.ToBase64String(pngBytes);
    }

    /// <summary>
    /// Converte uma string HEX (#RRGGBB ou RRGGBB) em um array RGBA.
    /// </summary>
    private static byte[] ParseHexColor(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
            throw new ArgumentException("Cor HEX inválida.");

        hex = hex.TrimStart('#');

        if (hex.Length is not 6)
            throw new ArgumentException($"Formato de cor inválido: '{hex}'. Use RRGGBB.");

        byte r = Convert.ToByte(hex.Substring(0, 2), 16);
        byte g = Convert.ToByte(hex.Substring(2, 2), 16);
        byte b = Convert.ToByte(hex.Substring(4, 2), 16);

        // alfa 255 (sem transparência)
        return new byte[] { r, g, b, 255 };
    }

    public void Dispose() => base.Dispose();
}
