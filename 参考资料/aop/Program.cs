using MethodBoundaryAspect.Fody.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApp7
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Test();
            }
         
               catch
            {
                Console.WriteLine("end");
            }
            Console.Read();
        }

        [TransactionScope]
        static void Test()
        {
            
                Console.WriteLine("Hello World.");
                throw new Exception();
            
         
        }
    }

    public sealed class TransactionScopeAttribute : OnMethodBoundaryAspect
    {
        
        public override void OnEntry(MethodExecutionArgs args)
        {
            args.MethodExecutionTag = "";
        }

        public override void OnExit(MethodExecutionArgs args)
        {

        }

        public override void OnException(MethodExecutionArgs args)
        {
            args.FlowBehavior = FlowBehavior.Return;
            var ret = MessageBox.Show("1", "",MessageBoxButtons.YesNo);
            if(ret == DialogResult.No)
            {
                args.FlowBehavior = FlowBehavior.RethrowException;
                return;
            }



           args.Method.Invoke(args.Instance, args.Arguments);
        }
    }
}
