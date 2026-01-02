namespace Dietcode.Core.Lib.Codes
{
    /// <summary>
    /// Encoder de QR Code simples e moderno.
    /// 
    /// - Detecta automaticamente o modo (Numérico, Alfanumérico, Byte/UTF-8).
    /// - Escolhe automaticamente a menor versão possível (1–40).
    /// - Suporta ECC L/M/Q/H.
    /// - Retorna <see cref="QRCodeData"/> para ser renderizado em PNG, Base64, etc.
    /// </summary>
    public sealed class QrEncoder : IDisposable
    {
        private readonly QRCodeGenerator generator = new();

        /// <summary>
        /// Gera apenas os dados do QR Code (matriz de módulos).
        /// Use essa API quando quiser controlar o renderizador (PNG, SVG, etc.) separadamente.
        /// </summary>
        /// <param name="text">Texto a ser codificado no QR Code.</param>
        /// <param name="level">Nível de correção de erro. Padrão: <see cref="QrErrorCorrectionLevel.M"/>.</param>
        /// <param name="forceUtf8">
        /// Se true, força modo Byte UTF-8, mesmo que o texto seja compatível com Numérico/Alfanumérico.
        /// </param>
        /// <returns>Instância de <see cref="QRCodeData"/> pronta para ser renderizada.</returns>
        /// <exception cref="ArgumentNullException">Se <paramref name="text"/> for nulo.</exception>
        /// <exception cref="Exceptions.DataTooLongException">Se o conteúdo não couber em nenhuma versão do QR Code.</exception>
        public QRCodeData Encode(
            string text,
            QrErrorCorrectionLevel level = QrErrorCorrectionLevel.M,
            bool forceUtf8 = false)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            var ecc = MapEcc(level);

            // A engine interna já:
            // - Detecta modo (Numeric / Alphanumeric / Byte)
            // - Escolhe versão mínima
            // - Gera Reed–Solomon / ECC
            // - Monta a matriz completa
            return QRCodeGenerator.GenerateQrCode(
                plainText: text,
                eccLevel: ecc,
                forceUtf8: forceUtf8,
                utf8BOM: false,
                eciMode: EciMode.Default,
                requestedVersion: -1
            );
        }

        /// <summary>
        /// Gera um PNG em memória (byte[]) com o QR Code.
        /// </summary>
        /// <param name="text">Texto do QR Code.</param>
        /// <param name="level">Nível de correção de erro.</param>
        /// <param name="pixelsPerModule">Tamanho de cada “quadradinho” (módulo) em pixels.</param>
        /// <param name="drawQuietZones">Se true, desenha a borda (quiet zone) padrão.</param>
        public byte[] EncodeToPngBytes(
            string text,
            QrErrorCorrectionLevel level = QrErrorCorrectionLevel.M,
            int pixelsPerModule = 10,
            bool drawQuietZones = true)
        {
            using var data = Encode(text, level);
            using var png = new PngByteQRCode(data);
            return png.GetGraphic(pixelsPerModule, drawQuietZones);
        }

        /// <summary>
        /// Gera um PNG em Base64 (ex.: para embutir em HTML/JSON).
        /// </summary>
        /// <param name="text">Texto do QR Code.</param>
        /// <param name="level">Nível de correção de erro.</param>
        /// <param name="pixelsPerModule">Tamanho de cada “quadradinho” (módulo) em pixels.</param>
        /// <param name="darkColorHex">Cor dos módulos escuros (formato HEX, ex: "#000000").</param>
        /// <param name="lightColorHex">Cor do fundo (formato HEX, ex: "#FFFFFF").</param>
        /// <param name="drawQuietZones">Se true, desenha a borda (quiet zone) padrão.</param>
        /// <returns>String Base64 do PNG.</returns>
        public string EncodeToBase64Png(
            string text,
            QrErrorCorrectionLevel level = QrErrorCorrectionLevel.M,
            int pixelsPerModule = 10,
            string darkColorHex = "#000000",
            string lightColorHex = "#FFFFFF",
            bool drawQuietZones = true)
        {
            using var data = Encode(text, level);
            using var base64 = new Base64QRCode(data);
            return base64.GetGraphic(
                pixelsPerModule: pixelsPerModule,
                darkColorHex: darkColorHex,
                lightColorHex: lightColorHex,
                drawQuietZones: drawQuietZones
            );
        }

        /// <summary>
        /// Mapeia o enum externo amigável para o enum interno usado pela engine.
        /// </summary>
        private static ECCLevel MapEcc(QrErrorCorrectionLevel level)
            => level switch
            {
                QrErrorCorrectionLevel.L => ECCLevel.L,
                QrErrorCorrectionLevel.M => ECCLevel.M,
                QrErrorCorrectionLevel.Q => ECCLevel.Q,
                QrErrorCorrectionLevel.H => ECCLevel.H,
                _ => ECCLevel.M
            };

        /// <inheritdoc />
        public void Dispose()
        {
            generator.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
