namespace Dietcode.Core.Lib.Codes
{
    /// <summary>
    /// Nível de correção de erro do QR Code.
    /// </summary>
    public enum QrErrorCorrectionLevel
    {
        /// <summary>
        /// Baixa redundância (L) — suporta ~7% de dano.
        /// </summary>
        L,

        /// <summary>
        /// Médio (M) — padrão recomendado. Suporta ~15% de dano.
        /// </summary>
        M,

        /// <summary>
        /// Alta (Q) — suporta ~25% de dano.
        /// </summary>
        Q,

        /// <summary>
        /// Máxima (H) — suporta ~30% de dano.
        /// </summary>
        H
    }
}
