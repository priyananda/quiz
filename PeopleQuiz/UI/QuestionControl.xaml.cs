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
                SetProperties(VisualProperties.Max);
            }
        }

        private void SetProperties(VisualProperties vp)
        {
            Person person = m_quiz.Questions.Get(m_qid).Person;
            this.imgPerson.Source = MediaManager.GetPerson(person, m_quiz.VProps.ShowMoustache);
            this.lblPerson.Content = person.ToString();
            this.lblQuestionNumber.Content = m_qid.ToString();
            this.imgHand.Visibility = m_quiz.VProps.ShowHand && person != Person.Parth ? Visibility.Visible : Visibility.Hidden;
            this.imgDifficulty.Visibility = Visibility.Hidden;
            bool fShowMark = m_quiz.VProps.ShowMark && (m_random.Next() % 2 == 0);
            this.imgXMark.Visibility = fShowMark ? Visibility.Visible : Visibility.Hidden;
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
        private static Random m_random = new Random();
    }
}
