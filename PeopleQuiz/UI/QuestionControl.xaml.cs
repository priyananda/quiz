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
            InitializeComponent();
        }

        public int QuestionId
        {
            get { return m_qid; }
            set
            {
                m_qid = value;
                Question question = Questions.Get(m_qid);
                this.lblPerson.Content = question.Person.ToString();
                if (!question.AllPlay)
                    imgIcon.Visibility = System.Windows.Visibility.Hidden;
                switch (question.Difficulty)
                {
                    case QuestionDifficulty.Red: imgDifficulty.Fill = Brushes.Red; break;
                    case QuestionDifficulty.Yellow: imgDifficulty.Fill = Brushes.Yellow; break;
                    case QuestionDifficulty.Green: imgDifficulty.Fill = Brushes.Green; break;
                }
                MetaModifierState.Current.OnStateChanged += SetProperties;
                //SetProperties(MetaModifierState.Current, MetaModifiers.Max);
            }
        }

        private void SetProperties(MetaModifierState mmstate, Celeb mm)
        {
            bool fRoman = false;// mmstate.IsEnabled(MetaModifiers.RomanTheme);
            this.lblQuestionNumber.Content = fRoman ?  RomanNumbers.Convert(m_qid) : m_qid.ToString();
            if (fRoman)
                this.FontFamily = new FontFamily("Papyrus");
            this.imgPerson.Source = MediaManager.GetPerson(Questions.Get(m_qid).Person);
            //imgDifficulty.Visibility = mmstate.IsEnabled(MetaModifiers.RedYellowGreen) ?
             //   Visibility.Visible : Visibility.Hidden;
        }

        public void SetMode(bool fEnabled, bool fGreyedOut)
        {
            this.IsEnabled = fEnabled;
            imgPerson.Opacity = !fGreyedOut ? 1.0 : .5;
        }

        private void HandleClick(object sender, MouseButtonEventArgs e)
        {
            new QuestionWindow(m_qid).ShowDialog();
        }

        private int m_qid;
    }
}
