using Dietcode.Database;
using Dietcode.Database.Enums;

namespace TestingDb
{
    public partial class Form1 : Form
    {
        private string conectionString = "Data Source=db.mssql.credpay.com.br;User ID=credpay-prod;Password=V%Xqv*DSaeYZ;Initial Catalog=LogCredPay;TrustServerCertificate=true";
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {

            var db = new Repository<LogManutencao>(conectionString, EnumBancos.SqlServer);

            await db.Add(new LogManutencao
            {
                Processo = "Teste",
                Comando = "Teste",
                Data = DateTime.Now
            });

            var lista = (await db.Get()).ToList();

            var dado = await db.Get(413);

            await db.Delete(414);
        }
    }
}
