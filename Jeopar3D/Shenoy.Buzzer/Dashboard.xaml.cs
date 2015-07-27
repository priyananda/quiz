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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Shenoy.Game.Buzzer
{
    public partial class Dashboard : UserControl, IDisposable
    {
        public Dashboard()
        {
            InitializeComponent();

            m_icons = new Ellipse[]{
                team1,
                team2,
                team3,
                team4,
                team5,
                team6,
            };
        }

        public void RegisterWindow(Window window)
        {
            WindowInteropHelper helper = new WindowInteropHelper(window);
            m_inputdevice = new BuzzerInputDevice(helper.Handle);
            m_inputdevice.ButtonPressed += OnButtonPress;
            m_inputdevice.Connect();
            m_inputdevice.NumControllersEnabled = 6;
            m_inputdevice.Enable();
        }

        public void Dispose()
        {
            m_inputdevice.ClearEverthing();
        }

        private void OnButtonPress(int controller, BuzzerButton button)
        {
            if (controller >= 6)
                return;
            m_icons[controller].Fill = Brushes.Red;
            m_inputdevice.LockController(controller);
            m_inputdevice.Disable();
            FixIconStates();
        }

        private void btnUnlock_Click(object sender, RoutedEventArgs e)
        {
            m_inputdevice.Enable();
            FixIconStates();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            m_inputdevice.UnlockAllControllers();
            btnUnlock_Click(sender, e);
        }

        private void FixIconStates()
        {
            int count = m_inputdevice.NumControllersEnabled;
            for(int icont = 0; icont < count; ++icont)
            {
                if (m_inputdevice.IsControllerLocked(icont))
                    m_icons[icont].Fill = Brushes.Red;
                else if (m_inputdevice.State == BuzzerState.Accepting)
                    m_icons[icont].Fill = Brushes.Yellow;
                else
                    m_icons[icont].Fill = Brushes.LightGray;
            }
        }

        private BuzzerInputDevice m_inputdevice;
        private Ellipse[] m_icons;
    }
}
