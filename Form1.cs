
namespace TestEnviroment
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Print();

        }

        private void Print()
        {
            Printer print = new Printer(new DrawPage(richTextBox1).CreateOnCallLog());
            print.Print();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
