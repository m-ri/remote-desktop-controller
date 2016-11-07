using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using CommonClasses.Win32;

namespace MasterMain.Hooks
{
    class GlobalHooker
    {
        /// <summary>
        /// Windows NT/2000/XP/Vista/7: Installs a hook procedure that monitors low-level mouse input events.
        /// </summary>
        internal const int WH_MOUSE_LL = 14;

        /// <summary>
        /// Windows NT/2000/XP/Vista/7: Installs a hook procedure that monitors low-level keyboard  input events.
        /// </summary>
        internal const int WH_KEYBOARD_LL = 13;

        internal /*int*/IntPtr Subscribe(int hookId, HookCallback hookCallback)
        {
            /*int*/IntPtr hookHandle = Win32NativeMethods.SetWindowsHookEx(
                hookId,
                hookCallback,
                Process.GetCurrentProcess().MainModule.BaseAddress,
                0);

            if (hookHandle == IntPtr.Zero)
            {
                ThrowLastUnmanagedErrorAsException();
            }

            return hookHandle;
        }

        internal bool Unsubscribe(IntPtr handle)
        {
            bool result = Win32NativeMethods.UnhookWindowsHookEx(handle);
            handle = IntPtr.Zero;
            return result;
        }

        internal static void ThrowLastUnmanagedErrorAsException()
        {
            //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
            int errorCode = Marshal.GetLastWin32Error();
            //Initializes and throws a new instance of the Win32Exception class with the specified error. 
            throw new Win32Exception(errorCode);
        }
    }
}
