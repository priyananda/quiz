using System.Windows;
using ConnQuiz.Model;
using System.Windows.Media.Imaging;

namespace ConnQuiz.UI
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            
            this.Left = 40;
            this.Top = 0;
            //this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            //this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 100;
            this.Width = 1024;
            this.Height = 768 - 100;

            Questions.Load("qdata.xml");
            Questions.DirectionChange += new System.Action(Questions_DirectionChange);
            this.Closing += new System.ComponentModel.CancelEventHandler(Window1_Closing);
            //LayoutGen.Generate("diagram.xml");
            MyDesigner.Open_Executed(null, null);
        }

        void Window1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Really Close?", "Quiz 114 App", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                e.Cancel = true;
        }

        void Questions_DirectionChange()
        {
            imgDirection.Source = MediaManager.LoadImage("Resources\\Images\\aclock.png", true);
        }
    }
}
