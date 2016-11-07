using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

using CommonClasses;
using SlaveMain;
using System.Net.Sockets;
using CommonClasses.SocketMessages;
using CommonClasses.Win32;

namespace MasterMain
{
    public partial class Form1 : Form
    {
      
        static Form1 _form_master = null;
        public Form1()
        {
            InitializeComponent();
            _form_master = this;
            rdbMouseAbs.CheckedChanged += rdbMouse_CheckedChanged;
            rdbMouseAbsResc.CheckedChanged += rdbMouse_CheckedChanged;
            rdbMouseRel.CheckedChanged += rdbMouse_CheckedChanged;
            keyboardListener = new Hooks.KeyboardListener(globalHooks, hotKeySuite);
            mouseListener = new Hooks.MouseListener(globalHooks);
        }
        System.Threading.Timer timerTimeout;
        public static Form1 getForm()
        {
            return _form_master;

        }

        private void impostazioniToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void preferenzeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        public static void setTitle(string str="")
        {
            _form_master.BeginInvoke((Action)(()=>{
                if(str.CompareTo("")==0)
                    _form_master.Text="Master";
                else _form_master.Text="Master ("+str+")";
            }));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            updateMouseTypeMovement();
            //lsbSlaves.
            /*lsbSlaves.Columns.Add("Id");
            lsbSlaves.Columns.Add("IP");
            lsbSlaves.Columns.Add("porta");*/
            timerTimeout = new System.Threading.Timer((ee) =>checkConnections(), null, 0, 20 * 1000);
            setTitle();
        }
        //timeout
        void checkConnections()
        {

            if (masterSocket == null) return;
            HashSet<int> slavesMissing = new HashSet<int>();

            foreach (int idSlave in masterSocket.getIdSlaves())
            {
                
                Socket.MasterSocket.Slave slave = Socket.MasterSocket.getSlave(idSlave);
                bool ok = !slave.mustBeDeleted;
                if (ok)
                {
                    try
                    {
                        ok = ok && CommonClasses.SocketMessages.TcpUtilities.isTcpActive(slave.cli);
                        ok = ok && CommonClasses.SocketMessages.TcpUtilities.isTcpActive(slave.clipboard_cli);
                    }
                    catch (Exception exp)
                    {
                        ok = false;
                    }
                    
                }
                if (!ok) slavesMissing.Add(idSlave);
            }

            
            foreach (int idSlave in slavesMissing)
            {
                Socket.MasterSocket.Slave slave = Socket.MasterSocket.getSlave(idSlave);
                if (idSlave == idSlaveFocus)
                {
                    try{
                        if(!slave.mustBeDeleted )Socket.MasterSocket.SlaveEnableDisable(idSlave, false);
                        idSlaveFocus = -1;
                    }catch(Exception e){}
                }
                
                try{
                   
                //lsbSlaves.Items.RemoveByKey(""+idSlave);
                slave.close();
                }catch(Exception e){}

                Form1.getForm().Invoke((Action)(() =>
                {
                    List<ListViewItem> itemsList = new List<ListViewItem>();
                    foreach (ListViewItem item in lsbSlaves.Items)
                    {
                        if (slavesMissing.Contains((int)item.Tag)) itemsList.Add(item);
                    }
                    foreach (ListViewItem item in itemsList) lsbSlaves.Items.Remove(item);

                }));

                masterSocket.removeSlave(idSlave);

            }
            

        }
     
        private void btnChangePort_Click(object sender, EventArgs e)
        {

        }

        IDictionary<string, object> mappa;
        //copy clipboard
        private void button1_Click(object sender, EventArgs e)
        {

            mappa = ManageClipboard.getCurrentClipboard();
           // ManageClipboard.getCurrentClipboardBase();
        }
        //paste clipboard
        private void button2_Click(object sender, EventArgs e)
        {
            ManageClipboard.setCurrentClipboard(mappa);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            try
            {
                mouseListener.Dispose();
                keyboardListener.Dispose();
            }
            catch (Exception exp) { }
            SlaveMain.Form1 slaveForm = new SlaveMain.Form1();
            try
            {
                slaveForm.Closed += (s, args) => this.Close(); 
                //slaveForm.ShowDialog();

                slaveForm.ShowDialog();

            }
            catch (Exception exp) {
                throw exp; }
            try
            {
                this.Close();
            }
            catch (Exception exp) { }
            //Hide();
          

        }
        private NetworkStream ns;
        private void button4_Click(object sender, EventArgs e)
        {

            CommonClasses.SocketMessages.Message m;
           /* byte[] bytes = MD5PasswordHashing.GetMd5HashBytes(SlaveGlobalVars.Password);
           
            TcpClient cl = new TcpClient("127.0.0.1", CommonClasses.DefaultSettings.listeningPort);

           
   // MessageBox.Show("master connecting");
             m=new CommonClasses.SocketMessages.Message(CommonClasses.SocketMessages.Message.Status.Authenticate,bytes);
            ns = cl.GetStream();
            m.WriteToSocket(ns);
            ns.Flush();
            System.Threading.Thread.Sleep(500);
            m = CommonClasses.SocketMessages.Message.ReadFromSocket(ns);*/



            INPUT input = new INPUT();
            input.type=1;
            input.mkhi.ki = new KEYBDINPUT();
            input.mkhi.ki.Vk = (ushort)0x41;
            input.mkhi.ki.Scan = 0;
            input.mkhi.ki.Flags = 0;
            input.mkhi.ki.Time = 0;
            input.mkhi.ki.ExtraInfo = IntPtr.Zero;


           /* KEYBDINPUT keydb = new KEYBDINPUT();

            keydb.Vk = (ushort)0x41;// keyCode;
            keydb.Scan = 0;
            keydb.Flags = 0;
            keydb.Time = 0;
            keydb.ExtraInfo = IntPtr.Zero;
           


            INPUT input=new INPUT();
            input.mkhi.ki = keydb;*/
            m = new CommonClasses.SocketMessages.Message(CommonClasses.SocketMessages.Message.TypeMessage.Input);
            m.data = input;
            m.WriteToSocket(Socket.MasterSocket.getSlave(idSlaveFocus).ns);
            Socket.MasterSocket.getSlave(idSlaveFocus).ns.Flush();
            



        }

        //TODO remove it
         private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (mouseListener != null) mouseListener.Dispose();
                if (keyboardListener != null) keyboardListener.Dispose();
            }
            catch (Exception exp) { }
            Environment.Exit(Environment.ExitCode);
        }

        private bool hook_listening=false;
        Hooks.GlobalHooker globalHooks=new Hooks.GlobalHooker();

        Hotkeys.HotKeySuite hotKeySuite=new Hotkeys.HotKeySuite();
        Hooks.KeyboardListener keyboardListener =null;
        Hooks.MouseListener mouseListener = null;
        //abilita/disabilita hooking mouse e tastiera
         private void btnHook_Click(object sender, EventArgs e)
         {
             
                 
             

             if (!hook_listening) {
                 updateMouseTypeMovement();
                 if (radioButtonHookGlobal.Checked){
                     keyboardListener = new Hooks.KeyboardListener(globalHooks, hotKeySuite);
                     mouseListener = new Hooks.MouseListener(globalHooks);
                     //globalHooks.Subscribe(CommonClasses.Win32.HookType.WH_KEYBOARD_LL,
                 }else if(radioButtonHookLocal.Checked){

                 }

                 btnHook.Text = "Sospendi eventi";
             }else{
                 keyboardListener.Dispose();
                 mouseListener.Dispose();
                 btnHook.Text = "Intercetta eventi";
             }

             hook_listening = !hook_listening;
             radioButtonHookGlobal.Enabled = !hook_listening;
             radioButtonHookLocal.Enabled = !hook_listening;
             /*new Thread(() =>
             {
                 CommonClasses.SocketMessages.Message m = new CommonClasses.SocketMessages.Message(CommonClasses.SocketMessages.Message.Status.Input);
                 INPUT input;
                 Globals.MasterGlobalVars.ConcurrentQueue.


             }).Start();*/
         }

         Socket.MasterSocket masterSocket = null;
         static volatile int idSlaveFocus=-1;
         static public int getIdSlaveFocus()
         {
             return idSlaveFocus;
         }
        static public void setIdSlaveFocus(int id){//called by hot-key method(to focus a slave)
            idSlaveFocus = id;
        }
       
         private void button6_Click(object sender, EventArgs e)
         {
             if (masterSocket == null)
             {
                 masterSocket = new Socket.MasterSocket(CommonClasses.DefaultSettings.ListeningPort);
             }
             //idSlave= masterSocket.AddNewSlave("127.0.0.1");
             string IP=Microsoft.VisualBasic.Interaction.InputBox("insert IP","","192.168.1.50");
             idSlaveFocus = masterSocket.AddNewSlave(IP, CommonClasses.DefaultSettings.ListeningPort);
             masterSocket.ConnectToSlave(idSlaveFocus);
             Socket.MasterSocket.SlaveEnableDisable(idSlaveFocus, true);

             Hotkeys.HotKeyHelper.addHotKeySlave(idSlaveFocus);
             //idSlave = masterSocket.AddNewSlave("192.168.1.164");
             //masterSocket.ConnectToSlave(idSlave);
             //new Thread(() => masterSocket.ConnectToSlave(idSlave)).Start();
         }

         private void updateMouseTypeMovement()
         {
             MouseTypeMovement mouseTypeMovementOld = _mouseTypeMovement;
             if (rdbMouseAbs.Checked)
             {
                 _mouseTypeMovement=CommonClasses.Win32.MouseTypeMovement.ABSOLUTE;
             }else if (rdbMouseAbsResc.Checked)
             {
                 _mouseTypeMovement=CommonClasses.Win32.MouseTypeMovement.ABSOLUTE_RESCALED;
             }else if (rdbMouseRel.Checked)
             {
                 _mouseTypeMovement = CommonClasses.Win32.MouseTypeMovement.RELATIVE;
             }
             if (_mouseTypeMovement != mouseTypeMovementOld)
             {
                 Hooks.MouseListener.setMouseTypeMovement(_mouseTypeMovement);
             }
         }

         private static CommonClasses.Win32.MouseTypeMovement _mouseTypeMovement;
         public CommonClasses.Win32.MouseTypeMovement mouseTypeMovement
         {
             get
             {
                 return _mouseTypeMovement;
             }
         }

       

        private void rdbMouse_CheckedChanged(object sender, EventArgs e)
        {
            updateMouseTypeMovement();
        }
        public Rectangle getScreenSize()
        {
            return Screen.FromControl(this).Bounds;
        }

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private void button7_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(3000);

            Point point = Cursor.Position;
            CommonClasses.Win32.Win32NativeMethods.mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)point.X, (uint)point.Y, 0, 0);
            System.Threading.Thread.Sleep(1000);
            CommonClasses.Win32.Win32NativeMethods.mouse_event(MOUSEEVENTF_LEFTUP, (uint)point.X, (uint)point.Y, 0, 0);
        }

        private void btnSlavesNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (masterSocket == null)
                {
                    masterSocket = new Socket.MasterSocket(CommonClasses.DefaultSettings.ListeningPort);
                }
                int idSlave;
                string IP = Microsoft.VisualBasic.Interaction.InputBox("insert IP", "", "192.168.1.50");
                string sPort;
                do
                {
                    sPort = Microsoft.VisualBasic.Interaction.InputBox("insert port", "", "" + CommonClasses.DefaultSettings.ListeningPort);
                } while (!sPort.All(char.IsDigit));
                int port = Convert.ToInt32(sPort);
                string password = Microsoft.VisualBasic.Interaction.InputBox("insert password", "", CommonClasses.DefaultSettings.DefaultPassword);

                idSlave = masterSocket.AddNewSlave(IP, port, password);
                masterSocket.ConnectToSlave(idSlave);

                

                ListViewItem item = new ListViewItem(""+idSlave);
                item.SubItems.Add(IP);
                item.SubItems.Add(""+port);
                item.Tag = idSlave; //item.Text=IP + ":" + port;
                lsbSlaves.Items.Add(item);
                Hotkeys.HotKeyHelper.addHotKeySlave(idSlave);
            }
            catch (Exception exp) {
                MessageBox.Show("Impossibile stabilire una connessione");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (lsbSlaves.SelectedItems.Count > 0)
            {
                ListViewItem item = (ListViewItem)lsbSlaves.SelectedItems[0];
                int idFlagSelected = Convert.ToInt32(item.Tag);
                //MessageBox.Show("id" + idFlagSelected);
                if (idSlaveFocus == idFlagSelected)
                {
                    Socket.MasterSocket.SlaveEnableDisable(idFlagSelected, false);
                    idSlaveFocus = -1;//I want to avoid all future references
                }
                masterSocket.removeSlave(idFlagSelected);
                lsbSlaves.Items.Remove(item);
            }
        }

        bool hotkeyDetectingNewPrefix = false;
        private void btnHotkeyManage_Click(object sender, EventArgs e)
        {
            
            if (!hotkeyDetectingNewPrefix)
            {
                Hooks.KeyboardListener.hkm.removeAllHotKeys();
                Hotkeys.HotKeyManager.detectNewPrefixStart();
                btnHotkeyManage.Text = "Termina lettura";
            }
            else
            {
                Hotkeys.HotKeyManager.detectNewPrefixEnd();
                Hotkeys.HotKeyHelper.fullfillHotKeys();
                foreach (int idSlave in masterSocket.getIdSlaves())
                {
                    Hotkeys.HotKeyHelper.addHotKeySlave(idSlave);
                }
                btnHotkeyManage.Text = "Inizia";
            }
            hotkeyDetectingNewPrefix = !hotkeyDetectingNewPrefix;
        }

        private void lsbSlaves_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //I want to avoid when user click a white area
            //http://stackoverflow.com/questions/12872740/doubleclick-on-a-row-in-listview
            ListViewHitTestInfo info = lsbSlaves.HitTest(e.X, e.Y);
            ListViewItem item = info.Item;

            if (item != null)
            {
                //MessageBox.Show("The selected Item Name is: " + item.Text);
                int oldId = idSlaveFocus;
                int actId = Convert.ToInt32(item.Text);
                idSlaveFocus = actId;
                if(oldId<0){
                    MasterMain.Socket.MasterSocket.SlaveEnableDisable(actId, true);
                }else if (actId == oldId)
                {
                    MasterMain.Socket.MasterSocket.SlaveEnableDisable(actId, true);
                }
                else
                {
                    MasterMain.Socket.MasterSocket.SlaveEnableDisable(oldId, false);
                    MasterMain.Socket.MasterSocket.SlaveEnableDisable(actId, true);
                }
                Hooks.KeyboardListener.blockLocalInput = true; Hooks.KeyboardListener.blockRemoteInput = false;
                Hooks.MouseListener.blockLocalInput = true; Hooks.MouseListener.blockRemoteInput = false;
            }
            else
            {
                //this.lsbSlaves.SelectedItems.Clear();
                //MessageBox.Show("No Item is selected");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (masterSocket != null)
            {
                foreach (int idSlave in masterSocket.getIdSlaves())
                {

                    Socket.MasterSocket.Slave slave = Socket.MasterSocket.getSlave(idSlave);
                    try
                    {
                        slave.cli.Close();
                        slave.clipboard_cli.Close();
                        slave.threadMessage.Abort();
                        slave.taskClip.Dispose();
                    }
                    catch (Exception exp) { }
                }
            }
        }


    }
}
