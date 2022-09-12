using System.Threading;
using System.Windows;

namespace Gocool.Error
{
    /// <summary>
    /// 可视化的错误处理弹窗
    /// </summary>
    public class ManualErrorHandlingReminder
    {
        ErrorWindow _mainWindow = null;
        AutoResetEvent _waiter = new AutoResetEvent(false);

        public ErrorHandlingOption ShowOptions(string info, string errorOptions)
        {
            Show(info, errorOptions);
            return Wait();
        }

        /// <summary>
        /// 弹窗：错误信息 + 错误处理选项
        /// </summary>
        /// <param name="info"></param>
        /// <param name="errorLevel"></param>
        private void Show(string info, string errorOptions)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _mainWindow = new ErrorWindow();
                _mainWindow.Show(info, errorOptions, _waiter);
            });
        }

        /// <summary>
        /// 等待用户选择
        /// </summary>
        /// <returns></returns>
        private ErrorHandlingOption Wait()
        {
            ErrorHandlingOption option = ErrorHandlingOption.ABORT;
            _waiter.WaitOne();
            Application.Current.Dispatcher.Invoke(() =>
            {
                option = _mainWindow.option;
            });
            return option;
        }
    }
}