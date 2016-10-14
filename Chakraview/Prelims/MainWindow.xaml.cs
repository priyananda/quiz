using Shenoy.Quiz.Backend;
using Shenoy.Quiz.Connector;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Shenoy.Quiz
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CalculateFinalistsPositions();
        }

        private void CalculateFinalistsPositions()
        {
            double centerX = bgCanvas.Width / 2 - 30;
            double centerY = bgCanvas.Height / 2 - 30;
            double radius = 100;
            int teamCount = 6;

            for (int team = 0; team < teamCount; ++team)
            {
                double angle = 2.0 * team * (Math.PI / teamCount);
                double teamX = centerX + radius * Math.Sin(angle);
                double teamY = centerY + radius * Math.Cos(angle);

                m_winnerLocations.Add(new Point(teamX, teamY));
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            Random random = new Random();
            foreach(UIElement child in bgCanvas.Children)
            {
                double deltaX = (random.NextDouble() - 0.5) * 5.0;
                double deltaY = (random.NextDouble() - 0.5) * 5.0;
                Canvas.SetLeft(child, Canvas.GetLeft(child) + deltaX);
                Canvas.SetTop(child, Canvas.GetTop(child) + deltaY);
            }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_dispatcherTimer != null)
                m_dispatcherTimer.Stop();

            m_teams = PrelimsService.GetTeams();
            if (m_teams == null || m_teams.Count == 0)
                return;

            bgCanvas.Children.Clear();

            double centerX = bgCanvas.Width / 2 - 30;
            double centerY = bgCanvas.Height / 2 - 30;
            double radius = 280;
            int teamCount = m_teams.Count;
            bool anyFinalists = false;

            for (int team = 0; team < teamCount; ++team)
            {
                double teamRadius = radius - (team % 2) * 30;
                double angle = 2.0 * team * (Math.PI / teamCount);
                double teamX = centerX + teamRadius * Math.Sin(angle);
                double teamY = centerY + teamRadius * Math.Cos(angle);

                Ellipse ellipse = new Ellipse();
                ellipse.Fill = Brushes.AliceBlue;
                ellipse.Stroke = Brushes.Black;
                ellipse.Width = 40;
                ellipse.Height = 40;

                TextBlock textblock = new TextBlock();
                textblock.Text = m_teams[team].TeamId.ToString();
                textblock.TextAlignment = TextAlignment.Center;
                textblock.VerticalAlignment = VerticalAlignment.Center;
                textblock.FontWeight = FontWeights.Bold;

                Grid grid = new Grid();
                grid.Children.Add(ellipse);
                grid.Children.Add(textblock);

                Canvas.SetLeft(grid, teamX);
                Canvas.SetTop(grid, teamY);
                bgCanvas.Children.Add(grid);

                if (m_teams[team].IsFinalist)
                {
                    anyFinalists = true;
                    m_winners.Add(grid);
                }
            }

            nextWinnerButton.Visibility = anyFinalists ? Visibility.Visible : Visibility.Hidden;

            if (!anyFinalists)
            {
                if (m_dispatcherTimer == null)
                {
                    m_dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                    m_dispatcherTimer.Tick += new EventHandler(OnTimerTick);
                    m_dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
                }
                m_dispatcherTimer.Start();
            }
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_winners.Count == 0 || m_winnerLocations.Count == 0)
                return;
            UIElement element = m_winners[0];
            m_winners.RemoveAt(0);

            Point destLoc = m_winnerLocations[0];
            m_winnerLocations.RemoveAt(0);

            AnimateElement(element, destLoc);
        }

        private void AnimateElement(UIElement element, Point destLoc)
        {
            var animX = new DoubleAnimation();
            animX.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            animX.From = Canvas.GetLeft(element);
            animX.To = destLoc.X;

            var animY = new DoubleAnimation();
            animY.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            animY.From = Canvas.GetTop(element);
            animY.To = destLoc.Y;

            element.BeginAnimation(Canvas.LeftProperty, animX);
            element.BeginAnimation(Canvas.TopProperty, animY);
        }

        DispatcherTimer m_dispatcherTimer;
        List<TeamInfo> m_teams;
        List<UIElement> m_winners = new List<UIElement>();
        List<Point> m_winnerLocations = new List<Point>();
    }
}
