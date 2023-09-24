using System.IO;
using System.Windows;
using Serialization;

namespace MyLibraries
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // обработка исключений
        // проверка на корректность данных на де/сериализацю
        // исправить десериализацию

        public MainWindow()
        {
            InitializeComponent();
            StreamWriter sw = new StreamWriter("result.xml");
            sw.Write("");
            sw.Close();
        }

        private void serializeA_Click(object sender, RoutedEventArgs e)
        {
            var valueA = int.Parse(field1A.Text);
            textBox.Clear();
            textBox.Text = XML.Serialize(new ClassA(valueA));
            StreamWriter sw = new StreamWriter("result.xml");
            sw.Write(textBox.Text);
            sw.Close();
        }

        private void deserializeA_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = new StreamWriter("result.xml");
            sw.Write(textBox.Text);
            sw.Close();
            var classA = XML.Deserialize<ClassA>("result.xml");
            field1A.Text = classA.ValueA.ToString();
        }

        private void serializeB_Click(object sender, RoutedEventArgs e)
        {
            var valueA = int.Parse(field1B.Text);
            var valueB = field2B.Text;
            textBox.Clear();
            textBox.Text = XML.Serialize(new ClassB(valueA, valueB));
            StreamWriter sw = new StreamWriter("result.xml");
            sw.Write(textBox.Text);
            sw.Close();
        }

        private void deserializeB_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = new StreamWriter("result.xml");
            sw.Write(textBox.Text);
            sw.Close();
            var classB = XML.Deserialize<ClassB>("result.xml");
            field1B.Text = classB.ValueA.ToString();
            field2B.Text = classB.ValueB.ToString();
        }

        private void serializeC_Click(object sender, RoutedEventArgs e)
        {
            var valueA = int.Parse(field1C.Text);
            var valueB = field2C.Text;
            var valueC = bool.Parse(field3C.Text);
            textBox.Clear();
            textBox.Text = XML.Serialize(new ClassC(valueA, valueB, valueC));
            StreamWriter sw = new StreamWriter("result.xml");
            sw.Write(textBox.Text);
            sw.Close();
        }

        private void deserializeC_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = new StreamWriter("result.xml");
            sw.Write(textBox.Text);
            sw.Close();
            var classC = XML.Deserialize<ClassC>("result.xml");
            field1C.Text = classC.ValueA.ToString();
            field2C.Text = classC.ValueB.ToString();
            field3C.Text = classC.ValueC.ToString();
        }
    }
}
