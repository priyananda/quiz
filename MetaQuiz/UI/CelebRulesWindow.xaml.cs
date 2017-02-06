using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Shenoy.Quiz.Model;

namespace Shenoy.Quiz.UI
{
    /// <summary>
    /// Interaction logic for QuestionWindow.xaml
    /// </summary>
    public partial class CelebRulesWindow : Window
    {
        public CelebRulesWindow(Celeb celeb)
        {
            InitializeComponent();
            this.Left = 40;
            this.Top = 0;

            celebControl.Person = celeb;
            celebControl.SetMode(false, false);

            ShowCelebRuleSlide();
       }

        private void ShowCelebRuleSlide()
        {
            contentArea.Source = MediaManager.GetCelebRuleSlide(celebControl.Person);
        }
    }
}
