using Shenoy.Game.Buzzer;
using Shenoy.Question.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RK.Wpf3DSampleBrowser.UI
{
    class QuestionBuzzerDashboardExt : IQuestionUIExtension
    {
        Dashboard m_dashboard = new Dashboard();
        public System.Windows.Controls.UserControl UserControl
        {
            get { return m_dashboard; }
        }

        public void RegisterWindow(System.Windows.Window window)
        {
            m_dashboard.RegisterWindow(window);
        }

        public void Dispose()
        {
            m_dashboard.Dispose();
        }
    }
}
