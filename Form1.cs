
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
            Printer print = new Printer(new DrawPage().CreateOnCallLog());
            print.Print();

        }
    }
}
