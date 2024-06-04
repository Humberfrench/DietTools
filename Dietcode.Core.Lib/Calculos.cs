using Microsoft.VisualBasic;

namespace Dietcode.Core.Lib
{
    public class Calculos
    {
        public string CalculaJuros(double taxa, double parcelas, double valoroperacao, double decimais, double tarifa)
        {
            var valorparcela = (Financial.Pmt(taxa / 100, parcelas, valoroperacao + tarifa, decimais) * -1).ToString("N2");

            return valorparcela;
        }

        public double CalculaJuros2(double taxa, double parcelas, double valoroperacao, double decimais, double tarifa)
        {
            var valorparcela = (Financial.Pmt(taxa / 100, parcelas, valoroperacao + tarifa, decimais) * -1);

            return valorparcela;
        }

        public string CalculaValorLiquido(double valor, double retencao)
        {
            return (valor * (1 - (retencao / 100))).ToString("N2");
        }

        public double CalculaValorLiquido2(double valor, double retencao)
        {
            return (valor * (1 - (retencao / 100)));
        }

        public string CalculaValorTotal(double valorParcela, double parcelas)
        {
            return (valorParcela * parcelas).ToString("N2");
        }

        public double CalculaValorTotal2(double valorParcela, double parcelas)
        {
            return (valorParcela * parcelas);
        }

    }
}
