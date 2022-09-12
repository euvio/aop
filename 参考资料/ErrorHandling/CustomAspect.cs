using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostSharp.Aspects;
using System.Reflection;
using System.Threading;
using PostSharp.Serialization;
using System.Diagnostics;
using MyMachineState;
using TongfuInn.Log;

/// <summary>
/// Call(2019,"Free HK")，Wish(2020,Survive)
/// </summary>
namespace Gocool.Error
{
    #region 计时器
    /// <summary>
    /// 功能:统计一个方法执行的时间
    /// 使用:对感兴趣的方法加上改属性，会把方法开始和成功运行完毕的时间及耗时
    /// 打印到日志文件中
    /// 注意：如果方法中途抛出异常
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class Timing : OnMethodBoundaryAspect
    {
        private readonly string logger = "计时器";
        private Stopwatch watch = new Stopwatch();
        public override void OnEntry(MethodExecutionArgs args)
        {
            watch.Start();
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            watch.Stop();
            long millspan = watch.ElapsedMilliseconds;
            StringBuilder sb = new StringBuilder();
            Formatter.GetMethodSignature(sb, args);
            Logger.Write($"Timing:{sb.ToString()} taken {millspan.ToString()} ms.",logger,false, LogLevel.Debug);
        }
    }
    #endregion

    #region 记录者
    /// <summary>
    /// 功能：方法开始执行时打印开始执行和方法实参，方法结束打印运行结束和方法返回值，
    /// 并显示方法执行成功或失败，中途抛出异常则认为执行失败。
    /// </summary>
    [Serializable]
    public class Record : OnMethodBoundaryAspect
    {
        private bool success = false;
        private readonly string logger;
        private string methodSignature;
        public Record(string logger)
        { 
            this.logger = logger;
        }
        public override void OnEntry(MethodExecutionArgs args)
        {
            StringBuilder sb = new StringBuilder();
            Formatter.GetMethodSignature(sb, args);
            methodSignature = sb.ToString();
            Logger.Write($"{methodSignature} starts execution", logger);
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            if (!success)
            {
                if (!args.Method.IsConstructor)
                {
                        Logger.Write($"{methodSignature} failed.", logger,true,LogLevel.Error);
                        if (args.Exception != null)
                        {
                            if (args.Exception is CustomException)
                            {
                                Logger.Write($"Exception thrown is:{(args.Exception as CustomException).ToString()},StackTrace-->{args.Exception.StackTrace}",logger,true,LogLevel.Error);
                            }
                            else
                            {
                                Logger.Write($"Exception thrown is: Type-->{args.Exception.GetType()}, Message-->{args.Exception.Message},StackTrace-->{args.Exception.StackTrace}",logger,true,LogLevel.Error);
                            }
                        }
                    }
            }
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            success = true;
            if (!args.Method.IsConstructor)
            {
                if (((MethodInfo)args.Method).ReturnType != typeof(void))
                {
                    Logger.Write($"{methodSignature} succeed,and return value is {args.ReturnValue}", logger);
                }
                else
                {
                    Logger.Write($"{methodSignature} succeed", logger);
                }
            }
        }
    }
    #endregion

    [Serializable]
    public sealed class ManualRetryAttribute : MethodInterceptionAspect
    {
        private static MachineStateManager _machineStateManager = MachineStateManager.Instance;
        public ManualRetryAttribute() { }
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            while (true)
            {
                try
                {
                    args.Proceed();
                    return;
                }
                catch(Exception ex)
                {
                    CustomException cex = ex as CustomException;
                    /* 1. 未知异常
                     * 2. 被标记成不要继续被捕获的的异常
                     * 3. Debug模式下
                     * 上述三种异常，不予捕获，直接甩给Application_DispatcherUnhandledException
                     */
                    if (cex == null || cex.StopCatch || CustomException.DebugMode)
                    {
                        if(cex == null)
                        {
                            Logger.Write("Unexpected exception occurred", "PROJECT", true, LogLevel.Fatal);
                        }
                        throw;
                    }
                    //WARN级别的错误，弹窗提醒
                    if (cex.ErrorLevel == MachineErrorLevel.WARN)
                    {
                        Logger.Write($"WARN Exception occurred:{Environment.NewLine} + {cex}", "PROJECT", true, LogLevel.Warn);
                        ManualErrorHandlingReminder reminder = new ManualErrorHandlingReminder();
                        reminder.ShowOptions(cex.ToString(),cex.HandlingOptions);
                        return;
                    }
                    //ERROR级别的错误，机台状态切换成ERROR。可供用户的选择的操作包含 [IGNORE，RETRY，ABORT]中的两种或三种
                    if(cex.ErrorLevel == MachineErrorLevel.ERROR)
                    {
                        Logger.Write($"ERROR Exception occurred:{Environment.NewLine} + {cex}", "PROJECT", true, LogLevel.Error);
                        _machineStateManager.CurrentMachineState = MachineState.ERROR;
                        ManualErrorHandlingReminder reminder = new ManualErrorHandlingReminder();
                        ErrorHandlingOption option   = reminder.ShowOptions(cex.ToString(), cex.HandlingOptions);
                        Logger.Write($"Operator chose Error Handling Option: [{option}]", "UI", true, LogLevel.Info);
                        if (option == ErrorHandlingOption.IGNORE)
                        {
                            _machineStateManager.CurrentMachineState = _machineStateManager.PreviousMachineState;
                            return;
                        }
                        if (option == ErrorHandlingOption.RETRY)
                        {
                            _machineStateManager.CurrentMachineState = _machineStateManager.PreviousMachineState;
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                            continue;
                        }
                        if (option == ErrorHandlingOption.ABORT)
                        {
                            _machineStateManager.CurrentMachineState = MachineState.ABORT;
                            cex.StopCatch = true;
                            throw cex;
                        }
                    }
                    if (cex.ErrorLevel == MachineErrorLevel.FATAL)
                    {
                        _machineStateManager.CurrentMachineState = MachineState.ABORT;
                        ManualErrorHandlingReminder reminder = new ManualErrorHandlingReminder();
                        reminder.ShowOptions(cex.ToString(), cex.HandlingOptions);
                        cex.StopCatch = true;
                        throw cex;
                    }
                }
            }
        }
    }

    #region 自动重试
    [Serializable]
    [LinesOfCodeAvoided(5)]
    public sealed class AutoRetryAttribute : MethodInterceptionAspect
    {
        public int MaxRetries { get; set; }
        public double Delay { get; set; }

        public AutoRetryAttribute(int maxRetries = 2, double delay = 0.1, params Type[] exceptions)
        {
            MaxRetries = maxRetries;
            Delay = delay;
        }

        public override void OnInvoke(MethodInterceptionArgs args)
        {
            for (int i = 0; ; i++)
            {
                try
                {
                    args.Proceed();
                    return;
                }
                catch (Exception ex)
                {
                    if (CanRetry(i, ex))
                    {
                        if (Delay > 0)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(Delay));
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        private bool CanRetry(int attempt, Exception ex)
        {
            return attempt < MaxRetries && (ex is CustomException);
        }
    }
    #endregion

    internal class Formatter
    {
        public static void GetMethodSignature(StringBuilder stringBuilder, MethodExecutionArgs args)
        {
            stringBuilder.Append(args.Method.DeclaringType.FullName + "." + args.Method.Name + "(");
            ParameterInfo[] pars = args.Method.GetParameters();
            Arguments arguments = args.Arguments;
            for (int i = 0; i < pars.Length; ++i)
            {
                stringBuilder.Append($"{pars[i].ParameterType.Name} {pars[i].Name},");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append(") { ");
            foreach (object item in arguments)
            {
                stringBuilder.Append(item.ToString() + ",");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append("} ");
        }
    }
}