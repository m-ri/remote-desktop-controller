using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using CommonClasses.Win32;
using CommonClasses.SocketMessages;
using System.Threading;
using SlaveMain.Dispatcher;

namespace SlaveMain.Socket
{
    /// <summary>
    /// Slave Socket waits for INPUT messages on the TCP socket, deserializes them 
    /// and sends them over a shared CuncurrentQueue to the dispatcher
    /// </summary>
      
    class SlaveSocket
    {
        private TcpListener li;

        static System.Windows.Forms.Form formUI;
        public void setForm(System.Windows.Forms.Form form)
        {
            formUI = form;
        }

        private bool CheckCorrectPassword(NetworkStream stream)
        {
            Message m;
            byte[] buffer = new byte[Message.SIZE_INIZ];
            
            try
            {
                m = Message.ReadFromSocket(stream);
                if (m.typeMessage != Message.TypeMessage.AuthenticateRequest)
                {
                    if (m.typeMessage == Message.TypeMessage.PingEcho)
                    {
                        m = new Message(Message.TypeMessage.PingReply);
                        m.WriteToSocket(stream);
                    }

                    return false;
                }
                    
                //send challenge and see response
                string challenge = CommonClasses.SocketMessages.SecureChallenge.getString64(16);
                m = new Message(Message.TypeMessage.AuthentChallenge);
                m.setString(challenge);
                m.WriteToSocket(stream);
                m = Message.ReadFromSocket(stream);
                if (m.typeMessage != Message.TypeMessage.AuthentResponse) return false;
                string expected = CommonClasses.SocketMessages.SecureChallenge.fusePasswordChallenge(SlaveGlobalVars.Password, challenge);
                bool isVerified = (expected == m.getString());
                //bool isVerified = MD5PasswordHashing.VerifyMd5Hash(SlaveGlobalVars.Password, (byte[])m.data);
                //TODO: Define protocol to exchange status messages (OK, ERR, PW REFUSED...)
                Message message_sent=new Message();
                if (isVerified)
                {
                    message_sent.typeMessage = Message.TypeMessage.CorrectPassword;
                }
                else {
                    message_sent.typeMessage = Message.TypeMessage.WrongPassword;
                }
                message_sent.WriteToSocket(stream);
                stream.Flush();
                return isVerified;
            }
            catch (Exception e)
            {
                throw new Exception ("Error occurred during read from socket");
            }
        }
        /// <summary>
        /// Function executed when a slave accepts the master
        /// </summary>
        private static void MasterAcceptFunction(NetworkStream _stream)
        {
            Message m;
            NetworkStream stream = _stream;
            try
            {
                while (stream.CanRead && Form1.getForm().isConnectedMaster)
                {
                    try
                    {
                        m = Message.ReadFromSocket(stream);

                        if (m.typeMessage == Message.TypeMessage.Input)
                        {
                            SlaveGlobalVars.SocketToDispatcherQueue.Enqueue(m.Content<INPUT_EXTENDED>());

                            //formUI.BeginInvoke((Action)(() => { ((Form1)formUI).setLabel("get_000"); InputDispatcher.DispatchInput(); ((Form1)formUI).setLabel("get " + m.Content<INPUT>().mkhi.ki.Vk); }));
                            formUI.BeginInvoke((Action)(() => { /*((Form1)formUI).setLabel("get_000"); */InputDispatcher.DispatchInput();/* ((Form1)formUI).setLabel("get " + m.Content<INPUT>().mkhi.ki.Vk); */}));

                            //throw new Exception("ok");


                            //SlaveGlobalVars.SocketToDispatcherQueue.Add(m.Content<INPUT>());
                        }
                        else if (m.typeMessage == Message.TypeMessage.ClipboardAttached)
                        {//TODO move to another socket

                            formUI.BeginInvoke((Action)(() => { ((Form1)formUI).setLabel("clipboard copying"); CommonClasses.ManageClipboard.setCurrentClipboard(m); ((Form1)formUI).setLabel("clipboard copied"); }));
                        }
                        else if (m.typeMessage == Message.TypeMessage.Error)
                        {
                            SlaveGlobalVars.isTarget = false;
                            return;
                        }
                        else if (m.typeMessage == Message.TypeMessage.CloseConnection)
                        {
                            Form1.getForm().isConnectedMaster = false;
                            return;//in some case is not reached
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(10);//TODO use async
                        }
                    }
                    catch (Exception e)
                    {
                        int a;
                        //no problem?
                    }
                }
            }
            catch (Exception e)
            {
                Form1.getForm().isConnectedMaster = false;
                //Console.WriteLine("Can't read from socket anymore");
            }
        }

        //public SlaveSocket(String localIPAddress)
        public SlaveSocket()
        {
            try
            {
                li = new TcpListener(IPAddress.Any, CommonClasses.DefaultSettings.ListeningPort);
                
                //li = new TcpListener(new IPEndPoint(IPAddress.Parse(localIPAddress), CommonClasses.DefaultSettings.listeningPort));
            }
            /*catch (FormatException)
            {
                throw new Exception("The address isn't a valid IP address");
            }*/
            catch (Exception)
            {
                /*throw new Exception("Couldn't open a new tcp listener on the address " + localIPAddress + ":"
                    + CommonClasses.DefaultSettings.listeningPort);*/
                throw new Exception("Couldn't open a new tcp listener on the port "+ CommonClasses.DefaultSettings.ListeningPort);
            }
        }

        static int currentPort = CommonClasses.DefaultSettings.ListeningPort;
       // public SlaveSocket(String localIPAddress, Int32 port)
        public SlaveSocket(Int32 port)
        {
            try{
                //li = new TcpListener(new IPEndPoint(IPAddress.Parse(localIPAddress), port));
                li = new TcpListener(IPAddress.Any, port);
                currentPort = port;
            }
            /*catch(FormatException){
                throw new Exception("The address isn't a valid IP address");
            }*/
            catch(Exception){
                //throw new Exception("Couldn't open a new tcp listener on the address " + localIPAddress +":" + port);
                throw new Exception("Couldn't open a new tcp listener on the port " + port);
            }
        }
        //connections must survice when the listener is deleted and recreated(change port)
        static public  volatile TcpClient master;
        static volatile NetworkStream stream;

        static public volatile TcpClient clipCli;
        static volatile NetworkStream clipNs;

        //Task clipTask;
        static public Thread clipThread;
        static public Thread commThread;
        public void WaitForMasterConnection()
        {
            try
            {
                li.Start();

                //bool looped = true;
                while (true)
                {
                    try
                    {
                    //looped = false;
                        TcpClient pendingConnection=li.AcceptTcpClient();
                       
                        //if (Form1.getForm().isConnectedMaster) { master.Close(); continue; }
                        master = pendingConnection;
                        master.NoDelay = true;//otherwise waits to full buffer
                        //master.ReceiveTimeout = CommonClasses.DefaultSettings.tcpRxTimeout;
                        stream = master.GetStream();
                    //System.Windows.Forms.MessageBox.Show( master.Client.RemoteEndPoint.ToString());
                       
                            if (CheckCorrectPassword(stream))
                            {
                                SlaveGlobalVars.form =(Form1) formUI;
                                SlaveGlobalVars.isTarget = true;
                                clipCli = li.AcceptTcpClient();
                                clipNs = clipCli.GetStream();
                                //System.Windows.Forms.MessageBox.Show(clipCli.Client.RemoteEndPoint.ToString());
                                Message m = Message.ReadFromSocket(clipNs);
                                if (m.typeMessage == Message.TypeMessage.ClipboardSocketRequest )
                                {
                                    m = new Message(Message.TypeMessage.ClipboardSocketOk);
                                    m.WriteToSocket(clipNs);
                                    //clipTask = Task.Factory.StartNew(daemonClipboard);
                                    clipThread = new Thread( ()=>SlaveSocket.daemonClipboard(clipNs));
                                    clipThread.IsBackground = true;
                                    clipThread.Start();
                                    clipThread.IsBackground = true;

                                    commThread = new Thread(() => SlaveSocket.MasterAcceptFunction(stream));
                                    commThread.Start();
                                    commThread.IsBackground = true;
                                    Form1.getForm().isConnectedMaster = true;
                                    Form1.setStatusGUI(Form1.TypeIcon.connectedInactive);

                                }
                                else
                                {
                                    throw new Exception("error during creation of clipboard socket");
                                }
                            }
                        }
                        catch (Exception e)
                        {           
                            Console.WriteLine(e.Message);
                            Form1.getForm().isConnectedMaster = false;

                        }
                        finally
                        {
                            //stream.Close();
                            //master.Close();
                        }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                
                //If the error is represented by an invalid choice of address-port for the socket,
                //notify the user to change the port
                if (e.SocketErrorCode == SocketError.AddressAlreadyInUse ||
                    e.SocketErrorCode == SocketError.AddressNotAvailable ||
                    e.SocketErrorCode == SocketError.AccessDenied )
                    throw new SockAddrAlreadyInUseException();
            }
        }
        private static void daemonClipboard(NetworkStream clipStream)
        {
            try
            {
                int contExp = 0;

                Message m = Message.ReadFromSocket(clipStream);
                                //if (true /*m.status == Message.Status.CliSocketRequest*/ )
                                //{
                                    m = new Message(Message.TypeMessage.ClipboardSocketOk);
                                    m.WriteToSocket(clipStream);


                while (clipStream.CanRead && Form1.getForm().isConnectedMaster)
                {
                    try
                    {
                        m = Message.ReadFromSocket(clipStream);
                        if (m.typeMessage == Message.TypeMessage.ClipboardAttached)
                        {

                            Form1.getForm().BeginInvoke((Action)(() => CommonClasses.ManageClipboard.setCurrentClipboard(m)));
                            //TODO send confirmation end
                        }
                        else if (m.typeMessage == Message.TypeMessage.ClipboardRequest)
                        {
                            m = (CommonClasses.SocketMessages.Message)Form1.getForm().Invoke(new Func<CommonClasses.SocketMessages.Message>(() => CommonClasses.ManageClipboard.getCurrentClipboardMessage()));
                            m.WriteToSocket(clipStream);
                        }
                        else if (m.typeMessage == Message.TypeMessage.CloseConnection)
                        {
                            return;
                            //I didn't manage here
                        }
                        contExp=0;
                    }
                    catch (Exception exp)
                    {
                        Exception e=exp;
                        contExp++;
                        if (contExp > 5) throw e;
                        //formUI.BeginInvoke((Action)(() => System.Windows.Forms.MessageBox.Show("Problem during copy of clipboard " + e.Message)));
                    }

                }
            }
            catch (Exception e)
            {
                Form1.getForm().isConnectedMaster = false;
                //throw e;
            }
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
        //public string IPEndPoint { get { return li.LocalEndpoint.ToString(); } }
        public string IPEndPoint { get { return GetLocalIPAddress() + ":" + currentPort; } }

        public TcpListener listener { get { return li; } }
    }
}
