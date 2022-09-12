/*
─────────────────────────────────────────────────────────────────────
* 版   权: Future Laser Company.All rights reserved
* 作   者: Liuwei.Li@cygia.com
* 创建时间: 2021/3/8 19:52:39
* 文件描述:
─────────────────────────────────────────────────────────────────────
*/

using MethodBoundaryAspect.Fody.Attributes;
using System;
using System.Reflection;
using System.Text;

namespace FodySugar
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class MethodRecorderAttribute : OnMethodBoundaryAspect
    {
        public MethodRecorderAttribute()
        {
        }

        public override void OnEntry(MethodExecutionArgs arg)
        {
            base.OnEntry(arg);
            StringBuilder sb = new StringBuilder();
            sb.Append("Entering ");
            Formatter.AppendCallInformation(arg, sb);
            Console.WriteLine(sb);
        }

        public override void OnExit(MethodExecutionArgs arg)
        {
            base.OnExit(arg);
            StringBuilder sb = new StringBuilder();
            sb.Append("Exiting ");
            Formatter.AppendCallInformation(arg, sb);
            if (!arg.Method.IsConstructor && ((MethodInfo)arg.Method).ReturnType != typeof(void))
            {
                sb.Append(" with return value ");
                sb.Append(arg.ReturnValue);
            }
            Console.WriteLine(sb);
        }

        public override void OnException(MethodExecutionArgs arg)
        {
            base.OnException(arg);
            StringBuilder sb = new StringBuilder();
            sb.Append("Exiting ");
            Formatter.AppendCallInformation(arg, sb);
            if (!arg.Method.IsConstructor && ((MethodInfo)arg.Method).ReturnType != typeof(void))
            {
                sb.AppendLine(" with exception ");
                sb.AppendLine($"MESSAGE: {arg.Exception.Message}");
                sb.AppendLine($"TYPE: {arg.Exception.GetType()}");
                sb.AppendLine($"STACE_TRACE: {arg.Exception.StackTrace}");
            }
            Console.WriteLine(sb);
            arg.FlowBehavior = FlowBehavior.RethrowException;
        }
    }
}