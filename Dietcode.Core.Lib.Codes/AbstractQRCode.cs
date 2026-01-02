namespace Dietcode.Core.Lib.Codes
{
    /// <summary>
    /// Classe base abstrata para geradores de QR Code.
    /// Implementações derivadas devem transformar os dados do QR em um formato específico 
    /// (PNG, SVG, Bitmap, PDF, etc.).
    /// </summary>
    public abstract class AbstractQRCode : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Dados internos do QR Code gerados por <see cref="QRCodeGenerator.CreateQrCode"/>.
        /// As classes derivadas utilizam esses dados para realizar a renderização.
        /// </summary>
        protected QRCodeData? QrCodeData { get; private set; }

        /// <summary>
        /// Construtor padrão. Normalmente usado em cenários de interoperabilidade (COM) 
        /// ou quando os dados do QR serão atribuídos posteriormente.
        /// </summary>
        protected AbstractQRCode() { }

        /// <summary>
        /// Construtor que recebe a estrutura de dados do QR Code.
        /// </summary>
        /// <param name="data">Instância de <see cref="QRCodeData"/> previamente gerada.</param>
        protected AbstractQRCode(QRCodeData data)
        {
            QrCodeData = data ?? throw new ArgumentNullException(nameof(data));
        }

        /// <summary>
        /// Permite definir os dados do QR Code após a criação da instância.
        /// Útil quando o objeto é construído por frameworks ou serviços externos.
        /// </summary>
        /// <param name="data">Instância de <see cref="QRCodeData"/> previamente gerada.</param>
        public virtual void SetQRCodeData(QRCodeData data)
        {
            ThrowIfDisposed();
            QrCodeData = data ?? throw new ArgumentNullException(nameof(data));
        }

        /// <summary>
        /// Libera os recursos associados ao QRCodeData.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            if (QrCodeData is IDisposable d)
                d.Dispose();

            QrCodeData = null;
            _disposed = true;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Lança exceção se o objeto já tiver sido descartado.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AbstractQRCode));
        }
    }
}

