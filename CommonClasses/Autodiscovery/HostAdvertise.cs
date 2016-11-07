using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace CommonClasses.Autodiscovery
{
  /*  static class  HostAdvertise
    {
        private static volatile bool _isRunning = false;
        private static Thread _thread = null;

        public static void start()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                _thread = new Thread(() => run());
                _thread.Start();
            }
        }
        public static void stop()
        {
            _isRunning = false;
        }

        private static void run()
        {
            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, DefaultSettings.AutoDiscoveryListeningUDPPort);
            byte[] bytes = Encoding.ASCII.GetBytes("Hello by RemoteController " + DefaultSettings.AutoDiscoveryExpectedString);
            
            while (_isRunning)
            {
                try
                {
                    client.Send(bytes, bytes.Length, ip);
                }
                catch (Exception e)//Connection can temporally go down
                {
                }
                Thread.Sleep(TimeSpan.FromSeconds(DefaultSettings.AutoDiscoveryIntervalSeconds));
            }
            client.Close();
        }


    }*/
}
