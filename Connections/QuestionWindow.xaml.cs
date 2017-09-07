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
using ConnQuiz.Model;

namespace ConnQuiz.UI
{
    /// <summary>
    /// Interaction logic for QuestionWindow.xaml
    /// </summary>
    public partial class QuestionWindow : Window
    {
        public QuestionWindow(int questionid)
        {
            InitializeComponent();
            this.Left = 40;
            this.Top = 0;

            m_question = Questions.Get(questionid);

            ShowQuestion();
            ShowSet();
        }

        private void ShowQuestion()
        {
            txtQName.Text = m_question.Name;
            txtQPoints.Text = m_question.Points.ToString();
            if (!m_question.Exhaustive)
                imgExh.Source = null;
            if (!m_question.InOrder)
                imgInOrder.Source = null;
            if (!m_question.Silly)
                imgSuperficial.Source = null;
            if (!m_question.Limited)
                imgLimited.Source = null;
            if (m_question.Type == QuestionType.StagedConnect)
            {
                m_currentSet = 0;
                btnNext.IsEnabled = btnPrev.IsEnabled = true;
            }
        }
        private void ShowSet()
        {
            this.cluesPanel.Children.Clear();
            this.m_owsList.Clear();
            m_selectedIndex = -1;

            if (!fShowingAnswer)
            {
                switch (m_question.Type)
                {
                    case QuestionType.Simple:
                        AddClue(((SimpleQuestion)m_question).Clue);
                        break;
                    case QuestionType.Connect:
                        foreach (var clue in (m_question as ConnectQuestion).Clues)
                            AddClue(clue);
                        break;
                    case QuestionType.LongList:
                        foreach (var clue in (m_question as LongListQuestion).Clues)
                            AddClue(clue);
                        break;
                    case QuestionType.StagedConnect:
                        StagedConnectQuestion stg = m_question as StagedConnectQuestion;
                        foreach (var clue in stg.ClueSets[m_currentSet])
                            AddClue(clue);
                        int pnts = stg.PointsForStage(m_currentSet);
                        int npts = (m_currentSet < stg.ClueSets.Length - 1) ? - pnts / 2 : 0;
                        npts -= (npts % 5);
                        txtQPoints.Text = String.Format("+{0}/{1}", pnts, npts);
                        break;
                }
            }
            else
            {
                AddClue(m_question.Ans, true);
                SelectImage(0);
            }
        }
        private void AddClue(Clue clue)
        {
            AddClue(clue, clue.Unlocked);
        }
        private void AddClue(ObjectWithSlide ows, bool fUnlocked)
        {
            BitmapImage bmp = fUnlocked ? MediaManager.GetSlide(ows.SlideId) : MediaManager.UnknownImage;

            Image image = new Image();
            image.Source = bmp;
            image.Width = 120;
            image.Height = 90;
            image.Tag = (int)this.cluesPanel.Children.Count;
            image.MouseLeftButtonDown += new MouseButtonEventHandler(image_MouseLeftButtonDown);

            Border border = new Border();
            border.BorderBrush = Brushes.Transparent;
            border.BorderThickness = new Thickness(1);

            border.Child = image;
            this.cluesPanel.Children.Add(border);

            this.m_owsList.Add(ows);
        }

        private void ShowVideo()
        {
            btnPlay.Visibility = Visibility.Hidden;

            if (m_video != null)
            {
                this.grid.Children.Remove(m_video);
                m_video = null;
                m_fPlaying = false;
            }

            if (m_selectedIndex >= m_owsList.Count || m_owsList[m_selectedIndex].VideoUrl == null)
                return;

            ObjectWithSlide owsCur = m_owsList[m_selectedIndex];

            if (!fShowingAnswer && ! (owsCur as Clue).Unlocked)
                return;

            m_video = new MediaElement();
            m_video.Margin = new Thickness(owsCur.VideoLocation.Left, owsCur.VideoLocation.Top, 0, 0);
            m_video.Width = owsCur.VideoLocation.Width;
            m_video.Height = owsCur.VideoLocation.Height;
            m_video.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            m_video.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            m_video.LoadedBehavior = MediaState.Manual;
            btnPlay.Visibility = Visibility.Visible;

            this.grid.Children.Add(m_video);
            Grid.SetColumn(m_video, 1);
            Grid.SetZIndex(m_video, 2);
            m_video.Source = MediaManager.VideoURL(owsCur.VideoUrl);
            btnPlay.Visibility = Visibility.Visible;
        }

        void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            if (image != null && image.Tag != null)
            {
                SelectImage((int)image.Tag);
            }
        }
        void OnAnswerClick(object sender, MouseButtonEventArgs e)
        {
            m_question.AnswerQuestion();
            fShowingAnswer = true;
            ShowSet();
        }
        void btn_Click(object sender, RoutedEventArgs e)
        {
            StagedConnectQuestion stgq = m_question as StagedConnectQuestion;
            int oldSet = m_currentSet;
            if (sender == btnNext)
            {
                if (!fShowingAnswer && m_currentSet < stgq.ClueSets.Length - 1)
                    m_currentSet++;
            }
            else
            {
                if (fShowingAnswer)
                {
                    fShowingAnswer = false;
                    m_currentSet = stgq.ClueSets.Length - 1;
                }
                else if (m_currentSet > 0)
                    m_currentSet--;
            }
            if (oldSet != m_currentSet)
                ShowSet();
        }
        private void SelectImage(int idx)
        {
            if (m_selectedIndex >= 0)
            {
                Border oldSelection = this.cluesPanel.Children[m_selectedIndex] as Border;
                oldSelection.BorderBrush = Brushes.Transparent;
            }
            m_selectedIndex = idx;
            Border newSelection = this.cluesPanel.Children[m_selectedIndex] as Border;
            newSelection.BorderBrush = Brushes.Black;

            Image newImage = newSelection.Child as Image;
            contentArea.Source = newImage.Source;

            ShowVideo();
        }
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (m_video != null)
            {
                if (m_fPlaying)
                    m_video.Pause();
                else
                    m_video.Play();
                m_fPlaying = !m_fPlaying;
            }
        }

        private int m_selectedIndex;
        private int m_currentSet;
        private Question m_question;
        private bool fShowingAnswer;
        private MediaElement m_video;
        private bool m_fPlaying;
        private List<ObjectWithSlide> m_owsList = new List<ObjectWithSlide>();
    }
}
