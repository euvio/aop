using MethodBoundaryAspect.Fody.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShareSharp.Aspects.CustomLogging
{
    public sealed class LogMethodAttribute : OnMethodBoundaryAspect
    {
        public static ILogger Logger { get; set; }
        public string Module { get; set; }

        public LogMethodAttribute()
        {
            Module = "default";
        }
        public LogMethodAttribute(string module)
        {
            Module = module;
        }
        public override void OnEntry(MethodExecutionArgs args)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("Entering ");
            Formatter.AppendCallInformation(args, stringBuilder);
            Logger.Info(stringBuilder.ToString(), Module);
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("Exiting  ");
            Formatter.AppendCallInformation(args, stringBuilder);

            if (!args.Method.IsConstructor && ((MethodInfo)args.Method).ReturnType != typeof(void))
            {
                stringBuilder.Append(" with return value ");
                stringBuilder.Append(args.ReturnValue);
            }
            Logger.Info(stringBuilder.ToString(), Module);
        }


        public override void OnException(MethodExecutionArgs args)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("Exiting ");
            Formatter.AppendCallInformation(args, stringBuilder);

            if (!args.Method.IsConstructor && ((MethodInfo)args.Method).ReturnType != typeof(void))
            {
                stringBuilder.Append(" with exception ");
                stringBuilder.Append(args.Exception.GetType().Name);
            }

            Logger.Error(stringBuilder.ToString(), Module);
        }
    }
}
