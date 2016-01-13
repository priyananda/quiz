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
    public partial class CelebControl : UserControl
    {
        public CelebControl()
        {
            InitializeComponent();
            Quiz.Model.Quiz.Current.VProps.OnPropertyChange += OnPropertyChange;
        }

        private void OnPropertyChange(VisualProperties obj)
        {
            bool fEnabled = true;
            if (m_celeb == Celeb.GRRM || m_celeb == Celeb.KyloRen)
                fEnabled = false;
            else if (m_celeb == Celeb.Kejriwal)
                fEnabled = !Quiz.Model.Quiz.Current.VProps.ShowOnlyWomen;
            else if (m_celeb == Celeb.RahulG)
                fEnabled = (Quiz.Model.Quiz.Current.VProps.EvenOddState == KejriwalState.ShowAll);
            SetMode(fEnabled, !fEnabled);
        }

        public Celeb Person
        {
            get { return m_celeb; }
            set
            {
                m_celeb = value;
                this.lblPerson.Content = m_celeb.ToString();
                this.imgPerson.Source = MediaManager.GetPerson(m_celeb);
                if (m_celeb == Celeb.GRRM || m_celeb == Celeb.KyloRen)
                    SetMode(false, true);
            }
        }

        public void SetMode(bool fEnabled, bool fGreyedOut)
        {
            this.IsEnabled = fEnabled;
            imgPerson.Opacity = !fGreyedOut ? 1.0 : .5;
        }

        private void HandleClick(object sender, MouseButtonEventArgs e)
        {
            Model.Quiz.Current.MetaManager.Activate(m_celeb);
            var grid = (this.Parent as Grid);
            grid.Children.Remove(this);
            Quiz.Model.Quiz.Current.VProps.OnPropertyChange -= OnPropertyChange;
            new CelebRulesWindow(m_celeb).Show();
        }

        private Celeb m_celeb = Celeb._Max;
    }
}
