using System.Collections;

namespace Dietcode.Core.Lib.Codes
{
    /// <summary>
    /// High-level QR Code generator – minimal, clean and modern version (.NET 9).
    /// Handles: text, payloads, bytes. Delegates heavy work to internal providers.
    /// </summary>
    public sealed partial class QRCodeGenerator : IDisposable
    {
        private readonly IQRCodeEncodingProvider _encoding;
        private readonly IQRCodeVersionProvider _versioning;
        private readonly IQRCodeBuildProvider _builder;

        public QRCodeGenerator()
        {
            _encoding = new DefaultEncodingProvider();
            _versioning = new DefaultVersionProvider();
            _builder = new DefaultQRCodeBuildProvider();
        }

        // ------------------------------------------------------------
        // PUBLIC API — same as original, but extremely simplified
        // ------------------------------------------------------------

        public QRCodeData CreateQrCode(PayloadGenerator.Payload payload)
            => GenerateQrCode(payload);

        public QRCodeData CreateQrCode(PayloadGenerator.Payload payload, ECCLevel eccLevel)
            => GenerateQrCode(payload, eccLevel);

        public QRCodeData CreateQrCode(
            string plainText,
            ECCLevel eccLevel,
            bool forceUtf8 = false,
            bool utf8BOM = false,
            EciMode eci = EciMode.Default,
            int version = -1)
            => GenerateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eci, version);

        public QRCodeData CreateQrCode(byte[] binaryData, ECCLevel ecc)
            => GenerateQrCode(binaryData, ecc);

        // ------------------------------------------------------------
        // CORE GENERATION API
        // ------------------------------------------------------------

        public static ECCLevel NormalizeECC(ECCLevel ecc)
            => ecc == ECCLevel.Default ? ECCLevel.M : ecc;

        public QRCodeData GenerateQrCode(PayloadGenerator.Payload payload)
            => GenerateQrCode(
                payload.ToString(),
                payload.EccLevel,
                false,
                false,
                payload.EciMode,
                payload.Version);

        public QRCodeData GenerateQrCode(PayloadGenerator.Payload payload, ECCLevel eccLevel)
        {
            eccLevel = NormalizeECC(eccLevel);

            if (payload.EccLevel != ECCLevel.Default && eccLevel != payload.EccLevel)
                throw new ArgumentOutOfRangeException(nameof(eccLevel),
                    $"Payload exige ECC {payload.EccLevel}.");

            return GenerateQrCode(
                payload.ToString(),
                eccLevel,
                false,
                false,
                payload.EciMode,
                payload.Version);
        }


        public QRCodeData GenerateQrCode(
            string text,
            ECCLevel ecc,
            bool forceUtf8,
            bool utf8BOM,
            EciMode eciMode,
            int requestedVersion)
        {
            ecc = NormalizeECC(ecc);

            // 1) Build encoded segment
            var segment = _encoding.Encode(text, forceUtf8, utf8BOM, eciMode);

            // 2) Determine proper QR Version
            int version = _versioning.ResolveVersion(segment, ecc, requestedVersion);

            // 3) Convert segment to "raw QR bit data"
            BitArray bitStream = segment.ToBitArray(version);

            // 4) Build the QRCode final matrix (ECC, mask, modules)
            return _builder.Build(bitStream, version, ecc);
        }


        public QRCodeData GenerateQrCode(byte[] binaryData, ECCLevel ecc)
        {
            ecc = NormalizeECC(ecc);

            // numeric version resolution
            int version = _versioning.ResolveBinaryVersion(binaryData.Length, ecc);

            // prepare bit stream
            BitArray bitStream = _encoding.EncodeBinary(binaryData, version);

            // build final QR
            return _builder.Build(bitStream, version, ecc);
        }

        public void Dispose() => GC.SuppressFinalize(this);
    }

    // ---------------------------------------------------------------------------------------
    //  PROVIDERS — clean, interface-based, easy to test, easy to expand
    // ---------------------------------------------------------------------------------------

    public interface IQRCodeEncodingProvider
    {
        DataSegment Encode(string text, bool forceUtf8, bool utf8Bom, EciMode eci);
        BitArray EncodeBinary(byte[] data, int version);
    }

    public interface IQRCodeVersionProvider
    {
        int ResolveVersion(DataSegment segment, ECCLevel ecc, int requestedVersion);
        int ResolveBinaryVersion(int byteLen, ECCLevel ecc);
    }

    public interface IQRCodeBuildProvider
    {
        QRCodeData Build(BitArray rawBits, int version, ECCLevel eccLevel);
    }

    // ---------------------------------------------------------------------------------------
    //  DEFAULT PROVIDERS — stubbed, minimalistic, ready to implement
    // ---------------------------------------------------------------------------------------

    internal sealed class DefaultEncodingProvider : IQRCodeEncodingProvider
    {
        public DataSegment Encode(string text, bool forceUtf8, bool utf8Bom, EciMode eci)
        {
            // choose best encoding mode (numeric / alphanumeric / byte)
            var mode = EncodingEvaluator.GetBestMode(text, forceUtf8);

            return mode switch
            {
                EncodingMode.Numeric => new NumericDataSegment(text),
                EncodingMode.Alphanumeric => new AlphanumericDataSegment(text),
                _ => new ByteDataSegment(text, forceUtf8, utf8Bom, eci)
            };
        }

        public BitArray EncodeBinary(byte[] data, int version)
            => EncodingEvaluator.ToBitArray(data, version);
    }

    internal sealed class DefaultVersionProvider : IQRCodeVersionProvider
    {
        public int ResolveVersion(DataSegment segment, ECCLevel ecc, int requestedVersion)
            => CapacityTables.GetVersionForSegment(segment, ecc, requestedVersion);

        public int ResolveBinaryVersion(int len, ECCLevel ecc)
            => CapacityTables.GetVersionForBinary(len, ecc);
    }

    internal sealed class DefaultQRCodeBuildProvider : IQRCodeBuildProvider
    {
        public QRCodeData Build(BitArray bits, int version, ECCLevel ecc)
            => QRMatrixBuilder.BuildMatrix(bits, version, ecc);
    }
}
