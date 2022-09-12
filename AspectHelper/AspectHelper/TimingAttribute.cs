using MethodBoundaryAspect.Fody.Attributes;

namespace AspectHelper
{
    // 用于对方法计时，统计方法的执行时间
    public class TimingAttribute : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs arg)
        {
            base.OnEntry(arg);
        }
    }
}