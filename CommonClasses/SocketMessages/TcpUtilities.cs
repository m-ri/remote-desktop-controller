using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace CommonClasses.SocketMessages
{
    public class TcpUtilities
    {
        //Thanks to http://stackoverflow.com/questions/1387459/how-to-check-if-tcpclient-connection-is-closed
        /// <summary>
        /// Checks if a TcpClient is still open and valid
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool isTcpActive(TcpClient client)
        {
            bool isConnectionStillOk = true;

            if (client == null || !client.Connected)
            {
                isConnectionStillOk = false;
                return isConnectionStillOk;
            }

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections().Where(x => x.LocalEndPoint.Equals(client.Client.LocalEndPoint) && x.RemoteEndPoint.Equals(client.Client.RemoteEndPoint)).ToArray();
           
            if (tcpConnections != null && tcpConnections.Length > 0)
            {
                TcpState stateOfConnection = tcpConnections.First().State;
                if (stateOfConnection == TcpState.Established)
                {
                    // Connection is OK
                }
                else
                {
                    isConnectionStillOk = false; // No active tcp Connection to hostName:port
                }

            }
            else
            {
                isConnectionStillOk = false;
            }
            //other method
            if (client.Client.Poll(0, SelectMode.SelectRead))
            {
                if (!client.Connected) isConnectionStillOk = false;
                else
                {
                    byte[] b = new byte[1];
                    try
                    {
                        if (client.Client.Receive(b, SocketFlags.Peek) == 0)
                        {
                            // Client disconnected
                            isConnectionStillOk = false;
                        }
                    }
                    catch { isConnectionStillOk = false; }
                }
            }

            return isConnectionStillOk;
        }
    }
}
