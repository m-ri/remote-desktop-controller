using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SlaveMain.Socket
{
    class ThreadConnection
    {
        private TcpListener listener;
        public void start(ref Form1 form,int port)
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
            }
            catch (FormatException)
            {
                throw new Exception("The address isn't a valid IP address");
            }
            catch (Exception)
            {
                throw new Exception("Couldn't open a new TCP listener on the port "
                    + listener);
            }


            TcpClient client = listener.AcceptTcpClient();

            NetworkStream ns = client.GetStream();
            byte[] b=new byte[100];
            ns.Read(b,0,100);
            
            

            

        }
    }
}
