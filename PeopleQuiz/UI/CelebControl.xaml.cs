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
        }

        public Celeb Person
        {
            get { return m_celeb; }
            set
            {
                m_celeb = value;
                this.lblPerson.Content = m_celeb.ToString();
                this.imgPerson.Source = MediaManager.GetPerson(m_celeb);
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
            this.SetMode(false, true);
        }

        private Celeb m_celeb = Celeb._Max;
    }
}
