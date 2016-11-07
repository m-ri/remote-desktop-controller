using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MasterMain.Hooks;
using CommonClasses;
using CommonClasses.Win32;

namespace MasterMain.Hotkeys
{
    class HotKeyHelper
    {
        public static int[] getCompleteHotkey(int symbol)
        {
            int lenPrefix=HotKeyManager.getPrefix().Length ;
            int[] ret = new int[lenPrefix+ 1];

            ret[0]=symbol;
            for (int i = 0; i < lenPrefix; i++)
                ret[i + 1] = HotKeyManager.getPrefix()[i];
            return ret;

        }
        public static void fullfillHotKeys()
        {
            {
                int[] keys = getCompleteHotkey((int)Keys.M); //{ (int)Keys.LControlKey, (int)Keys.LMenu, (int)Keys.M };
                //Keys[] keys = { Keys.K };
                HotKeyManager.HotKey hotKey = new HotKeyManager.HotKey(keys, () =>
                {
                    try
                    {
                        INPUT_EXTENDED input_extended = new INPUT_EXTENDED();
                        input_extended.focusChange = true; input_extended.focusActivateDeactivate = false; input_extended.isNotInput = true;
                        MasterGlobalVars.HookingToSocketQueue.Add(input_extended);
                        foreach (int _key in HotKeyManager.getPrefix())
                        {
                            int key = _key;//due to bug in some version of C#
                            Form1.getForm().BeginInvoke((Action)(() => WindowsInput.InputSimulator.SimulateKeyUp((WindowsInput.VirtualKeyCode)key)));
                        }
                    }
                    catch (Exception e) { }

                    KeyboardListener.blockLocalInput = false; KeyboardListener.blockRemoteInput = true;
                    MouseListener.blockLocalInput = false; MouseListener.blockRemoteInput = true;
                },propagateLastKey:false);
                KeyboardListener.hkm.insertHotKey(hotKey);
            }
            {
                int[] keys =getCompleteHotkey((int)Keys.S);// { (int)Keys.LControlKey, (int)Keys.LMenu, (int)Keys.S };
                //Keys[] keys = { Keys.K };
                HotKeyManager.HotKey hotKey = new HotKeyManager.HotKey(keys, () => hotkeyMasterSendClipboard(), propagateLastKey: false);
                KeyboardListener.hkm.insertHotKey(hotKey);
            }
            {
                int[] keys =getCompleteHotkey((int)Keys.G);// { (int)Keys.LControlKey, (int)Keys.LMenu, (int)Keys.G };
                //Keys[] keys = { Keys.K };
                HotKeyManager.HotKey hotKey = new HotKeyManager.HotKey(keys, () => hotkeyMasterGetClipboard(), propagateLastKey: false);
                KeyboardListener.hkm.insertHotKey(hotKey);
            }


            /*{
                int[] keys = { (int)Keys.LControlKey, (int)Keys.LMenu, (int)Keys.K };
                //Keys[] keys = { Keys.K };
                HotKeyManager.HotKey hotKey = new HotKeyManager.HotKey(keys, KeyboardListener.toggleLocalKeyInsert);
                KeyboardListener.hkm.insertHotKey(hotKey);
            }
            {
                int[] keys = { (int)Keys.LControlKey, (int)Keys.LMenu, (int)Keys.M };
                //Keys[] keys = { Keys.K };
                HotKeyManager.HotKey hotKey = new HotKeyManager.HotKey(keys, MouseListener.toggleLocalMouseInsert);
                KeyboardListener.hkm.insertHotKey(hotKey);
            }
            {
                int[] keys = { (int)Keys.LControlKey, (int)Keys.LMenu, (int)Keys.S };
                //Keys[] keys = { Keys.K };
                HotKeyManager.HotKey hotKey = new HotKeyManager.HotKey(keys, () =>
                {
                    Socket.MasterSocket.pendingClipboard = true;
                    INPUT_EXTENDED wake_up = new INPUT_EXTENDED();
                    wake_up.not_send = true;
                    MasterGlobalVars.HookingToSocketQueue.Add(wake_up);
                });
                KeyboardListener.hkm.insertHotKey(hotKey);
            }*/
        }
        public static void addHotKeySlave(int _ID)
        {
            int ID = _ID;
            if (ID > 9 || ID < 0) throw new Exception("range hot-key for new slave not supported");

            int[] keys = getCompleteHotkey((int)Keys.NumPad0 + ID);//{ (int)Keys.LControlKey, (int)Keys.LMenu, (int)Keys.NumPad0+ID };
            //Keys[] keys = { Keys.K };
            HotKeyManager.HotKey hotKey = new HotKeyManager.HotKey(keys, () => hotkeyFocusToSlave(ID), propagateLastKey: false);
            KeyboardListener.hkm.insertHotKey(hotKey);

        }

        private static void hotkeyFocusToSlave(int ID)
        {
            int idOld=Form1.getIdSlaveFocus();
            if ( idOld != ID)
            {
                if(idOld>=0) MasterMain.Socket.MasterSocket.SlaveEnableDisable(idOld, false);
                MasterMain.Socket.MasterSocket.SlaveEnableDisable(ID, true);
            }
            try
            {
                INPUT_EXTENDED input_extended = new INPUT_EXTENDED();
                input_extended.isNotInput = true;
                input_extended.focusChange = true; input_extended.focusActivateDeactivate = true;
                MasterGlobalVars.HookingToSocketQueue.Add(input_extended);
            }
            catch (Exception e) { }

            KeyboardListener.blockLocalInput = true; KeyboardListener.blockRemoteInput = false;
            MouseListener.blockLocalInput = true; MouseListener.blockRemoteInput = false;
        }
        
        private static void hotkeyMasterSendClipboard()
        {
            int idSlave = Form1.getIdSlaveFocus();
            if(idSlave<0)return;
            Socket.MasterSocket.Slave slave= Socket.MasterSocket.getSlave(idSlave);
            slave.taskClip.ContinueWith(antecendent =>
            {
                try
                {
                    Form1.setTitle("clipboard sending");
                    CommonClasses.SocketMessages.Message m;
                    m=(CommonClasses.SocketMessages.Message)Form1.getForm().Invoke(new Func< CommonClasses.SocketMessages.Message>(()=>ManageClipboard.getCurrentClipboardMessage()));
                    //ManageClipboard.getCurrentClipboardMessage();
                    m.WriteToSocket(slave.clipboard_ns);
                    slave.clipboard_ns.Flush();
                    Form1.setTitle("clipboard sent");
                }
                catch (Exception e)
                {
                    throw e;
                }
            });
        }
        private static void hotkeyMasterGetClipboard()
        {
            int idSlave = Form1.getIdSlaveFocus();
            if (idSlave < 0) return;
            Socket.MasterSocket.Slave slave = Socket.MasterSocket.getSlave(idSlave);
            slave.taskClip.ContinueWith(antecendent =>
            {
                try
                {
                    Form1.setTitle("clipboard receiving");
                    CommonClasses.SocketMessages.Message m = new CommonClasses.SocketMessages.Message(CommonClasses.SocketMessages.Message.TypeMessage.ClipboardRequest);
                    m.WriteToSocket(slave.clipboard_ns); slave.clipboard_ns.Flush();
                    m = CommonClasses.SocketMessages.Message.ReadFromSocket(slave.clipboard_ns);
                    if (m.typeMessage == CommonClasses.SocketMessages.Message.TypeMessage.ClipboardAttached)
                    {
                       Form1.getForm().BeginInvoke((Action)(()=> ManageClipboard.setCurrentClipboard(m)));
                       Form1.setTitle("clipboard received");
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
               
            });
        }
    }
}
