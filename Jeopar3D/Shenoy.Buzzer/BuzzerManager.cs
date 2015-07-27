using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Shenoy.Game.Buzzer
{
    public class BuzzerManager
    {
        public static BuzzerInputDevice CreateBuzzerForWindow(Window window)
        {
            WindowInteropHelper helper = new WindowInteropHelper(window);
            if (helper.Handle.ToInt32() == 0)
                return null;
            return new BuzzerInputDevice(helper.Handle);
        }
    }
}
