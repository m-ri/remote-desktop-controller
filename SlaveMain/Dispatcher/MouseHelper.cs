using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonClasses.Win32;

namespace SlaveMain.Dispatcher
{
    class MouseHelper
    {
        /*
         * Covert internal numbers,taken for working example
         * http://stackoverflow.com/questions/14876345/using-sendmessage-to-simulate-mouse-clicks
         *
         *Complete list of values:
         *https://msdn.microsoft.com/it-it/library/windows/desktop/ms646260(v=vs.85).aspx
         * 
         */
        static public int convertMessageToInt(MouseMessage mouseMessage)
        {
            switch (mouseMessage)
            {
                case MouseMessage.WM_LBUTTONDOWN:
                    return 0x02;
                case MouseMessage.WM_LBUTTONUP:
                    return 0x04;
                case MouseMessage.WM_RBUTTONDOWN:
                    return 0x08; 
                case MouseMessage.WM_RBUTTONUP:
                    return 0x010; 
                case MouseMessage.WM_MBUTTONDOWN:
                    return 0x020; 
                case MouseMessage.WM_MBUTTONUP:
                    return 0x040; 
                case MouseMessage.WM_MOUSEWHEEL:
                    return 0x0800;
                case MouseMessage.WM_MOUSEHWHEEL:
                    return 0x01000; 
                default:
                    int a=1;
                    return 0;
                    break;

            }
        }
        //https://msdn.microsoft.com/it-it/library/windows/desktop/ms644970(v=vs.85).aspx
        public static uint getWheelRotation(uint mouseData)
        {
            uint val = (uint)((int)mouseData >> 16);// &0x0000FFFF;
            return val;
        }
        //TODO is correct to treat both wheels in same way?
        static public bool isWheel(MouseMessage mouseMessage)
        {
            return (mouseMessage == MouseMessage.WM_MOUSEWHEEL || mouseMessage == MouseMessage.WM_MOUSEHWHEEL);
        }
    }
}
