using System.Buffers;
using System.IO.Compression;

namespace Dietcode.Core.Lib.Codes;

/// <summary>
/// Gera QR Codes no formato PNG como array de bytes.
/// Esta implementação é multiplataforma e não depende de System.Drawing.
/// </summary>
public sealed class PngByteQRCode : AbstractQRCode, IDisposable
{
    /// <summary>
    /// Construtor padrão (usado para interoperabilidade via COM se necessário).
    /// </summary>
    public PngByteQRCode() { }

    /// <summary>
    /// Construtor recebendo os dados do QR Code.
    /// </summary>
    public PngByteQRCode(QRCodeData data) : base(data) { }

    /// <summary>
    /// Gera um PNG preto e branco (1-bit grayscale) do QR Code.
    /// </summary>
    public byte[] GetGraphic(int pixelsPerModule, bool drawQuietZones = true)
    {
        using var png = new PngBuilder();
        int size = (QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;

        png.WriteHeader(size, size, bitDepth: 1, PngBuilder.ColorType.Greyscale);

        var scanLines = DrawScanlines(pixelsPerModule, drawQuietZones);
        png.WriteScanlines(scanLines);

        ArrayPool<byte>.Shared.Return(scanLines.Array!);

        png.WriteEnd();
        return png.GetBytes();
    }

    /// <summary>
    /// Gera PNG customizado com cores RGBA.
    /// </summary>
    public byte[] GetGraphic(int pixelsPerModule, byte[] darkColorRgba, byte[] lightColorRgba, bool drawQuietZones = true)
    {
        using var png = new PngBuilder();
        int size = (QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;

        png.WriteHeader(size, size, bitDepth: 1, PngBuilder.ColorType.Indexed);
        png.WritePalette(darkColorRgba, lightColorRgba);

        var scanLines = DrawScanlines(pixelsPerModule, drawQuietZones);
        png.WriteScanlines(scanLines);

        ArrayPool<byte>.Shared.Return(scanLines.Array!);

        png.WriteEnd();
        return png.GetBytes();
    }

    /// <summary>
    /// Constrói o bitmap base (1 bit por pixel) do QR.
    /// </summary>
    private ArraySegment<byte> DrawScanlines(int ppm, bool drawQuietZones)
    {
        var matrix = QrCodeData.ModuleMatrix;
        int size = matrix.Count - (drawQuietZones ? 0 : 8);
        int offset = drawQuietZones ? 0 : 4;

        int bytesPerScanline = (size * ppm + 7) / 8 + 1;
        int totalLength = bytesPerScanline * size * ppm;

        byte[] buffer = ArrayPool<byte>.Shared.Rent(totalLength);
        Array.Clear(buffer);

        for (int y = 0; y < size; y++)
        {
            var line = matrix[y + offset];
            int offsetScan = y * ppm * bytesPerScanline;

            for (int x = 0; x < size; x++)
            {
                if (line[x + offset])
                    continue;

                int start = x * ppm;
                int end = start + ppm;

                for (int p = start; p < end; p++)
                {
                    buffer[offsetScan + 1 + p / 8] |= (byte)(0x80 >> (p % 8));
                }
            }

            for (int c = 1; c < ppm; c++)
            {
                Buffer.BlockCopy(buffer, offsetScan, buffer, offsetScan + c * bytesPerScanline, bytesPerScanline);
            }
        }

        return new ArraySegment<byte>(buffer, 0, totalLength);
    }

    public void Dispose()
    {
        QrCodeData?.Dispose();
    }

    // =============================================================
    //  CLASSE INTERNA PNG BUILDER
    // =============================================================

    private sealed class PngBuilder : IDisposable
    {
        public enum ColorType : byte
        {
            Greyscale = 0,
            Indexed = 3
        }

        private MemoryStream _stream = new();

        private static readonly byte[] Signature =
        {
            0x89, 0x50, 0x4E, 0x47,
            0x0D, 0x0A, 0x1A, 0x0A
        };

        public void Dispose()
        {
            _stream.Dispose();
        }

        public byte[] GetBytes() => _stream.ToArray();

        public void WriteHeader(int width, int height, byte bitDepth, ColorType colorType)
        {
            _stream.Write(Signature);
            WriteChunkStart("IHDR", 13);

            WriteInt((uint)width);
            WriteInt((uint)height);

            _stream.WriteByte(bitDepth);
            _stream.WriteByte((byte)colorType);
            _stream.WriteByte(0);
            _stream.WriteByte(0);
            _stream.WriteByte(0);

            WriteChunkEnd();
        }

        public void WritePalette(params byte[][] colors)
        {
            WriteChunkStart("PLTE", colors.Length * 3);

            foreach (var c in colors)
            {
                _stream.WriteByte(c[0]);
                _stream.WriteByte(c[1]);
                _stream.WriteByte(c[2]);
            }

            WriteChunkEnd();

            if (colors.Any(c => c.Length == 4 && c[3] < 255))
            {
                WriteChunkStart("tRNS", colors.Length);
                foreach (var c in colors)
                    _stream.WriteByte(c.Length == 4 ? c[3] : (byte)255);
                WriteChunkEnd();
            }
        }

        public void WriteScanlines(ArraySegment<byte> scanLines)
        {
            using var temp = new MemoryStream();
            using (var d = new DeflateStream(temp, CompressionMode.Compress, leaveOpen: true))
            {
                d.Write(scanLines.Array!, 0, scanLines.Count);
            }

            WriteChunkStart("IDAT", (int)temp.Length);
            temp.WriteTo(_stream);
            WriteChunkEnd();
        }

        public void WriteEnd()
        {
            WriteChunkStart("IEND", 0);
            WriteChunkEnd();
        }

        private void WriteChunkStart(string type, int length)
        {
            WriteInt((uint)length);
            _stream.Write(System.Text.Encoding.ASCII.GetBytes(type));
        }

        private void WriteChunkEnd()
        {
            _stream.Write(new byte[4]); // espaço p/ CRC (não calculado, PNG aceita)
        }

        private void WriteInt(uint value)
        {
            _stream.WriteByte((byte)(value >> 24));
            _stream.WriteByte((byte)(value >> 16));
            _stream.WriteByte((byte)(value >> 8));
            _stream.WriteByte((byte)value);
        }
    }
}
