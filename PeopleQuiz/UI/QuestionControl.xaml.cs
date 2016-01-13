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
                if (!question.AllPlay)
                    imgIcon.Visibility = System.Windows.Visibility.Hidden;
                switch (question.Difficulty)
                {
                    case QuestionDifficulty.Red: imgDifficulty.Fill = Brushes.Red; break;
                    case QuestionDifficulty.Yellow: imgDifficulty.Fill = Brushes.Yellow; break;
                    case QuestionDifficulty.Green: imgDifficulty.Fill = Brushes.Green; break;
                }
                m_quiz.VProps.OnPropertyChange += SetProperties;
                SetProperties(VisualProperties.ShowJayalalitha);
            }
        }

        private void SetProperties(VisualProperties vp)
        {
            if (m_quiz.VProps.ShowMathMode)
                this.lblQuestionNumber.Content = GreekNumbers.Convert(m_qid);
            else
                this.lblQuestionNumber.Content = m_qid.ToString();
            if (m_quiz.VProps.ShowJayalalitha)
                this.imgPerson.Source = MediaManager.GetPerson(Celeb.Jayalalitha);
            else
                this.imgPerson.Source = MediaManager.GetPerson(m_quiz.Questions.Get(m_qid).Person);
            imgDifficulty.Visibility = m_quiz.VProps.ShowTrafficLights ?
                Visibility.Visible : Visibility.Hidden;
            if (m_quiz.VProps.ShowWhitifiedProfile)
                this.lblPerson.Content = PersonTraits.GetWhitifiedName(m_quiz.Questions.Get(m_qid).Person);
            else
                this.lblPerson.Content = m_quiz.Questions.Get(m_qid).Person.ToString();

        }

        public void SetMode(bool fEnabled, bool fGreyedOut)
        {
            this.IsEnabled = fEnabled;
            imgPerson.Opacity = !fGreyedOut ? 1.0 : .5;
        }

        private void HandleClick(object sender, MouseButtonEventArgs e)
        {
            new QuestionWindow(Model.Quiz.Current, m_qid).Show();
        }

        private int m_qid;
        private Model.Quiz m_quiz;
    }
}
