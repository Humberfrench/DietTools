using System.ComponentModel;
using System.Reflection;

namespace Dietcode.Core.Lib
{
    public enum EnumTipoBoleto
    {
        [Description("Banco")]
        Banco,

        [Description("Arrecadação Prefeitura")]
        ArrecadacaoPrefeitura,

        [Description("Arrecadação Saneamento")]
        ArrecadacaoSaneamento,

        [Description("Arrecadação Energia Elétrica/Gás")]
        ArrecadacaoEnergiaEletricaOuGas,

        [Description("Arrecadação Telecomunicação")]
        ArrecadacaoTelecomunicacao,

        [Description("Arrecadação Órgãos Governamentais")]
        ArrecadacaoOrgaosGovernamentais,

        [Description("Arrecadação Taxas De Trânsito")]
        ArrecadacaoTaxasDeTransito,

        [Description("Outros")]
        Outros,

        [Description("Cartão de Crédito")]
        CartaoDeCredito
    }

    public enum EnumTipoCodigo
    {
        [Description("Código de Barras")]
        CodigoDeBarras,

        [Description("Linha Digitável")]
        LinhaDigitavel,

        [Description("Tamanho Incorreto")]
        TamanhoIncorreto
    }

    // ============================================================
    // EXTENSIONS para Description + Código Estável
    // ============================================================
    public static class EnumExtensions
    {
        /// <summary>
        /// Retorna o valor do atributo [Description], se existir.
        /// Caso contrário, retorna ToString().
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            var member = type.GetMember(value.ToString());

            if (member.Length > 0)
            {
                var attr = member[0].GetCustomAttribute<DescriptionAttribute>();
                if (attr != null)
                    return attr.Description;
            }

            return value.ToString();
        }

        /// <summary>
        /// Retorna sempre um código fixo e estável em formato UPPER_SNAKE_CASE
        /// Excelente para logs, uso interno, integração, etc.
        /// </summary>
        public static string GetCode(this Enum value)
        {
            return value.ToString().ToUpperInvariant();
        }
    }
}
