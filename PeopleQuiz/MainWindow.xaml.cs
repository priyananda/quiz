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

            m_quiz = new Model.Quiz();
            m_quiz.Start();

            AddCelebs();
            AddFirstSet();
            AddSecondSet();
            AddThirdSet();
            EnableOnlyPriyanandaQuestions();

            m_quiz.VProps.OnPropertyChange += OnStateChanged;
        }

        private void OnStateChanged(VisualProperties vp)
        {
            if (m_quiz.VProps.ShowThirdSet)
                this.Width = 1540;
            else if (m_quiz.VProps.ShowSecondSet)
                this.Width = 1140;
            else if (m_quiz.VProps.ShowFirstSet)
            {
                EnableFirstSet();
            }
            if (m_quiz.VProps.ShowOnlyWomen)
                EnableOnlyWomenQuestions();
            else
                ApplyKejriwal(m_quiz.VProps.EvenOddState);
            if (m_quiz.VProps.ShowWhitifiedProfile)
                SetBackground("back_murica");
            else if (m_quiz.VProps.ShowJayalalitha)
                SetBackground("back_jaya");
            else if (m_quiz.VProps.ShowMathMode)
                SetBackground("back_math");
            else
                SetBackground("back_roman");
        }

        private void SetBackground(string s)
        {
            var x = backFillRect.Fill;
            var imageSource = new BitmapImage(new Uri(
                String.Format("pack://application:,,,/PeopleQuiz;component/Resources/{0}.jpg", s),
                UriKind.RelativeOrAbsolute));
            backFillRect.Fill = new ImageBrush(imageSource);
        }

        private void AddCelebs()
        {
            int celeb = 0;
            for (int r = 0; r < 4; ++r)
                for (int c = 0; c < 2; ++c)
                {
                    CelebControl control = new CelebControl();
                    control.Person = (Celeb)celeb;
                    Grid.SetRow(control, 1 + r);
                    Grid.SetColumn(control, CELEBS_COL_START + c);
                    this.theGrid.Children.Add(control);
                    ++celeb;
                }
        }

        private void AddFirstSet()
        {
            m_countEnabledQuestions = Questions.QUESTIONS_PER_SET;
            questionControls = new QuestionControl[m_quiz.Questions.Count];
            m_quiz.Questions.QuestionAnswered += OnAnswerQuestion;

            int qid = 1;
            for (int r = 0; r < 4; ++r)
                for (int c = 0; c < 4; ++c, qid++)
                {
                    QuestionControl control = new QuestionControl();
                    control.QuestionId = qid;
                    Grid.SetRow(control, 1 + r);
                    Grid.SetColumn(control, FIRST_SET_COL_START + c);
                    questionControls[qid] = control;
                    this.theGrid.Children.Add(control);
                }
        }

        private void AddSecondSet()
        {
            int qid = m_quiz.Questions.LastInSet + 1;
            int qidMax = m_quiz.Questions.Count;
            for (int r = 1; r <= 4; ++r)
                for (int c = 0; c < 4 && qid < qidMax; ++c, qid++)
                {
                    QuestionControl control = new QuestionControl();
                    control.QuestionId = qid;
                    Grid.SetRow(control, r);
                    Grid.SetColumn(control, SECOND_SET_COL_START + c);
                    questionControls[qid] = control;
                    control.SetMode(false, true);
                    this.theGrid.Children.Add(control);
                }
            
        }

        private void AddThirdSet()
        {
            int qid = Questions.QUESTIONS_PER_SET * 2 + 1;
            for (int r = 1; r <= 4; ++r)
                for (int c = 0; c < 4; ++c)
                {
                    QuestionControl control = new QuestionControl();
                    control.QuestionId = qid;
                    Grid.SetRow(control, r);
                    Grid.SetColumn(control, THIRD_SET_COL_START + c);
                    questionControls[qid++] = control;
                    control.SetMode(false, true);
                    this.theGrid.Children.Add(control);
                }
            
        }

        private void EnableOnlyPriyanandaQuestions()
        {
            EnableQuestions(
                (qid) => (m_quiz.Questions.Get(qid).Person == Person.Priyananda));
        }

        private void EnableFirstSet()
        {
            EnableQuestions((qid) => true);
        }

        private void EnableSecondSet()
        {
            m_quiz.Questions.AdvanceSet();
            m_countEnabledQuestions = Questions.QUESTIONS_PER_SET;
            m_quiz.VProps.ShowJayalalitha = false;
            m_quiz.VProps.ShowMathMode = false;
            m_quiz.MetaManager.ForceFinishAll();
            EnableQuestions((qid) => true);
        }

        private void EnableOnlyWomenQuestions()
        {
            bool fEnableAll = false;

            if (m_quiz.Questions.CountOpenWomen() == 0)
                fEnableAll = true;
            EnableQuestions(
                (qid) => (fEnableAll || PersonTraits.GenderOf(m_quiz.Questions.Get(qid).Person) == GenderType.Female));
        }

        private void ApplyKejriwal(KejriwalState evenOddState)
        {
            bool fEnableAll = false;
            bool fEven = evenOddState == KejriwalState.ShowOnlyEven;

            if (evenOddState == KejriwalState.ShowAll ||
                m_quiz.Questions.CountOpenEvenOdd(fEven) == 0)
                fEnableAll = true;

            EnableQuestions(
                (qid) => (fEnableAll || (qid % 2 == (fEven ? 0 : 1))));
        }

        private void EnableQuestions(Predicate<int> FCheck)
        {
            for (int qid = m_quiz.Questions.FirstInSet; qid <= m_quiz.Questions.LastInSet; ++qid)
            {
                if (questionControls[qid] != null)
                {
                    if (FCheck(qid))
                        questionControls[qid].SetMode(true, false);
                    else
                        questionControls[qid].SetMode(false, true);
                }
            }
        }

        private void OnAnswerQuestion(Question question)
        {
            this.theGrid.Children.Remove(questionControls[question.Id]);
            questionControls[question.Id] = null;
            m_countEnabledQuestions--;
            if (m_quiz.Questions.CurrentSet == 0 && m_countEnabledQuestions == 0)
                EnableSecondSet();
        }

        private void btnEAIS_Click(object sender, RoutedEventArgs e)
        {
            EnableFirstSet();
        }

        private void btnFMMF_Click(object sender, RoutedEventArgs e)
        {
            m_quiz.MetaManager.ForceFinishAll();
        }

        private void btnAdv_Click(object sender, RoutedEventArgs e)
        {
            EnableSecondSet();
        }

        private QuestionControl[] questionControls;
        private int m_countEnabledQuestions;
        private const int CELEBS_COL_START = 1;
        private Shenoy.Quiz.Model.Quiz m_quiz;
        private const int FIRST_SET_COL_START = CELEBS_COL_START + 2;
        private const int SECOND_SET_COL_START = FIRST_SET_COL_START + 4;
        private const int THIRD_SET_COL_START = SECOND_SET_COL_START + 4;
    }
}
