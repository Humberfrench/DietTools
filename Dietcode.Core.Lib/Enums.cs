using System.ComponentModel;

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
}
