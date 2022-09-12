using MethodBoundaryAspect.Fody.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspectHelper
{
    // 手动重试，点击弹窗按钮选择如何处理故障
    public class ManualRetryAttribute : OnMethodBoundaryAspect
    {
    }
}
