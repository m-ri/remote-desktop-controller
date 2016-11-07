using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClasses
{
    public class DefaultSettings
    {
        public static Int32 ListeningPort = 1500;
        public static Int32 AutoDiscoveryListeningUDPPort = 1500;

        public static Int32 AutoDiscoveryIntervalSeconds = 20;
        public const string AutoDiscoveryExpectedString = "qwe78xd427,dsf5fdw7j8n8r.f";
        public const String DefaultPassword = "Pass8w9";
        public static int MaxPasswordLength = 20;
        public static int MinPasswordLength = 4;
        //public static int tcpRxTimeout = 100;//milliseconds
    }
}
