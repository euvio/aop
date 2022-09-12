using ShareSharp.Aspects.CustomLogging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShareSharp.Aspects
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TestLogMethod();

        }


        public void TestLogMethod()
        {
            LogMethodAttribute.Logger = new Logger();

            TestClass<double>.Add<string>(1, 2);
        }
    }


    class TestClass<T>
    {
        [CustomLogging.LogMethod("Calcautor")]
        public static int Add<K>(int a,int b)
        {
            return a + b;
        }
    }



    class Logger : ILogger
    {
        public void Debug(string content, string module)
        {
            Console.WriteLine(content);
        }

        public void Error(string content, string module)
        {
            Console.WriteLine(content);
        }

        public void Fatal(string content, string module)
        {
            Console.WriteLine(content);
        }

        public void Info(string content, string module)
        {
            Console.WriteLine(content);
        }

        public void Warn(string content, string module)
        {
            Console.WriteLine(content);
        }
    }

}
