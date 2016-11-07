using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using CommonClasses.Win32;
using CommonClasses.SocketMessages;
using System.Windows.Forms;

namespace SlaveMain
{
    public static class SlaveGlobalVars
    {
        //public static BlockingCollection<INPUT> SocketToDispatcherQueue = new BlockingCollection<INPUT>();
        public static ConcurrentQueue<INPUT_EXTENDED> SocketToDispatcherQueue = new ConcurrentQueue<INPUT_EXTENDED>();

        //public static readonly object _lockTargetObject = new object();
        //Non è sicuro se necessario un locking sulla variabile isTarget
        //Al momento, essa è gestita e letta unicamente dal thread che gestisce il socket


        //Definisce se il server è target del master, e in tal caso è possibile visualizzare un'informazione
        //diversa nella GUI (stato del sistema)
        private static bool _isTarget = false;
        
        public static bool isTarget
        {
            set
            {
                try
                {
                    _isTarget = value;
                    _form.BeginInvoke((Action)(() => _form.UpdateTarget(value)));
                    //Form1.getForm().UpdateTarget(value);
                }
                catch (Exception e)
                {
                    e = e;
                }
            }
            get
            {
                return _isTarget;
            }
        }

        private static Form1 _form=null;
        public static Form1 form
        {
            set
            {
                _form = value;
            }
        }
     
        private static string _password = CommonClasses.DefaultSettings.DefaultPassword; //Password di default vuota
        public static string Password
        {
            set
            {
                _password = value;
            }
            get
            {
                return _password;
                //return MD5PasswordHashing.GetMd5Hash(_password);
            }
        }

       
    }
}
