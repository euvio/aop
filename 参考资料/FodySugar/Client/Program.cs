using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FodySugar;
namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            GoodNice.Fun(3.14,CultureInfo.CurrentCulture);
            Console.Read();
        }

    }

    [MethodRecorder]
    class GoodNice
    {

       
        public static string Fun(double go,CultureInfo cultureInfo)
        {
            return "ASCII";
        }


    }
}
