using System;
using System.Runtime.InteropServices;

/// <summary>
/// Here there are different structs, both for interaction with Windows API and communication between hosts.
/// </summary>
namespace CommonClasses.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    /// <summary>
    /// win32 struct for keystrokes
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
        /// <summary>
        /// Specifies a virtual-key code
        /// </summary>
        public int vkCode;
        /// <summary>
        /// Specifies a hardware scan code for the key
        /// </summary>
        public int scanCode;    
        public int flags;
        /// <summary>
        /// Specifies the time stamp for this message
        /// </summary>
        public int time;       
        public int dwExtraInfo;
    }
   /* public struct KBDLLHOOKSTRUCT
    {
        public ushort vkCode;
        public ushort scanCode;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }*/

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    // SendInput section
    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

   /* [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }*/
    [StructLayout(LayoutKind.Sequential)]
    public /*internal*/ struct KEYBDINPUT
    {
        public ushort Vk;
        public ushort Scan;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {
        public int uMsg;
        public short wParamL;
        public short wParamH;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct MOUSEKEYBDHARDWAREINPUT
    {
        [FieldOffset(0)]
        public MOUSEINPUT mi;

        [FieldOffset(0)]
        public KEYBDINPUT ki;

        [FieldOffset(0)]
        public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        public int type;
        //public InputType type;
        public MOUSEKEYBDHARDWAREINPUT mkhi;
    }
    public struct INPUT_EXTENDED
    {
        public bool isNotInput;
        public INPUT input;
        public POINT screen_size;
        public MouseTypeMovement mouseTypeMovement;
        public MouseMessage mouseMessage;
        public bool not_send;

        //currently this part only change icons on slave(because connections remain open)
        public bool focusChange;
        public bool focusActivateDeactivate;//true for active focus
    }
    public enum MouseTypeMovement
    {
        ABSOLUTE=0,
        ABSOLUTE_RESCALED=1,//dx and dy contain absolute x and y from commander
        RELATIVE=2//dx and dy contain shifts from last 
    }

    public enum InputType
    {
        MOUSE = 0,
        KEYBOARD = 1
    }

    public enum HookType
    {
        WH_KEYBOARD = 2,
        WH_MOUSE = 7,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }

    public enum MouseMessage
    {
        NO_MESSAGE=0,//TODO: check if 0 is already assinged for other
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_RBUTTONDBLCLK = 0x0206,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MBUTTONDBLCLK = 0x0209,

        WM_MOUSEWHEEL = 0x020A,
        WM_MOUSEHWHEEL = 0x020E,

        WM_NCMOUSEMOVE = 0x00A0,
        WM_NCLBUTTONDOWN = 0x00A1,
        WM_NCLBUTTONUP = 0x00A2,
        WM_NCLBUTTONDBLCLK = 0x00A3,
        WM_NCRBUTTONDOWN = 0x00A4,
        WM_NCRBUTTONUP = 0x00A5,
        WM_NCRBUTTONDBLCLK = 0x00A6,
        WM_NCMBUTTONDOWN = 0x00A7,
        WM_NCMBUTTONUP = 0x00A8,
        WM_NCMBUTTONDBLCLK = 0x00A9
    }

    


}
    