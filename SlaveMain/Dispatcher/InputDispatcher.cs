using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonClasses.Win32;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using System.Drawing;
using WindowsInput;




namespace SlaveMain.Dispatcher
{
    public class InputDispatcher
    {
        //TODO move or remove
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);
        const UInt32 WM_KEYDOWN = 0x0100;
        const int VK_F5 = 0x74;

        //private INPUT inputToDispatch;
        public InputDispatcher() {throw new Exception("not implemented"); }

        static POINT screenSize;
        static bool isInitialized = false;
        static HashSet<int> keyPressed = new HashSet<int>();
        //public void DispatchInput()
        static public void DispatchInput()
        {
            if(!isInitialized){
                System.Drawing.Rectangle rect = Form1.getForm().getScreenSize();
                screenSize.x = rect.Width;
                screenSize.y = rect.Height;
                isInitialized=true;
            }
            //while (true)
            INPUT_EXTENDED inputExtended;
            while (SlaveGlobalVars.SocketToDispatcherQueue.TryDequeue(out inputExtended))
            {

                INPUT inputToDispatch = inputExtended.input;

               // System.Threading.Thread.Sleep(3000);
                /*INPUT input = new INPUT
                {
                    type = 1
                };
                input.mkhi.ki = new KEYBDINPUT();
                input.mkhi.ki.Vk = (ushort)0x41;
                input.mkhi.ki.Scan = 0;
                input.mkhi.ki.Flags = 0;
                input.mkhi.ki.Time = 0;
                input.mkhi.ki.ExtraInfo = IntPtr.Zero;*/
                //if (inputToDispatch.type == (int)CommonClasses.Win32.HookType.WH_KEYBOARD_LL || inputToDispatch.type == (int)CommonClasses.Win32.HookType.WH_KEYBOARD)
                if (inputExtended.isNotInput) {
                    if (inputExtended.focusChange)
                    {
                        if (inputExtended.focusActivateDeactivate){ Form1.setStatusGUI(Form1.TypeIcon.connectedFocus);keyPressed.Clear();}
                        else
                        {
                            WindowsInput.InputSimulator.SimulateKeyUp((WindowsInput.VirtualKeyCode)Keys.LControlKey);
                            WindowsInput.InputSimulator.SimulateKeyUp((WindowsInput.VirtualKeyCode)Keys.LMenu);
                            Form1.setStatusGUI(Form1.TypeIcon.connectedInactive);
                            foreach (int key in keyPressed) WindowsInput.InputSimulator.SimulateKeyUp((WindowsInput.VirtualKeyCode)key);
                        }
                    }
                
                 }
                else if (inputToDispatch.type == (int)CommonClasses.Win32.InputType.KEYBOARD)
                {
                    INPUT[] inputs = new INPUT[] { inputToDispatch };
                    //WindowsInput.VirtualKeyCode
                    //if((inputToDispatch.mkhi.ki.Flags & 0xC0000000)==0)
                    if ((inputToDispatch.mkhi.ki.Flags & 128) == 0)
                    {
                        
                        WindowsInput.InputSimulator.SimulateKeyDown((WindowsInput.VirtualKeyCode) inputToDispatch.mkhi.ki.Vk);
                        keyPressed.Add((int)inputToDispatch.mkhi.ki.Vk);
                    }else{
                        WindowsInput.InputSimulator.SimulateKeyUp((WindowsInput.VirtualKeyCode)inputToDispatch.mkhi.ki.Vk);
                        if(keyPressed.Contains((int)inputToDispatch.mkhi.ki.Vk))keyPressed.Remove((int)inputToDispatch.mkhi.ki.Vk);
                    }
                
                    //PostMessage(proc.MainWindowHandle, WM_KEYDOWN, VK_F5, 0);
                    // Form1.getForm().M
                    //inputToDispatch = SlaveGlobalVars.SocketToDispatcherQueue.Take();
                   //last used!!
                    /* if (Win32NativeMethods.SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT))) == 0)
                    {
                        throw new Exception();
                    }*/
                    //Form1.getForm().setLabel("msg" + inputToDispatch.mkhi.ki.Vk);

                    // System.Threading.Thread.Sleep(3000);
                    // System.Windows.Forms.MessageBox.Show("ok");}
                }
                //else if (inputToDispatch.type == (int)CommonClasses.Win32.HookType.WH_MOUSE_LL)
                //http://stackoverflow.com/questions/8021954/sendinput-doesnt-perform-click-mouse-button-unless-i-move-cursor
                else if (inputToDispatch.type == (int)CommonClasses.Win32.InputType.MOUSE)
                {
                    //before movement,after click
                    INPUT mouseInput = new INPUT();
                    mouseInput.type = (int)CommonClasses.Win32.InputType.MOUSE;// SendInputEventType.InputMouse;

                    if (inputExtended.mouseTypeMovement == MouseTypeMovement.ABSOLUTE) { 
                        mouseInput.mkhi.mi.dx = inputToDispatch.mkhi.mi.dx;// CalculateAbsoluteCoordinateX(x);
                        mouseInput.mkhi.mi.dy = inputToDispatch.mkhi.mi.dy;// CalculateAbsoluteCoordinateY(y);
                    }
                    else if (inputExtended.mouseTypeMovement == MouseTypeMovement.ABSOLUTE_RESCALED)
                    {
                        mouseInput.mkhi.mi.dx = inputToDispatch.mkhi.mi.dx*screenSize.x/inputExtended.screen_size.x ;
                        mouseInput.mkhi.mi.dy = inputToDispatch.mkhi.mi.dy * screenSize.y / inputExtended.screen_size.y;

                    }
                    else if (inputExtended.mouseTypeMovement == MouseTypeMovement.RELATIVE)
                    {
                        Point oldPoint = Form1.getForm().pointMouse;
                        mouseInput.mkhi.mi.dx = oldPoint.X+ inputToDispatch.mkhi.mi.dx;
                        mouseInput.mkhi.mi.dy = oldPoint.Y + inputToDispatch.mkhi.mi.dy;

                    }
                    mouseInput.mkhi.mi.mouseData = inputToDispatch.mkhi.mi.mouseData;
                    INPUT[] inputs = new INPUT[] { mouseInput };
                    /*if (Win32NativeMethods.SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)))== 0)
                    {
                        throw new Exception();
                    }*/
                    Form1.getForm().pointMouse = new Point (mouseInput.mkhi.mi.dx,  mouseInput.mkhi.mi.dy);
                    if (inputExtended.mouseMessage != MouseMessage.WM_MOUSEMOVE && inputExtended.mouseMessage != MouseMessage.NO_MESSAGE)
                    {
                        int readDebug =(int) inputExtended.mouseMessage;
                       
                        if (Win32NativeMethods.isMouseButtonUp(inputExtended.mouseMessage)) System.Threading.Thread.Sleep(40);
                       // InputSimulator.
                        int valueMouse = MouseHelper.convertMessageToInt(inputExtended.mouseMessage);
                        //I've supposed that 3rd field is used only bt wheel
                        if (MouseHelper.isWheel(inputExtended.mouseMessage))
                        {
                            Win32NativeMethods.mouse_event((uint)valueMouse, 0, 0, MouseHelper.getWheelRotation(mouseInput.mkhi.mi.mouseData), 0);
                        }
                        else
                        {
                            Win32NativeMethods.mouse_event((uint)valueMouse, 0, 0,0, 0);
                        }
                            readDebug = readDebug;
                    }
                    //Win32NativeMethods.mouse_event(mouseInput.mkhi.mi.dwFlags,(uint) mouseInput.mkhi.mi.dx,(uint) mouseInput.mkhi.mi.dy, 0,(uint) mouseInput.mkhi.mi.dwExtraInfo);
                    //Cursor.Position = new Point(location.X + (button.Width / 2), location.Y + (button.Height / 2));


                }
            }
        }
    }
}
