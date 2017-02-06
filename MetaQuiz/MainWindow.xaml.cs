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
            questionControls = new QuestionControl[39];
            int index = 1;

            for (int r = 0; r < 6; ++r)
            {
                for (int c = 0; c < 9; ++c)
                {
                    var key = new Tuple<int, int>(r, c);
                    if (!m_loc2qid.ContainsKey(key))
                        continue;
                    int qid = m_loc2qid[key];
                    QuestionControl control = new QuestionControl();
                    questionGrid.Children.Add(control);
                    Grid.SetRow(control, r);
                    Grid.SetColumn(control, c);
                    questionControls[index++] = control;
                    control.SetMode(false, false);
                    control.QuestionId = qid;
                    m_quiz.Questions.QuestionAnswered += OnQuestionAnswered;
                }
            }
            EnableQuestions(new List<int>() { 26, 27, 28, 29, 30, 31 });
        }

        private void OnQuestionAnswered(Question obj)
        {
            switch (obj.Id)
            {
                case 26:
                    ctopic1.Visibility = ctopic2.Visibility = Visibility.Visible;
                    EnableQuestions(new List<int>() { 2, 7, 8, 16, 17, 25});break;
                case 27:
                    EnableQuestions(new List<int>() { 1, 3, 9, 18, 35, 34}); break;
                case 28:
                    jtopic1.Visibility = jtopic2.Visibility = jtopic3.Visibility = Visibility.Visible;
                    EnableQuestions(new List<int>() { 4,5,6,10,11,12,19,20,21 }); break;
                default:
                    {
                        int thisQuestionIndex = Array.IndexOf(longConnectOrdering, obj.Id);
                        if (thisQuestionIndex >= 0 && thisQuestionIndex < longConnectOrdering.Length - 1)
                            EnableQuestions(new List<int>() { longConnectOrdering[thisQuestionIndex + 1] });
                        break;
                    }
            }
            if (questionControls[obj.Id] != null)
            {
                questionControls[obj.Id].Visibility = Visibility.Hidden;
            }
        }

        private void EnableQuestions(List<int> questions){
            foreach(int i in questions)
            {
                if (questionControls[i] != null)
                    questionControls[i].SetMode(true, true);
            }
        }

        private Dictionary<Tuple<int, int>, int> m_loc2qid = new Dictionary<Tuple<int, int>, int>()
        {
            {new Tuple<int, int>(0, 2), 1},
            {new Tuple<int, int>(1, 1), 2},
            {new Tuple<int, int>(1, 2), 3},
            {new Tuple<int, int>(1, 3), 4},
            {new Tuple<int, int>(1, 4), 5},
            {new Tuple<int, int>(1, 5), 6},

            {new Tuple<int, int>(2, 0), 7},
            {new Tuple<int, int>(2, 1), 8},
            {new Tuple<int, int>(2, 2), 9},
            {new Tuple<int, int>(2, 3), 10},
            {new Tuple<int, int>(2, 4), 11},
            {new Tuple<int, int>(2, 5), 12},
            {new Tuple<int, int>(2, 6), 13},
            {new Tuple<int, int>(2, 7), 14},
            {new Tuple<int, int>(2, 8), 15},

            {new Tuple<int, int>(3, 0), 16},
            {new Tuple<int, int>(3, 1), 17},
            {new Tuple<int, int>(3, 2), 18},
            {new Tuple<int, int>(3, 3), 19},
            {new Tuple<int, int>(3, 4), 20},
            {new Tuple<int, int>(3, 5), 21},
            {new Tuple<int, int>(3, 6), 22},
            {new Tuple<int, int>(3, 7), 23},
            {new Tuple<int, int>(3, 8), 24},

            {new Tuple<int, int>(4, 0), 25},
            {new Tuple<int, int>(4, 1), 26},
            {new Tuple<int, int>(4, 2), 27},
            {new Tuple<int, int>(4, 3), 28},
            {new Tuple<int, int>(4, 4), 29},
            {new Tuple<int, int>(4, 5), 30},
            {new Tuple<int, int>(4, 6), 31},
            {new Tuple<int, int>(4, 7), 32},
            {new Tuple<int, int>(4, 8), 33},

            {new Tuple<int, int>(5, 1), 34},
            {new Tuple<int, int>(5, 2), 35},
        };

        private int[] longConnectOrdering = new int[]
        {
            31, 32, 33, 24,23,22,13,14,15
        };

        private QuestionControl[] questionControls;
        private Shenoy.Quiz.Model.Quiz m_quiz;
    }
}
