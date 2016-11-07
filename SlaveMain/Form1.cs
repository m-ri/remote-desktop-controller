using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SlaveMain.Dispatcher;
using SlaveMain.Socket;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using CommonClasses.SocketMessages;
using System.Configuration;

namespace SlaveMain
{
    public partial class Form1 : Form
    {
        private SlaveSocket socket;
        private InputDispatcher dispatcher;
        private Thread dispatcherThread;
        private Thread socketThread;
        private StringValidator passwordValidator;

        private static bool _isConnectedMaster = false;
        public bool isConnectedMaster
        {
            get
            {
                return _isConnectedMaster;
            }
            set
            {
                bool old = _isConnectedMaster;
                if (old == true && value == false)
                {
                    try
                    {
                        Socket.SlaveSocket.clipThread.Abort();
                    }catch (Exception e) { }
                    try{
                        Socket.SlaveSocket.clipCli.Close();
                    }
                    catch (Exception e) { }
                    try
                    {
                        Socket.SlaveSocket.commThread.Abort();
                    }
                    catch (Exception e) { }
                    try{
                        Socket.SlaveSocket.master.Close();
                    }
                    catch (Exception e) { }
                    setStatusGUI(TypeIcon.begin);
                }

                
                _isConnectedMaster = value;
            }
        }

        void socketThreadFunc()
        {
            try
            {
                label3.ForeColor = Color.Green;
                socket.WaitForMasterConnection();
            }
            catch (SockAddrAlreadyInUseException)
            {
                label3.ForeColor = Color.Red;
                MessageBox.Show("Remote Control: Please change the listening port through the options. Address already in use.");
            }

        }

       /* void dispatcherThreadFunc()
        {
            dispatcher.DispatchInput();
        }*/



        public enum TypeIcon
        {
            begin,
            connectedInactive,
            connectedFocus
        }

        public static void setStatusGUI(TypeIcon typeIcon)
        {
           // return;
            try
            {
                String text = "";
                Bitmap theBitmap=new Bitmap("RemoteDesktopIcon.ico");
                switch (typeIcon)
                {
                    case TypeIcon.begin:
                        text = "Waiting to become target of the master...";
                        theBitmap = new Bitmap("RemoteDesktopIcon.ico");
                        //_form_slave.notifyIcon1.Icon = new Icon("RemoteDesktopIcon.ico");
                        //_form_slave.Icon = new Icon("RemoteDesktopIcon.ico");
                        break;
                    case TypeIcon.connectedFocus:
                        text = "Connected - Focused";
                        theBitmap = new Bitmap("RemoteDesktopIconGreen.ico");
                        //_form_slave.notifyIcon1.Icon = new Icon("RemoteDesktopIconGreen.ico");
                        //_form_slave.Icon = new Icon("RemoteDesktopIconGreen.ico");
                        break;
                    case TypeIcon.connectedInactive:
                        text = "Connected - Inactive";
                        theBitmap = new Bitmap("RemoteDesktopIconOrange.ico");
                        //_form_slave.notifyIcon1.Icon = new Icon("RemoteDesktopIconOrange.ico");
                        //_form_slave.Icon = new Icon("RemoteDesktopIconOrange.ico");
                        break;
                }
                IntPtr Hicon = theBitmap.GetHicon();// Get an Hicon for myBitmap.
                Icon newIcon = Icon.FromHandle(Hicon);
                Form1.getForm().BeginInvoke((Action)(() =>
                {
                    _form_slave.label2.Text = text;
                    _form_slave.notifyIcon1.Icon = newIcon;
                    _form_slave.Icon = newIcon;

                }));
          


            }
            catch (Exception exp)
            {
                exp = exp;
            }

        }
      

        public Form1()
        {
            _form_slave = this;//not after,because is used by other method called in constructor
            passwordValidator = new StringValidator(
                CommonClasses.DefaultSettings.MinPasswordLength,
                CommonClasses.DefaultSettings.MaxPasswordLength);

            InitializeComponent();
            
            
            //this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            //notifyIcon1.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);   
            

            socket = new SlaveSocket(); //Server in ascolto su localhost ad una porta di default 
            socket.setForm(this);
           // dispatcher = new InputDispatcher();

            /*dispatcherThread = new Thread(
                new ThreadStart(dispatcherThreadFunc));*/

            //socketThreadFunc();
            socketThread = new Thread(       socketThreadFunc);
           // socketThread.IsBackground = true;
           // dispatcherThread.Start();

            //MasterAcceptFunction(ns);
            socketThread.Start();
        }

        System.Threading.Timer timerTimeout;
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.MaxLength = CommonClasses.DefaultSettings.MaxPasswordLength;
            toolTip1.SetToolTip(textBox1,
                "Password must be " + CommonClasses.DefaultSettings.MinPasswordLength +
                "-" + CommonClasses.DefaultSettings.MaxPasswordLength +
                " characters long");
 
            toolTip1.SetToolTip(maskedTextBox1,
                "The port number is an integer value between 0-"+
                UInt16.MaxValue);
           // HideSlaveOptions();
            UpdateTarget(false);
            UpdateControls();
            AppStartup.CreateStartupFolderShortcut();
            setStatusGUI(TypeIcon.begin);
           // socketThreadFunc();

            timerTimeout = new System.Threading.Timer((ee) =>
            {
                bool ok = true;
                if (isConnectedMaster)
                {
                    try
                    {
                        ok = ok && CommonClasses.SocketMessages.TcpUtilities.isTcpActive(SlaveSocket.master);
                        ok = ok && CommonClasses.SocketMessages.TcpUtilities.isTcpActive(SlaveSocket.clipCli);
                    }
                    catch (Exception exp)
                    {
                        ok = false;
                    }
                    if (!ok) isConnectedMaster = false;
                }
            }, null, 0, 20*1000);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowSlaveOptions();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                socket.listener.Server.Close();
                socketThread.Abort();
                Socket.SlaveSocket.master.Close();
                Socket.SlaveSocket.clipCli.Close();
                Socket.SlaveSocket.commThread.Abort();
                Socket.SlaveSocket.clipThread.Abort();
            }
            catch (Exception exp) { }
            HideSlaveOptions();
            e.Cancel = true;
        }

        private void viewOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSlaveOptions();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChangeListeningPort(Int32.Parse(maskedTextBox1.Text));

            if (SlaveGlobalVars.Password !=textBox1.Text/* MD5PasswordHashing.GetMd5Hash(textBox1.Text)*/)
                SlaveGlobalVars.Password = textBox1.Text;

            UpdateControls();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HideSlaveOptions();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                AppStartup.CreateStartupFolderShortcut();
            else
                AppStartup.DeleteStartupFolderShortcuts(Path.GetFileName(Application.ExecutablePath));
        }

        private void maskedTextBox1_Validating(object sender, CancelEventArgs e)
        {
            if (Int32.Parse(maskedTextBox1.Text) > UInt16.MaxValue)
            {
                e.Cancel = true;
                maskedTextBox1.Select(0, maskedTextBox1.Text.Length);

                // Set the ErrorProvider error with the text to display. 
                this.errorProvider1.SetError(maskedTextBox1, maskedTextBox1.Text + " is not a correct port number");
            }
        }

        private void maskedTextBox1_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(maskedTextBox1, "");
        }



        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            //if(textBox1.Text.Length < CommonClasses.DefaultSettings.MinPasswordLength)
            if(passwordValidator.CanValidate(textBox1.Text.GetType()))
            {
                try
                {
                    passwordValidator.Validate(textBox1.Text);
                }
                catch (ArgumentException)
                {
                    e.Cancel = true;
                    textBox1.Select(0, textBox1.Text.Length);
                    this.errorProvider2.SetError(textBox1, "Password is too short or invalid chars are used");
                }
            }
        }

        private void textBox1_Validated(object sender, EventArgs e)
        {
            errorProvider2.SetError(textBox1, "");
        }


        private void HideSlaveOptions()
        {
            this.Visible = false;
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
        }

        private void ShowSlaveOptions()
        {
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.Visible = true;
        }

        private void UpdateControls()
        {
            label3.Text = socket.IPEndPoint;
            maskedTextBox1.Text = socket.IPEndPoint.Substring(socket.IPEndPoint.IndexOf(":") + 1);
            textBox1.Text = SlaveGlobalVars.Password;
        }
   
        public void UpdateTarget(bool isTarget)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<bool>(UpdateTarget), new object[] { isTarget });
                return;
            }
            /*if (isTarget)
                label2.Text = "This machine is currently the target of the master";
            else
                label2.Text = "Waiting to become target of the master...";*/

        }

        private static Form1 _form_slave = null;
        public static Form1 getForm()
        {
            /*Form f = Form1.ActiveForm;
            if (f != null)
                return (Form1)f;
            else throw new ArgumentNullException();*/
            return _form_slave;
        }



        //Effettua la modifica della porta generando un nuovo socket in ascolto, dopo aver chiuso il precedente
        //E' necessario richiamare WaitForMasterConnection() per iniziare di nuovo l'ascolto sulla porta specificata
        public void ChangeListeningPort(Int32 newPort)
        {
            try
            {
                
                Int32 oldPort = ((IPEndPoint)socket.listener.LocalEndpoint).Port;

                if (newPort != oldPort)
                {
                    //string address = ((IPEndPoint)li.LocalEndpoint).Address.ToString();
                    string address = "127.0.0.1";

                    socket.listener.Server.Close();
                    //socket.listener.Stop();
                    socketThread.Abort();

                    try
                    {
                        isConnectedMaster = false;
                    }
                    catch (Exception exp) { }

                    socket = new SlaveSocket(newPort);
                    socket.setForm(this);
                    socketThread = new Thread(socketThreadFunc);
                    socketThread.IsBackground = true;
                    socketThread.Start();
                }
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("No form available to display infos");
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                timerTimeout.Dispose();
                socket.listener.Stop();
                socketThread.Abort();
            }
            catch (Exception exp) { }
            Environment.Exit(Environment.ExitCode);
        }
        public void setLabel(string str)
        {
            lblMsg.Text = str;
        }

        public Point pointMouse
        {
            set
            {
                //Control.M
                Cursor.Position = value;
            }
            get
            {
                return Cursor.Position;
            }
        }
        public Rectangle getScreenSize()
        {
            return Screen.FromControl(this).Bounds;
        }

    


    }
}
