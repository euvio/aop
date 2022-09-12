using System;
using System.Windows;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TongfuInn.Log;

namespace Gocool.Error
{
    public sealed class CustomException : Exception
    {
        public static bool DebugMode { get; set; }

        public bool StopCatch { get; set; }

        public string Module { get; }
        public string HandlingOptions { get; }
        public MachineErrorLevel ErrorLevel { get; }
        public string ErrorDetail { get; }
        public string ErrorSuggest { get; }


        public CustomException(string errorDetail, string handlingOptions, string module, string errorSuggest = "")
        {
            if (handlingOptions.Length != 3)
            {
                throw new ArgumentException($"错误选项的格式是字符0和1构成的长度为3的字符串，参数[{handlingOptions}]不符合此格式.");
            }
            foreach (char item in handlingOptions)
            {
                if(item != '0' && item != '1')
                {
                    throw new ArgumentException($"错误选项的格式是字符0和1构成的长度为3的字符串，参数[{handlingOptions}]不符合此格式.");
                }
            }

            Module = module;
            HandlingOptions = handlingOptions;
            ErrorLevel = GetErrorLevel(handlingOptions);
            ErrorDetail = errorDetail;
            ErrorSuggest = errorSuggest;
        }

    
        public static void HandleException(Exception ex)
        {
            CustomException cex = ex as CustomException;
            if (cex != null)
            {
                Logger.Write(cex.ToString(), "调试模式", false, LogLevel.Error);
                MessageBox.Show(ex.ToString());
            }
            else
            {
                Logger.Write($"Exception Type:{ex.GetType()},{Environment.NewLine}" +
                    $"Exception Message:{ex.Message},{Environment.NewLine}," +
                    $"Exception Stack:{ex.StackTrace}", "调试模式",
                    false, LogLevel.Error);
                MessageBox.Show($"Exception Type:{ex.GetType()},{Environment.NewLine}" +
                    $"Exception Message:{ex.Message},{Environment.NewLine}," +
                    $"Exception Stack:{ex.StackTrace}");
            }
        }

        private MachineErrorLevel GetErrorLevel(string options)
        {
            if (options == "000")
            {
                return MachineErrorLevel.WARN;
            }
            else if (options == "001")
            {
                return MachineErrorLevel.FATAL;
            }
            else
            {
                return MachineErrorLevel.ERROR;
            }
        }

        public override string ToString()
        {
            return
                $"错误信息如下: " + Environment.NewLine +
                $"(1)错误模块: {Module}" + Environment.NewLine +
                $"(2)错误等级: {ErrorLevel.ToString()}" + Environment.NewLine +
                $"(3)错误详情: {ErrorDetail}" + Environment.NewLine +
                $"(4)纠错建议: {ErrorSuggest}" + Environment.NewLine;
        }
    }

    public enum MachineErrorLevel
    {
        FATAL,
        ERROR,
        WARN
    }
}
