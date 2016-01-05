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
    public partial class QuestionWindow : Window
    {
        public QuestionWindow(Shenoy.Quiz.Model.Quiz quiz, int questionid)
        {
            InitializeComponent();
            this.Left = 40;
            this.Top = 0;

            m_question = quiz.Questions.Get(questionid);
            m_question.Open();

            questionControl.QuestionId = questionid;
            questionControl.SetMode(false, false);

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
            if (m_question.Type == QuestionType.StagedConnect)
            {
                m_currentSet = 0;
                btnNext.IsEnabled = btnPrev.IsEnabled = true;
            }
        }
        private void ShowSet()
        {
            this.cluesPanel.Children.Clear();
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
                for (int slideId = m_question.Ans.SlideRangeStart; slideId <= m_question.Ans.SlideRangeEnd; ++slideId)
                    AddClue(slideId, true);
                SelectImage(0);
            }
        }
        private void AddClue(Clue clue)
        {
            AddClue(clue.SlideId, clue.Unlocked);
        }
        private void AddClue(int slideid, bool fUnlocked)
        {
            BitmapImage bmp = fUnlocked ? MediaManager.GetSlide(slideid) : MediaManager.UnknownImage;

            Image image = new Image();
            image.Source = bmp;
            image.Width = 120;
            image.Height = 90;
            image.Tag = (int)this.cluesPanel.Children.Count;
            image.MouseLeftButtonDown += new MouseButtonEventHandler(OnThumbnailClick);

            Border border = new Border();
            border.BorderBrush = Brushes.Transparent;
            border.BorderThickness = new Thickness(1);

            border.Child = image;
            this.cluesPanel.Children.Add(border);
        }
        private Clue GetCurrentClue()
        {
            if (fShowingAnswer || m_currentSet < 0 || m_selectedIndex < 0) return null;
            switch (m_question.Type)
            {
                case QuestionType.Simple: return (m_question as SimpleQuestion).Clue;
                case QuestionType.Connect: return (m_question as ConnectQuestion).Clues[m_selectedIndex];
                case QuestionType.LongList: return (m_question as LongListQuestion).Clues[m_selectedIndex];
                case QuestionType.StagedConnect: return (m_question as StagedConnectQuestion).ClueSets[m_currentSet][m_selectedIndex];
            }
            return null;
        }
        private void SetupAudioVideo()
        {
            if (video != null)
            {
                this.grid.Children.Remove(video);
                video = null;
                return;
            }
            Clue clue = GetCurrentClue();
            if (clue != null && clue.AVData != null)
            {
                AVNode av = clue.AVData;
                video = new MediaElement();
                video.Margin = new Thickness(av.X, av.Y, 0, 0);
                video.Width = av.W;
                video.Height = av.H;
                video.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                video.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                this.grid.Children.Add(video);
                Grid.SetColumn(video, 1);
                Grid.SetZIndex(video, 2);
            }
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

            SetupAudioVideo();
        }

        private void OnThumbnailClick(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            if (image != null && image.Tag != null)
            {
                SelectImage((int)image.Tag);
            }
        }
        private void OnAnswerClick(object sender, MouseButtonEventArgs e)
        {
            m_question.AnswerQuestion();
            fShowingAnswer = true;
            ShowSet();
        }
        private void OnNavigateButtonClicked(object sender, RoutedEventArgs e)
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

        private int m_selectedIndex;
        private int m_currentSet;
        private Question m_question;
        private bool fShowingAnswer;
        private MediaElement video;
    }
}
