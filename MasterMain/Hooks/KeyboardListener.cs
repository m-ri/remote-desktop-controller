using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using System.Windows.Forms;
using CommonClasses.Win32;
using MasterMain.Hotkeys;

namespace MasterMain.Hooks
{
    class KeyboardListener : IDisposable
    {
        private GlobalHooker h;
        //private HotKeySuite hkc;
        public static HotKeyManager hkm=new HotKeyManager();//modified only by Main Thread

        public KeyboardListener(GlobalHooker hooker, HotKeySuite HKCollection, bool blockLocalInput = false, bool blockRemoteInput=true)
        {
            if (hooker == null)
            {
                throw new ArgumentNullException("invalid keyboard hooker");
            }
           
            h = hooker;
            //hkc = HKCollection;
            //hkm=new HotKeyManager();
            Hotkeys.HotKeyHelper.fullfillHotKeys();
            KeyboardListener.blockLocalInput = blockLocalInput;
            KeyboardListener.blockRemoteInput = blockRemoteInput;

            HookCallbackReference = new HookCallback(HookCallback);
            try
            {
                HookHandle = h.Subscribe(GetHookId(), HookCallbackReference);
            }
            catch (Exception)
            {
                HookCallbackReference = null;
                HookHandle = IntPtr.Zero;
                throw;
            }
        }

        protected IntPtr HookHandle { get; set; }
        public volatile static bool  blockLocalInput ;//{ get; set; }
        public volatile static bool blockRemoteInput ;//{ get; set; }
        protected HookCallback HookCallbackReference { get; set; }


        public static void toggleLocalKeyInsert()
        {
            blockLocalInput = !blockLocalInput;
        }

                private bool sendKBDInputToQueue(KBDLLHOOKSTRUCT hookStruct)
        {
            INPUT input = new INPUT();
            input.type = (int)InputType.KEYBOARD;
            /*input.mkhi.ki.wVk = hookStruct.vkCode;
            input.mkhi.ki.wScan = hookStruct.scanCode;
            input.mkhi.ki.dwFlags = hookStruct.flags;
            input.mkhi.ki.dwExtraInfo = hookStruct.dwExtraInfo;*/
            input.mkhi.ki.Vk = 0;
            input.mkhi.ki.Vk =(ushort) hookStruct.vkCode;
            input.mkhi.ki.Scan=0;
            //input.mkhi.ki.Scan = (ushort)hookStruct.scanCode;
            
            input.mkhi.ki.Flags =(uint) hookStruct.flags;
            input.mkhi.ki.ExtraInfo = new System.IntPtr(hookStruct.dwExtraInfo);

            INPUT_EXTENDED input_extended = new INPUT_EXTENDED();
            input_extended.input = input;
            MasterGlobalVars.HookingToSocketQueue.Add(input_extended);
            //Socket.MasterSocket.sendQueuedInput();//TODO use another thread
            return true;
        }

        protected bool ProcessCallback(IntPtr wParam, IntPtr lParam)
        {
            //bool sendRemotelyThisKey = blockRemoteInput;
            //if(Form1.getForm().is)

            KeyEventMoreArgs e;
            bool KeyDown = false;
            bool KeyUp = false;
            KBDLLHOOKSTRUCT hookStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

            if (wParam.ToInt32() == Win32Messages.WM_KEYDOWN || wParam.ToInt32() == Win32Messages.WM_SYSKEYDOWN)
                KeyDown = true;
            else if (wParam.ToInt32() == Win32Messages.WM_KEYUP || wParam.ToInt32() == Win32Messages.WM_SYSKEYUP)
                KeyUp = true;

            /*e = new KeyEventMoreArgs((Keys)hookStruct.vkCode,KeyDown,KeyUp);

            //Fire event handler for hot-keys suite
            hkc.OnKey(e);

            if( e.IsKeyDown )
                InvokeKeyDown(e);
            else if(e.IsKeyUp)
                InvokeKeyUp(e);*/
            bool propagateKey = true;
            if (KeyDown)
                hkm.newKeyDown((int)hookStruct.vkCode);
            else if(KeyUp)
                propagateKey=hkm.newKeyUp((int)hookStruct.vkCode);

            if (!propagateKey) return true;//skip queueing of INPUT

            if (blockRemoteInput) return true;
            if(sendKBDInputToQueue(hookStruct) == true)
                return true;
            return false;
        }

        // protected int HookCallback(int nCode, Int32 wParam, IntPtr lParam)
        protected int HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                //send input to other machine
                if (ProcessCallback(wParam,lParam) == false)
                {
                    throw new Exception("Couldn't process the correct keyboard input");
                }
                if (blockLocalInput)
                {
                    return -1;
                }
            }
            return Win32NativeMethods.CallNextHookEx(HookHandle,nCode, wParam, lParam);
        }

        protected int GetHookId()
        {
            return GlobalHooker.WH_KEYBOARD_LL;
        }

        /*public HotKeySuite HotKeysSuite
        {
            get { return hkc; }
        }*/
     
        public event KeyEventHandler KeyDown;
        private void InvokeKeyDown(KeyEventArgs e)
        {
            KeyEventHandler handler = KeyDown;
            if (handler == null || e.Handled) { return; }
            handler(this, e);
        }

        public event KeyEventHandler KeyUp;
        private void InvokeKeyUp(KeyEventArgs e)
        {
            KeyEventHandler handler = KeyUp;
            if (handler == null || e.Handled) { return; }
            handler(this, e);
        }


        public void Dispose()
        {
            try
            {
                h.Unsubscribe(HookHandle);
            }
            finally
            {
                HookCallbackReference = null;
                HookHandle = IntPtr.Zero;
                KeyUp = null;
                KeyDown = null;
            }
            GC.SuppressFinalize(this);
        }
        ~KeyboardListener()
        {
            Win32NativeMethods.UnhookWindowsHookEx(HookHandle);
        }
    }
}
