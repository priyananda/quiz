using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Shenoy.Question.UI
{
    public interface IQuestionUIExtension : IDisposable
    {
        UserControl UserControl { get; }
        void RegisterWindow(Window window);
    }
}
