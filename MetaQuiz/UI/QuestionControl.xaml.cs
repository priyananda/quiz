using Shenoy.Quiz.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Shenoy.Quiz.UI
{
    public partial class QuestionControl : UserControl
    {
        public QuestionControl()
        {
            m_quiz = Quiz.Model.Quiz.Current;
            InitializeComponent();
        }

        public int QuestionId
        {
            get { return m_qid; }
            set
            {
                m_qid = value;
                Question question = m_quiz.Questions.Get(m_qid);
                SetProperties(VisualProperties.Max);
            }
        }

        private void SetProperties(VisualProperties vp)
        {
            this.lblQuestionNumber.Content = m_qid.ToString();
            this.lblSymbol.Content = m_quiz.Questions.Get(m_qid).Symbol;
        }

        public void SetMode(bool fEnabled, bool fGreyedOut)
        {
            this.IsEnabled = fEnabled;
            if (fEnabled)
                lblSymbol.Foreground = Brushes.Black;
            else
                lblSymbol.Foreground = Brushes.Transparent;
        }

        private void HandleClick(object sender, MouseButtonEventArgs e)
        {
            new QuestionWindow(Model.Quiz.Current, m_qid).Show();
            //m_quiz.Questions.Get(m_qid).AnswerQuestion();
        }

        private int m_qid;
        private Model.Quiz m_quiz;
        private static Random m_random = new Random();
    }
}
