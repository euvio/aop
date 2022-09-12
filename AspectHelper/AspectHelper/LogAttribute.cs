using MethodBoundaryAspect.Fody.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspectHelper
{
    // 打印入口和出口参数
    public class LogAttribute:OnMethodBoundaryAspect
    {
    }
}
