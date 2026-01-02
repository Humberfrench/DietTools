using System.Collections;
using System.IO.Compression;

namespace Dietcode.Core.Lib.Codes
{
    /// <summary>
    /// Representa a estrutura de dados de um QR Code.
    /// Contém a matriz de módulos e informações de versão,
    /// além de métodos de serialização e deserialização do formato binário compacto.
    /// </summary>
    public sealed class QRCodeData : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Matriz de módulos que compõem o QR Code.
        /// Cada linha é representada por um <see cref="BitArray"/>.
        /// </summary>
        public List<BitArray>? ModuleMatrix { get; private set; }

        /// <summary>
        /// Versão do QR Code (1–40 para QR padrão, -1 a -4 para Micro QR).
        /// </summary>
        public int Version { get; private set; }

        // ---------------------------------------------------------------------
        // Construtores
        // ---------------------------------------------------------------------

        /// <summary>
        /// Cria uma instância de <see cref="QRCodeData"/> com a versão especificada.
        /// </summary>
        public QRCodeData(int version)
        {
            Version = version;
            var size = ModulesPerSideFromVersion(version);

            ModuleMatrix = new List<BitArray>(size);
            for (int i = 0; i < size; i++)
                ModuleMatrix.Add(new BitArray(size));
        }

        /// <summary>
        /// Cria uma instância com opção de adicionar padding externo (quiet zone expandida).
        /// </summary>
        public QRCodeData(int version, bool addPadding)
        {
            Version = version;
            var size = ModulesPerSideFromVersion(version) + (addPadding ? 8 : 0);

            ModuleMatrix = new List<BitArray>(size);
            for (int i = 0; i < size; i++)
                ModuleMatrix.Add(new BitArray(size));
        }

        /// <summary>
        /// Carrega dados brutos de um arquivo .qrr (compressão opcional).
        /// </summary>
        public QRCodeData(string pathToRawData, Compression compressMode)
            : this(File.ReadAllBytes(pathToRawData), compressMode)
        {
        }

        /// <summary>
        /// Carrega a estrutura de dados a partir de um array compactado de bytes.
        /// </summary>
        public QRCodeData(byte[] rawData, Compression compressMode)
        {
            rawData = Decompress(rawData, compressMode);
            LoadFromRawData(rawData);
        }

        // ---------------------------------------------------------------------
        // Descompressão
        // ---------------------------------------------------------------------
        private static byte[] Decompress(byte[] data, Compression mode)
        {
            if (mode == Compression.Uncompressed)
                return data;

            using var input = new MemoryStream(data);
            using var output = new MemoryStream();

            Stream? decompressStream = mode switch
            {
                Compression.Deflate => new DeflateStream(input, CompressionMode.Decompress),
                Compression.GZip => new GZipStream(input, CompressionMode.Decompress),
                _ => null
            };

            if (decompressStream is null)
                return data;

            using (decompressStream)
                decompressStream.CopyTo(output);

            return output.ToArray();
        }

        // ---------------------------------------------------------------------
        // Leitura e reconstrução da matriz
        // ---------------------------------------------------------------------
        private void LoadFromRawData(byte[] raw)
        {
            if (raw.Length < 5)
                throw new InvalidDataException("Arquivo bruto inválido. Tamanho insuficiente.");

            if (raw[0] != 0x51 || raw[1] != 0x52 || raw[2] != 0x52)
                throw new InvalidDataException("Arquivo inválido. Assinatura 'QRR' não encontrada.");

            int sideLen = raw[4];

            // Detecta versão
            Version = sideLen < 29
                ? DecodeMicroVersion(sideLen)
                : DecodeStandardVersion(sideLen);

            // Reconstrói matriz
            var modules = new Queue<bool>(8 * (raw.Length - 5));

            for (int j = 5; j < raw.Length; j++)
            {
                byte b = raw[j];
                for (int i = 7; i >= 0; i--)
                    modules.Enqueue((b & (1 << i)) != 0);
            }

            ModuleMatrix = new List<BitArray>(sideLen);
            for (int y = 0; y < sideLen; y++)
            {
                var row = new BitArray(sideLen);
                for (int x = 0; x < sideLen; x++)
                    row[x] = modules.Dequeue();

                ModuleMatrix.Add(row);
            }
        }

        private static int DecodeMicroVersion(int sideLen)
        {
            if (((sideLen - 19) & 1) != 0)
                throw new InvalidDataException("Comprimento inválido para Micro QR.");

            int m = ((sideLen - 19) / 2) + 1;
            return -m;
        }

        private static int DecodeStandardVersion(int sideLen)
        {
            if (((sideLen - 29) % 4) != 0)
                throw new InvalidDataException("Comprimento inválido para QR Code padrão.");

            return ((sideLen - 29) / 4) + 1;
        }

        // ---------------------------------------------------------------------
        // Serialização
        // ---------------------------------------------------------------------

        /// <summary>
        /// Retorna os dados brutos (*.qrr) com a compressão especificada.
        /// </summary>
        public byte[] GetRawData(Compression compressMode)
        {
            ThrowIfDisposed();

            if (ModuleMatrix is null)
                throw new InvalidOperationException("Dados não carregados.");

            using var output = new MemoryStream();

            // Assinatura: QRR + reservado
            output.Write(new byte[] { 0x51, 0x52, 0x52, 0x00 }, 0, 4);

            // Tamanho da matriz
            output.WriteByte((byte)ModuleMatrix.Count);

            // Monta bitstream
            var values = new List<int>(ModuleMatrix.Count * ModuleMatrix.Count);
            foreach (var row in ModuleMatrix)
                for (int i = 0; i < row.Length; i++)
                    values.Add(row[i] ? 1 : 0);

            // Padding para alinhar a bytes
            int mod = values.Count % 8;
            if (mod != 0)
                values.AddRange(new int[8 - mod]);

            // Compacta bitstream em bytes
            for (int i = 0; i < values.Count; i += 8)
            {
                byte b = 0;
                for (int bit = 0; bit < 8; bit++)
                    b |= (byte)(values[i + bit] << (7 - bit));

                output.WriteByte(b);
            }

            // Comprime se necessário
            return compressMode switch
            {
                Compression.Deflate => Compress(output.ToArray(), Compression.Deflate),
                Compression.GZip => Compress(output.ToArray(), Compression.GZip),
                _ => output.ToArray()
            };
        }

        private static byte[] Compress(byte[] data, Compression mode)
        {
            using var output = new MemoryStream();
            Stream stream = mode switch
            {
                Compression.Deflate => new DeflateStream(output, CompressionMode.Compress),
                Compression.GZip => new GZipStream(output, CompressionMode.Compress),
                _ => output
            };

            using (stream)
                stream.Write(data, 0, data.Length);

            return output.ToArray();
        }

        /// <summary>
        /// Salva os dados brutos (*.qrr) em disco.
        /// </summary>
        public void SaveRawData(string path, Compression mode)
            => File.WriteAllBytes(path, GetRawData(mode));

        // ---------------------------------------------------------------------
        // Utilidades
        // ---------------------------------------------------------------------
        private static int ModulesPerSideFromVersion(int version)
            => version > 0
               ? 21 + (version - 1) * 4
               : 11 + (-version - 1) * 2;

        // ---------------------------------------------------------------------
        // Dispose
        // ---------------------------------------------------------------------
        public void Dispose()
        {
            if (_disposed)
                return;

            ModuleMatrix = null;
            _disposed = true;

            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(QRCodeData));
        }

        // ---------------------------------------------------------------------
        // Enum de compressão
        // ---------------------------------------------------------------------
        public enum Compression
        {
            Uncompressed,
            Deflate,
            GZip
        }
    }
}
