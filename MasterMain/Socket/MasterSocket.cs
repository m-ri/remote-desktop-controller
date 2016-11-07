using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using CommonClasses.Win32;
using CommonClasses.SocketMessages;

namespace MasterMain.Socket
{
     /// <summary>
    /// Master Socket waits for INPUT messages on a CuncurrentQueue 
    /// and serializes them over a TCP socket
    /// </summary>
    class MasterSocket
    {
        internal class Slave
        {
            private IPEndPoint ep;
            public Slave(String slaveIPAddress, Int32 port,String password=CommonClasses.DefaultSettings.DefaultPassword)
            {
                try
                {
                    _port = port;
                    ep = new IPEndPoint(IPAddress.Parse(slaveIPAddress), port);
                    Password = password;
                    focused = false;
                    taskClip = Task.Factory.StartNew(() => { ;});
                }
                catch (FormatException)
                {
                    throw;
                }
            }
            public void close()
            {
                try
                {
                    Message m = new Message(Message.TypeMessage.CloseConnection);
                    if(!mustBeDeleted) m.WriteToSocket(ns);
                }catch (Exception e) { }
                try
                {
                    cli.Close();
                }
                catch (Exception e) { }
                try
                {
                    Message m = new Message(Message.TypeMessage.CloseConnection);
                    if (!mustBeDeleted) m.WriteToSocket(clipboard_ns);
                }
                catch (Exception e) { }
                try
                {
                    clipboard_cli.Close();
                }
                catch (Exception e) { }
                try
                {
                    threadMessage.Abort();
                    taskClip.Dispose();
                }
                catch (Exception e) { }
            }
            public IPEndPoint location {
                get { return ep; }
            }
            private Int32 _port;
            public Int32 Port { get { return _port; } }
            public bool mustBeDeleted = false;//when is marked,the timer will kill him
            public string Password { get; set; }
            //socket with inputs/commands
            internal TcpClient cli;
            internal NetworkStream ns;

            internal TcpClient clipboard_cli;
            internal NetworkStream clipboard_ns;

            internal volatile bool focused;

            internal Thread threadMessage;
            public Task taskClip;//other classes will enqueue tasks
        }

        //public static TcpClient cli;//TODO change to private
        //public static NetworkStream ns;
        private static Dictionary<int, Slave> slaves=new Dictionary<int,Slave>();
        public static Slave getSlave(int ID)
        {
            return slaves[ID];
        }
          public int[] getIdSlaves()
         {
             int[] ret = new int[slaves.Count];
             slaves.Keys.CopyTo(ret,0);
             return ret;

         }

        public MasterSocket(Int32 port)
        {
            try
            {
                //cli = new TcpClient();
               // cli = new TcpClient(new IPEndPoint(IPAddress.Any, port));
               // slaves = new Dictionary<int,Slave>();
            }
            catch (Exception)
            {
                throw new Exception("Couldn't open a new tcp client on the address " + IPAddress.Any + ":" + port);
            }
        }

       
        /// <summary>
        /// Method used to add a new slave in the list of "known ones".
        /// It returns ID
        /// </summary>
        public int AddNewSlave(String slaveIPAddress, Int32 port,String password=CommonClasses.DefaultSettings.DefaultPassword, int ID=-1)
        {
            try
            {
                Slave s = new Slave(slaveIPAddress, port,password);


                if (ID>=0 && slaves.ContainsKey(ID))
                    throw new Exception("Duplicate key for slaves list");
                if (ID < 0)
                {
                    ID = 0;
                    while (slaves.ContainsKey(ID)) { ID++; }
                    /*ID = -1;
                    foreach (int key in slaves.Keys)
                    {
                        if (key > ID) ID = key;
                    }
                    ID++;*/
                }
                slaves[ID] = s;
                return ID;
            }
            catch (FormatException)
            {
                throw new Exception("The address of the slave isn't a valid IP address");
            }
        }
        public void removeSlave(int ID)
        {
             try
            {
               slaves[ID].close();
            }
            catch (Exception e) { }
            
            slaves.Remove(ID);
        }

        /*public int  AddNewSlave(String slaveIPAddress, int ID=-1)
        {
            return AddNewSlave(slaveIPAddress, CommonClasses.DefaultSettings.listeningPort,ID);
        }*/

        public void ConnectToSlave(int ID)
        {
            if (slaves.ContainsKey(ID))
                ConnectToSlave(ID, slaves[ID].Password);
            else throw new Exception("Incorrect slave specified! Connection refused.");
        }


        /// <summary>
        /// Function executed when the master connects to a slave, it acts by default taking the INPUT structures from the 
        /// blocking queue and then sending them over the socket through a Message payload
        /// </summary>
        public void ConnectToSlave(int ID, string password)
        {
            slaves[ID].cli = null;
            slaves[ID].ns = null;
            slaves[ID].clipboard_cli = null;
            slaves[ID].clipboard_ns = null;

            Message m = new Message(Message.TypeMessage.Input) ;
           
            if (!slaves.ContainsKey(ID))
                throw new Exception("The Slave requested isn't available");

            TcpClient cli = new TcpClient();

            try
            {
                //cli = new TcpClient();
                cli.NoDelay = true;
                //cli.ReceiveTimeout = CommonClasses.DefaultSettings.tcpRxTimeout;
                cli.Connect(slaves[ID].location);
                cli.NoDelay = true;//otherwise waits to full buffer
                //cli.ReceiveTimeout = CommonClasses.DefaultSettings.tcpRxTimeout;
            }
            catch (SocketException e1)
            {
                Console.WriteLine("SocketException: {0}", e1);
                throw new Exception("Couldn't connect the client to the listener on the address " + slaves[ID].location);
            }

            NetworkStream ns = cli.GetStream();


            if (!ns.CanWrite)
                throw new Exception("Master can't write to socket");
            try
            {
                if (AuthenticateMaster(ns,password))
                {
                    //!! useless??slaves[ID].Password = password;
                    /*slaves[ID].cli = cli;
                    slaves[ID].ns = ns;*/

                    TcpClient clipCli = new TcpClient(); 
                    clipCli.Connect(slaves[ID].location);
                    //IPEndPoint p;
                    
                    //clipCli.NoDelay = false;
                    NetworkStream clipNs = clipCli.GetStream();
                    m = new Message(Message.TypeMessage.ClipboardSocketRequest);
                    m.WriteToSocket(clipNs);
                    m = Message.ReadFromSocket(clipNs);
                    if (m.typeMessage == Message.TypeMessage.ClipboardSocketOk)
                    {
                        slaves[ID].cli = cli;
                        slaves[ID].ns = ns;

                        slaves[ID].clipboard_cli = clipCli;
                        slaves[ID].clipboard_ns = clipNs;
                    }
                    else
                    {
                        throw new Exception("problem during creation of clipboard socket");
                    }

                    /*while (stream.CanWrite)
                    {
                         INPUT inputToSend;
                        
                         if (MasterGlobalVars.HookingToSocketQueue.TryDequeue(out inputToSend))
                         {

                             m.data = inputToSend;
                             m.WriteToSocket(stream);
                         }
                         else
                         {
                             System.Threading.Thread.Sleep(10);
                         }
                    }*/

                    //!! terminare@@@@
                    //sendQueuedInput();
                }
                else
                {
                    Form1.getForm().BeginInvoke((Action)(()=>System.Windows.Forms.MessageBox.Show("Password errata")));
                    //...ask for new password or select new slave...
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Can't write to socket anymore");
            }
            finally
            {
               // ns.Close();
            }
        }
        public static void SlaveEnableDisable(int ID, bool enable)
        {
            if (!slaves.ContainsKey(ID)) throw new Exception("ID Slave not available");
            if (enable)
            {
                Form1.setIdSlaveFocus(ID);
                slaves[ID].focused = true;
                slaves[ID].threadMessage = new Thread(()=>sendQueuedInput(ID));
                slaves[ID].threadMessage.IsBackground = true;
                slaves[ID].threadMessage.Start();
                INPUT_EXTENDED input_extended = new INPUT_EXTENDED();
                input_extended.isNotInput = true; input_extended.focusChange = true; input_extended.focusActivateDeactivate = true;
                MasterGlobalVars.HookingToSocketQueue.Add(input_extended);
            }
            else
            {
                if (Form1.getIdSlaveFocus() == ID)
                {
                    Form1.setIdSlaveFocus(ID);
                    Hooks.MouseListener.blockLocalInput = Hooks.KeyboardListener.blockLocalInput = false;
                    Hooks.MouseListener.blockRemoteInput = Hooks.KeyboardListener.blockRemoteInput = true;
                }
                //slaves[ID].focused = false;
                INPUT_EXTENDED input_extended=new INPUT_EXTENDED();
               /* input_extended.focusChange = true;
                input_extended.focusActivateDeactivate = false;
                MasterGlobalVars.HookingToSocketQueue.Add(input_extended);*/

                slaves[ID].focused = false;
                if (slaves[ID].mustBeDeleted) return;
                input_extended = new INPUT_EXTENDED();
                input_extended.not_send=true;//I wake up consumer in order to terminate it(and not consume other INPUT)
                MasterGlobalVars.HookingToSocketQueue.Add(input_extended);
            }

        }


        public static bool pendingClipboard = false;
        public static void sendQueuedInput(int ID)
        {
            var ns = slaves[ID].ns;
            if (slaves[ID].mustBeDeleted) return;
            try
            {
                //NetworkStream stream = cli.GetStream();
                if (ns.CanWrite)
                {
                    INPUT_EXTENDED input_extended;
                    int numExc = 0;
                    //while (MasterGlobalVars.HookingToSocketQueue.TryDequeue(out input_extended))
                    while (ns.CanWrite)
                    {
                        try
                        {
                            {
                                Message m = new Message(Message.TypeMessage.Input);
                                input_extended = MasterGlobalVars.HookingToSocketQueue.Take();



                                //INPUT inputToSend = input_extended.input;
                                m.data = input_extended;
                                if (!input_extended.not_send) m.WriteToSocket(ns);
                                if (!slaves[ID].focused)
                                {
                                    m = new Message(Message.TypeMessage.Input);
                                    input_extended = new INPUT_EXTENDED();
                                    input_extended.focusChange = true; input_extended.focusActivateDeactivate = false; input_extended.isNotInput = true;
                                    m.WriteToSocket(ns);
                                    return;
                                }
                            }
                            if (MasterGlobalVars.HookingToSocketQueue.Count() > 2)
                            {
                                int a = 1;//debug purpose
                            }
                            if (pendingClipboard && slaves[ID].focused)
                            {
                                /* Message m = new Message(Message.Status.ClipboardAttached);//FIXED
                                 m = CommonClasses.ManageClipboard.getCurrentClipboardMessage();
                                 m.WriteToSocket(ns);
                                 ns.Flush();
                                 pendingClipboard = false;*/
                            }
                            numExc = 0;
                        }
                        catch (Exception e)
                        {
                            numExc++;
                            if (numExc > 4)
                            {
                                slaves[ID].mustBeDeleted = true;
                                slaves[ID].focused = false;
                                Form1.setIdSlaveFocus(-1);
                                Hooks.KeyboardListener.blockLocalInput = false; Hooks.KeyboardListener.blockRemoteInput = true;
                                Hooks.MouseListener.blockLocalInput = false; Hooks.MouseListener.blockRemoteInput = true;
                                return;
                            }
                        }
                    }
                }

            }
            catch (Exception e)//be careful!! è facile creare dipendenze circolari in fase di chiusura
            {
                slaves[ID].mustBeDeleted = true;
                slaves[ID].focused = false;
                Form1.setIdSlaveFocus(-1);
                Hooks.KeyboardListener.blockLocalInput = false; Hooks.KeyboardListener.blockRemoteInput = true;
                Hooks.MouseListener.blockLocalInput = false; Hooks.MouseListener.blockRemoteInput = true;
                return;
            }
        }

        private bool AuthenticateMaster(NetworkStream stream, string password)
        {
            //string MD5Password = MD5PasswordHashing.GetMd5Hash(password);
            //byte[] MD5PasswordBytes = MD5PasswordHashing.GetMd5HashBytes(password);
            Message m=new Message(); 
            try
            {
                //m = new Message(Message.Status.Authenticate,MD5PasswordBytes);
                /*m.status = Message.Status.Authenticate;
                m.data = MD5PasswordBytes;*/
                m = new Message(Message.TypeMessage.AuthenticateRequest);
                m.WriteToSocket(stream);
                
                //TODO: Define protocol to exchange status messages (OK, ERR, PW REFUSED...)
                m = Message.ReadFromSocket(stream);
                if (m.typeMessage != Message.TypeMessage.AuthentChallenge) return false;
                String challenge = m.getString();
                m = new Message(Message.TypeMessage.AuthentResponse);
                m.setString(CommonClasses.SocketMessages.SecureChallenge.fusePasswordChallenge(password, challenge));
                m.WriteToSocket(stream);
                m = Message.ReadFromSocket(stream);

            }
            catch (Exception e)
            {
                m.typeMessage = Message.TypeMessage.Error;
                m.WriteToSocket(stream);
                throw new Exception("Error while authenticating slave on the socket");
            }
            //TODO: Define protocol to exchange status messages (OK, ERR, PW REFUSED...)
            if (m.typeMessage == Message.TypeMessage.CorrectPassword)
            {
                return true;
            }
            else if (m.typeMessage == Message.TypeMessage.WrongPassword)
            {
                return false;
            }
            else return false;
        }

        /// <summary>
        /// Method used to disconnect permanently from a Slave-Server
        /// </summary>
        public void DeleteSlave(int ID)
        {
            if(slaves.ContainsKey(ID))
                slaves.Remove(ID);
        }

        public void Close()
        {
            foreach(Slave slave in slaves.Values)
                try
                {
                    slave.cli.Close();
                    slave.clipboard_cli.Close();
                }
                catch (Exception e) { }

            slaves.Clear();
            //cli.Close();
        }
    }
}
