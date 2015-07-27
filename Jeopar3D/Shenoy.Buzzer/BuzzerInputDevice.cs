using SlimDX;
using SlimDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Shenoy.Game.Buzzer
{
    public enum BuzzerState
    {
        NotConnected,
        Disabled,
        Accepting
    }

    public enum BuzzerButton
    {
        Red,
        Yellow,
        Green,
        Orange,
        Blue
    }

    public class BuzzerInputDevice
    {
        public BuzzerInputDevice(IntPtr hwnd)
        {
            m_hwnd = hwnd;
            m_timer.Interval = TimeSpan.FromMilliseconds(150);
            m_timer.Tick += new EventHandler(OnTimerTick);
        }

        public delegate void ButtonPressedEventHandler(int controller, BuzzerButton button);
        public event ButtonPressedEventHandler ButtonPressed;

        public delegate void StateChangedEventHandler(BuzzerState newState);
        public event StateChangedEventHandler StateChanged;

        public int NumControllers
        {
            get { return m_numControllers; }
        }

        public int NumControllersEnabled
        {
            get { return m_numControllersEnabled; }
            set
            {
                m_numControllersEnabled = Math.Min(value, m_numControllers);
                ResetControllerState();
            }
        }

        public BuzzerState State
        {
            get { return m_state; }
        }

        public void Connect()
        {
            if (m_state != BuzzerState.NotConnected)
                return;
            try
            {
                DirectInput input = new DirectInput();
                ClearEverthing();
                foreach (DeviceInstance device in input.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
                {
                    if (device.InstanceName != "Buzz")
                        continue;
                    m_numControllers += CONTROLLERS_PER_DEVICE;
                    Joystick joystick = new Joystick(input, device.InstanceGuid);
                    joystick.SetCooperativeLevel(m_hwnd, CooperativeLevel.Nonexclusive | CooperativeLevel.Background);
                    m_joysticks.Add(joystick);
                }
                this.NumControllersEnabled = m_numControllers;
                ChangeStateTo(BuzzerState.Disabled);
            }
            catch
            {
                ClearEverthing();
            }
        }

        public void Enable()
        {
            if (m_state == BuzzerState.NotConnected)
            {
                Connect();
                if (m_state == BuzzerState.NotConnected)
                    return;
                UnlockAllControllers();
            }
            if (m_state == BuzzerState.Accepting)
                return;
            m_timer.Start();
            ChangeStateTo(BuzzerState.Accepting);
        }

        public void Disable()
        {
            if (m_state != BuzzerState.Accepting)
                return;
            m_timer.Stop();
            ChangeStateTo(BuzzerState.Disabled);
        }

        public void UnlockAllControllers()
        {
            for (int i = 0; i < m_numControllersEnabled; ++i)
                m_fControllerUnLocked[i] = true;
        }

        public void LockController(int controller)
        {
            if (controller < m_fControllerUnLocked.Length)
                m_fControllerUnLocked[controller] = false;
        }

        public bool IsControllerLocked(int controller)
        {
            return !m_fControllerUnLocked[controller];
        }

        private void ResetControllerState()
        {
            m_fControllerUnLocked = new bool[m_numControllersEnabled];
        }

        public void ClearEverthing()
        {
            m_numControllers = 0;
            m_numControllersEnabled = 0;
            m_fControllerUnLocked = null;
            foreach (Joystick stick in m_joysticks)
            {
                stick.Unacquire();
                stick.Dispose();
            }
            m_joysticks.Clear();
            ChangeStateTo(BuzzerState.NotConnected);
        }

        private void ChangeStateTo(BuzzerState buzzerState)
        {
            bool fFire = (m_state != buzzerState);
            m_state = buzzerState;
            if (fFire && (StateChanged != null))
                StateChanged(m_state);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            bool fPressed = false;
            if (m_state != BuzzerState.Accepting)
                return;
            int controllerbase = -CONTROLLERS_PER_DEVICE;
            foreach (Joystick joystick in m_joysticks)
            {
                controllerbase += CONTROLLERS_PER_DEVICE;
                if (joystick.Acquire().IsFailure)
                    continue;
                if (joystick.Poll().IsFailure)
                    continue;
                JoystickState state = joystick.GetCurrentState();
                if (Result.Last.IsFailure)
                    continue;
                for (int i = 0; i < BUTTONS_PER_CONTROLLER * CONTROLLERS_PER_DEVICE; ++i)
                {
                    if (!state.IsPressed(i))
                        continue;
                    int controller = controllerbase + i / BUTTONS_PER_CONTROLLER;
                    int button = i % BUTTONS_PER_CONTROLLER;
                    if (controller >= m_numControllersEnabled)
                        continue;
                    if (!m_fControllerUnLocked[controller])
                        continue;
                    fPressed = true;
                    if (ButtonPressed != null)
                        ButtonPressed(controller, (BuzzerButton)Enum.Parse(typeof(BuzzerButton), button.ToString()));
                    break;
                }
                joystick.Unacquire();
                if (fPressed)
                    break;
            }
        }

        private BuzzerState m_state = BuzzerState.NotConnected;
        private IntPtr m_hwnd;
        private int m_numControllers;
        private int m_numControllersEnabled;
        private bool[] m_fControllerUnLocked;
        private List<Joystick> m_joysticks = new List<Joystick>();
        private DispatcherTimer m_timer = new DispatcherTimer();

        private const int CONTROLLERS_PER_DEVICE = 4;
        private const int BUTTONS_PER_CONTROLLER = 5;
    }
}