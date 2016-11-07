using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using CommonClasses.Win32;

namespace MasterMain.Hooks
{
    class MouseListener : IDisposable
    {
        private GlobalHooker h;
        POINT _previousXY;
        POINT _screenSize;
        bool previousXYInitialized = false;
        static MouseTypeMovement mouseTypeMovement;


        public MouseListener(GlobalHooker hooker, bool blockLocalInput = false, bool blockRemoteInput=true)
        {
            if (hooker == null)
            {
                throw new ArgumentNullException("invalid mouse hooker");
            }
            h = hooker;
            MouseListener.blockLocalInput = blockLocalInput;
            MouseListener.blockRemoteInput = blockRemoteInput;

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
            mouseTypeMovement = Form1.getForm().mouseTypeMovement;
            System.Drawing.Rectangle rect = Form1.getForm().getScreenSize();
            _screenSize.x = rect.Width;
            _screenSize.y = rect.Height;
        }

        protected IntPtr HookHandle { get; set; }
        public volatile static bool blockLocalInput;// { get; set; }
        public volatile static bool blockRemoteInput;// { get; set; }
        public static void toggleLocalMouseInsert()
        {
            blockLocalInput = !blockLocalInput;
        }
        protected HookCallback HookCallbackReference { get; set; }

        // protected bool ProcessCallback(int wParam, IntPtr lParam)
        protected bool ProcessCallback(IntPtr wParam, IntPtr lParam)
        {
            //bool shouldProcess = ProcessCallback(wParam, lParam);
            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            MouseMessage wmMouse = (MouseMessage)wParam;

            if (sendMInputToQueue(hookStruct,wmMouse) == true)
                return true;

            return false;
        }

        public static void setMouseTypeMovement(MouseTypeMovement mouseTypeMovement)
        {
            MouseListener.mouseTypeMovement = mouseTypeMovement;
        }

        
        /* The specific function that is being used to convert back from normalized coordinates to pixels is equivalent to:
        x = trunc(dx*PrimaryScreen.Width/65536)
        y = trunc(dy*PrimaryScreen.Height/65536)

        So to make sure that the click arrives at the exact coordinates you desire, you have to convert from pixels to normalized like this:
        dx = ceiling(x*65536/PrimaryScreen.Width)
        dy = ceiling(y*65536/PrimaryScreen.Height)
        */
        /*
         * http://stackoverflow.com/questions/8021954/sendinput-doesnt-perform-click-mouse-button-unless-i-move-cursor
         * */
        private bool sendMInputToQueue(MSLLHOOKSTRUCT hookStruct,MouseMessage mouseMessage)
        {
            MouseTypeMovement localMouseTypeMovement = mouseTypeMovement;

            if (!previousXYInitialized)
            {
                _previousXY.x = hookStruct.pt.x;
                _previousXY.y = hookStruct.pt.y;
                previousXYInitialized = true;
            }
            INPUT input = new INPUT();
            input.type = (int)InputType.MOUSE;
            input.mkhi.mi.dwFlags = (uint)hookStruct.flags;
            if (localMouseTypeMovement == MouseTypeMovement.ABSOLUTE || localMouseTypeMovement == MouseTypeMovement.ABSOLUTE_RESCALED)
            {
                input.mkhi.mi.dx = hookStruct.pt.x;
                input.mkhi.mi.dy = hookStruct.pt.y;
            }
            else if (localMouseTypeMovement == MouseTypeMovement.RELATIVE)
            {
                input.mkhi.mi.dx = hookStruct.pt.x-_previousXY.x;
                input.mkhi.mi.dy = hookStruct.pt.y-_previousXY.y;
            }
            input.mkhi.mi.mouseData = hookStruct.mouseData;
            input.mkhi.mi.dwExtraInfo = hookStruct.dwExtraInfo;

            INPUT_EXTENDED input_extended=new INPUT_EXTENDED();
            input_extended.input=input;
            input_extended.mouseMessage = mouseMessage;
            input_extended.mouseTypeMovement = localMouseTypeMovement;
            if (localMouseTypeMovement == MouseTypeMovement.ABSOLUTE_RESCALED)
            {
                input_extended.screen_size = _screenSize;
            }

            /*if (mouseMessage != MouseMessage.WM_MOUSEMOVE && ((int)mouseMessage) != 0)
            {
                mouseMessage = mouseMessage;  
            }*/
            //MasterGlobalVars.HookingToSocketQueue.Enqueue(input_extended);
           
            //Socket.MasterSocket.sendQueuedInput();//TODO use another thread

            if (!blockLocalInput)
            {
                _previousXY.x = hookStruct.pt.x;
                _previousXY.y = hookStruct.pt.y;
            }
            else
            {
                //workaround in order to get deltaXY: the SO inject the new position,but at the end is not accepted..so position remain the same
            }
            if (blockRemoteInput) return true;
            MasterGlobalVars.HookingToSocketQueue.Add(input_extended);

            return true;
        }

        //protected int HookCallback(int nCode, Int32 wParam, IntPtr lParam)
        protected int HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {

               
                    //send input to other machine
                if (ProcessCallback(wParam, lParam) == false)
                {
                    throw new Exception("Couldn't process the correct input");
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
            return GlobalHooker.WH_MOUSE_LL;
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
            }
            GC.SuppressFinalize(this);
        }
        ~MouseListener()
        {
            Win32NativeMethods.UnhookWindowsHookEx(HookHandle);
        }
    }
}
