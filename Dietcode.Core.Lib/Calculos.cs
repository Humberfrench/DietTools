using Maths = System.Math;
namespace Dietcode.Core.Lib
{
    public class Calculos
    {
        // ============================
        //   Fórmula PMT (juros compostos)
        // ============================
        private static double Pmt(double taxa, double parcelas, double valorPresente, double valorFuturo = 0)
        {
            if (taxa == 0)
                return (valorPresente + valorFuturo) / parcelas;

            var i = taxa;
            return (i * (valorPresente + valorFuturo)) / (1 - Maths.Pow(1 + i, -parcelas));
        }

        // ============================
        //       JUROS (PMT)
        // ============================
        public string CalculaJuros(double taxa, double parcelas, double valoroperacao, double decimais, double tarifa)
        {
            var parcela = Pmt(taxa / 100, parcelas, valoroperacao + tarifa, decimais) * -1;
            return parcela.ToString("N2");
        }

        public double CalculaJuros2(double taxa, double parcelas, double valoroperacao, double decimais, double tarifa)
        {
            return Pmt(taxa / 100, parcelas, valoroperacao + tarifa, decimais) * -1;
        }

        // ============================
        //   VALOR LÍQUIDO (Retenção)
        // ============================
        public string CalculaValorLiquido(double valor, double retencao)
        {
            return (valor * (1 - retencao / 100)).ToString("N2");
        }

        public double CalculaValorLiquido2(double valor, double retencao)
        {
            return valor * (1 - retencao / 100);
        }

        // ============================
        //     VALOR TOTAL (somado)
        // ============================
        public string CalculaValorTotal(double valorParcela, double parcelas)
        {
            return (valorParcela * parcelas).ToString("N2");
        }

        public double CalculaValorTotal2(double valorParcela, double parcelas)
        {
            return valorParcela * parcelas;
        }
    }
}
