using FodySugar.CustomLogging;
using MethodBoundaryAspect.Fody.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FodySugar
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class LogMethodAttribute : OnMethodBoundaryAspect
    {
        public LogMethodAttribute(string module)
        {
            this.Module = module;
        }

        public static Action<string,LogLevel,string> Log;

        public string Module { get; set; }

        public override void OnEntry(MethodExecutionArgs arg)
        {
            base.OnEntry(arg);
            string msg = null;
            Log(msg,LogLevel.INFO,Module);
        }

        public override void OnException(MethodExecutionArgs arg)
        {
            base.OnException(arg);

        }
    }
}
