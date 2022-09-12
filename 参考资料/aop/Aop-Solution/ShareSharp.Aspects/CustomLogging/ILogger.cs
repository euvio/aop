using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareSharp.Aspects.CustomLogging
{
    public interface ILogger
    {
        void Debug(string content, string module);
        void Info(string content, string module);
        void Warn(string content, string module);
        void Error(string content, string module);
        void Fatal(string content, string module);
    }
}
