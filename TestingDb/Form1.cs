
using Dietcode.Database.Classic;

namespace TestingDb
{
    public partial class Form1 : Form
    {
        private string conectionString = "Data Source=db-mssql-prd-internal.credpay.com.br;User ID=credpay-prod;Password=V%Xqv*DSaeYZ;Initial Catalog=LogCredPay;TrustServerCertificate=true";
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {

            var db = new BaseRepository<LogManutencao>(conectionString);

            await db.BeginTransaction();

            db.SaveAuto = true;

            await db.Adicionar(new LogManutencao
            {
                Processo = "Teste",
                Comando = "Teste",
                Data = DateTime.Now
            });

            //var lista1 = (await db.LoadAll()).ToList();
            
            await db.Commit();

            var lista2 = (await db.LoadAll()).ToList();

            //var tamanho = lista1.Count == lista2.Count;

            var dado = await db.Get(413);

            //await db.Delete(414);
        }
    }
}
