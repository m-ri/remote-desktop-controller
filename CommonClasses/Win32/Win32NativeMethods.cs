using System;
using System.Runtime.InteropServices;


namespace CommonClasses.Win32
{
   
    public static class Win32NativeMethods
    {

        /// <summary>
        /// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain. 
        /// A hook procedure can call this function either before or after processing the hook information. 
        /// </summary>
        /// <param name="idHook">Ignored.</param>
        /// <param name="nCode">[in] Specifies the hook code passed to the current hook procedure.</param>
        /// <param name="wParam">[in] Specifies the wParam value passed to the current hook procedure.</param>
        /// <param name="lParam">[in] Specifies the lParam value passed to the current hook procedure.</param>
        /// <returns>This value is returned by the next hook procedure in the chain.</returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode,
      IntPtr wParam, IntPtr lParam);
         /*public static extern int CallNextHookEx(
            int idHook,
            int nCode,
            int wParam,
            IntPtr lParam);*/

        /// <summary>
        /// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain. 
        /// You would install a hook procedure to monitor the system for certain types of events. These events 
        /// are associated either with a specific thread or with all threads in the same desktop as the calling thread. 
     /*   /// </summary>
        /// <param name="idHook">
        /// [in] Specifies the type of hook procedure to be installed. This parameter can be one of the following values.
        /// </param>
        /// <param name="lpfn">
        /// [in] Pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier of a 
        /// thread created by a different process, the lpfn parameter must point to a hook procedure in a dynamic-link 
        /// library (DLL). Otherwise, lpfn can point to a hook procedure in the code associated with the current process.
        /// </param>
        /// <param name="hMod">
        /// [in] Handle to the DLL containing the hook procedure pointed to by the lpfn parameter. 
        /// The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by 
        /// the current process and if the hook procedure is within the code associated with the current process. 
        /// </param>
        /// <param name="dwThreadId">
        /// [in] Specifies the identifier of the thread with which the hook procedure is to be associated. 
        /// If this parameter is zero, the hook procedure is associated with all existing threads running in the 
        /// same desktop as the calling thread. 
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the handle to the hook procedure.
        /// If the function fails, the return value is NULL. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int SetWindowsHookEx(
            int idHook,
            HookCallback lpfn,
            IntPtr hMod,
            int dwThreadId);*/

        /// <summary>
        /// The SetWindowsHookEx function installs an application-defined hook 
        /// procedure into a hook chain. You would install a hook procedure to monitor 
        /// the system for certain types of events. These events are associated either 
        /// with a specific thread or with all threads in the same desktop as the 
        /// calling thread. 
        /// </summary>
        /// <param name="hookType">
        /// Specifies the type of hook procedure to be installed
        /// </param>
        /// <param name="callback">Pointer to the hook procedure.</param>
        /// <param name="hMod">
        /// Handle to the DLL containing the hook procedure pointed to by the lpfn 
        /// parameter. The hMod parameter must be set to NULL if the dwThreadId 
        /// parameter specifies a thread created by the current process and if the 
        /// hook procedure is within the code associated with the current process. 
        /// </param>
        /// <param name="dwThreadId">
        /// Specifies the identifier of the thread with which the hook procedure is 
        /// to be associated.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the handle to the hook 
        /// procedure. If the function fails, the return value is 0.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowsHookEx(int hookType,
            HookCallback callback, IntPtr hMod, uint dwThreadId);

        /// <summary>
        /// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function. 
        /// </summary>
        /// <param name="idHook">
        /// [in] Handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to SetWindowsHookEx. 
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);
       // public static extern int UnhookWindowsHookEx(int idHook);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);
        //public static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        /// <summary>
        /// The mouse_event function fires a mouse event in the local message queue. 
        /// </summary>
       /* [DllImport("user32.dll")]
        public static extern void mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 dwData, IntPtr dwExtraInfo);
*/
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        /// <summary>
        /// Places the given window in the system-maintained clipboard format listener list.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern public bool AddClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// Removes the given window from the system-maintained clipboard format listener list.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern public bool RemoveClipboardFormatListener(IntPtr hwnd);

        //other about clipboard
        //https://msdn.microsoft.com/en-us/library/windows/desktop/ms649016(v=vs.85).aspx
        //TODO:use clipboard serial number in order to delete previous folders:https://msdn.microsoft.com/en-us/library/windows/desktop/ms649042(v=vs.85).aspx

        public static bool isMouseButtonUp(MouseMessage mouseMessage)
        {
            return ((mouseMessage == MouseMessage.WM_LBUTTONUP) || (mouseMessage == MouseMessage.WM_RBUTTONUP) || (mouseMessage == MouseMessage.WM_MBUTTONUP));
        }

    }

}