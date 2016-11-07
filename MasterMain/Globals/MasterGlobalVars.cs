using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonClasses.Win32;
using System.Security.Cryptography;

namespace MasterMain
{
    public static class MasterGlobalVars
    {
        //public static ConcurrentQueue<INPUT_EXTENDED> HookingToSocketQueue = new ConcurrentQueue<INPUT_EXTENDED>();
        public static BlockingCollection<INPUT_EXTENDED> HookingToSocketQueue = new BlockingCollection<INPUT_EXTENDED>();
    }
}
