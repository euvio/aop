
using System.Windows;
using System.Threading;

namespace Gocool.Error
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    partial class ErrorWindow : Window
    {
        AutoResetEvent _waiter = new AutoResetEvent(false);
        /// <summary>
        /// 区别调用Close()关闭窗体还是点击窗口右上角的[X]关闭窗体
        /// </summary>
        bool _isClickedOptionButton = false;
        internal ErrorHandlingOption option = ErrorHandlingOption.ABORT;

        internal ErrorWindow()
        {
            InitializeComponent();
        }

        internal void Show(string info, string options, AutoResetEvent waiter)
        {
            _waiter = waiter;
            tbkErrorContent.Text = info;
            spOption.Children.Clear();
            if(options == "000")
            {
                spOption.Children.Add(btnShowWarn);
                option = ErrorHandlingOption.IGNORE;
                _isClickedOptionButton = true;
                _waiter.Set();

            }
            else if(options == "001")
            {
                spOption.Children.Add(btnShowAbort);
                option = ErrorHandlingOption.ABORT;
                _isClickedOptionButton = true;
                _waiter.Set();
            }
            else
            {
                if (options[0] == '1')
                {
                    spOption.Children.Add(btnIgnore);
                }
                if (options[1] == '1')
                {
                    spOption.Children.Add(btnRetry);
                }
                if (options[2] == '1')
                {
                    spOption.Children.Add(btnAbort);
                }
            }
            base.Show();
        }

        private void btnIgnore_Click(object sender, RoutedEventArgs e)
        {
            option = ErrorHandlingOption.IGNORE;
            _isClickedOptionButton = true;
            _waiter.Set();
            this.Close();
        }

        private void btnRetry_Click(object sender, RoutedEventArgs e)
        {
            option = ErrorHandlingOption.RETRY;
            _isClickedOptionButton = true;
            _waiter.Set();
            this.Close();
        }

        private void btnAbort_Click(object sender, RoutedEventArgs e)
        {
            option = ErrorHandlingOption.ABORT;
            _isClickedOptionButton = true;
            _waiter.Set();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_isClickedOptionButton)
            {
                e.Cancel = true;
            }
        }

        private void btnShowAbort_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnShowWarn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
