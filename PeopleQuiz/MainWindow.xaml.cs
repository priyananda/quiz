using Shenoy.Quiz.Model;
using Shenoy.Quiz.UI;
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

namespace Shenoy.Quiz
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AddFirstSet();
            MetaModifierState.Current.OnStateChanged += OnStateChanged;
            //for (int q = 1; q <= 30; ++q)
            //{
            //    Questions.Get(q).Open();
            //    Questions.Get(q).AnswerQuestion();
            //}
        }

        private void OnStateChanged(MetaModifierState mmstate, MetaModifiers mm)
        {
            if (mm == MetaModifiers.RomanTheme)
            {
                var x = backFillRect.Fill;
                var imageSource = new BitmapImage(new Uri("pack://application:,,,/PeopleQuiz;component/Resources/back_roman.jpg", UriKind.RelativeOrAbsolute));
                backFillRect.Fill = new ImageBrush(imageSource);
            }
            else if (mm == MetaModifiers.DoublesDuration)
            {
                AddSecondSet();
            }
            listboxMM.Items.Add(mm.ToString());
        }

        private void AddFirstSet()
        {
            m_isFirstSet = true;
            m_countEnabledQuestions = Questions.FirstSetCount;
            questionControls = new QuestionControl[Questions.Count];
            int qid = 1;
            for (int r = 0; r < 4; ++r)
                for (int c = 0; c < 4; ++c, qid++)
                {
                    QuestionControl control = new QuestionControl();
                    Questions.Get(qid).Answered += OnAnswerQuestion;
                    control.QuestionId = qid;
                    Grid.SetRow(control, 1 + r);
                    Grid.SetColumn(control, 1 + c);
                    questionControls[qid] = control;
                    this.theGrid.Children.Add(control);
                }
        }

        private void AddSecondSet()
        {
            int qid = Questions.FirstSetCount + 1;
            int qidMax = Questions.Count;
            for (int r = 1; r <= 4; ++r)
                for (int c = 5; c <= 8 && qid < qidMax; ++c, qid++)
                {
                    QuestionControl control = new QuestionControl();
                    Questions.Get(qid).Answered += OnAnswerQuestion;
                    control.QuestionId = qid;
                    Grid.SetRow(control, r);
                    Grid.SetColumn(control, c);
                    questionControls[qid] = control;
                    control.SetMode(false, true);
                    this.theGrid.Children.Add(control);
                }
            this.Width = 1060;
        }

        private void EnableSecondSet()
        {
            m_isFirstSet = false;
            m_countEnabledQuestions = Questions.SecondSetCount;
            int qidMax = Questions.Count;
            for (int qid = Questions.FirstSetCount + 1; qid < qidMax; ++qid)
                questionControls[qid].SetMode(true, false);
        }

        private void OnAnswerQuestion(Question question)
        {
            this.theGrid.Children.Remove(questionControls[question.Id]);
            questionControls[question.Id] = null;
            m_countEnabledQuestions--;
            if (m_isFirstSet && m_countEnabledQuestions == 0)
                EnableSecondSet();
        }
        private QuestionControl[] questionControls;
        private bool m_isFirstSet;
        private int m_countEnabledQuestions;
    }
}
