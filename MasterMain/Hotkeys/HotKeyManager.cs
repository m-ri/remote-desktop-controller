using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasterMain.Hotkeys
{
    class HotKeyManager
    {

        private Dictionary<int, long> keysPressed = new Dictionary<int, long>();//used only by KeyboardListener
        protected long _count_hotKey = 0;

        private static bool detectingNewPrefix = false;
        private static HashSet<int> newPrefix = new HashSet<int>();

        private static int[] prefix = { (int)Keys.LControlKey, (int)Keys.LMenu };
        public static int[] getPrefix()
        {
            return prefix;
        }
        public static void detectNewPrefixStart()
        {
            newPrefix.Clear();
            //hotKeys.Clear();
            
            detectingNewPrefix = true;
        }
        public static void detectNewPrefixEnd()
        {
            if (newPrefix.Count > 0)
            {
                prefix = new int[newPrefix.Count];
                newPrefix.CopyTo(prefix);
            }

            detectingNewPrefix = false;
        }
        protected long getNewIdHotKey
        {
            get
            {
                _count_hotKey++;
                return _count_hotKey;
            }
        }
        static Dictionary<long, HotKey> hotKeys=new Dictionary<long,HotKey>();
        public long insertHotKey(HotKey hotKey)
        {
            long id_hotKey = getNewIdHotKey;
            hotKeys.Add(id_hotKey, hotKey);
            return id_hotKey;
        }
        public bool removeHotKey(long idHotKey)
        {
            return hotKeys.Remove(idHotKey);
        }

        public void removeAllHotKeys()
        {
            hotKeys.Clear();
        }

        public class HotKey
        {
            public HotKey(int[] keys, Action action, bool callOnce = true,bool propagateLastKey=true)
            {
                this.keys = keys;
                this.action = action;
                this.callOnce = callOnce;
                this.propagateLastKey = propagateLastKey;
            }
            internal int[] keys;
            internal Action action;
            internal bool callOnce;
            internal bool propagateLastKey;


        }

        HashSet<long> currentCallbacks=new HashSet<long>();
        long timeLastClean = 0;
        //return if propagate presse key
        public bool newKeyUp(int key)
        {
            if ((getMilliseconds() - timeLastClean) > 1500)
            {
                cleanPressed();
                timeLastClean = getMilliseconds();
            }
            bool propagateKey = true;

            if (keysPressed.ContainsKey(key))
            {
                keysPressed.Remove(key);
            }
            keysPressed.Add(key,getMilliseconds());
            if (detectingNewPrefix) newPrefix.Add(key);
            foreach (KeyValuePair<long, HotKey> kvp in hotKeys)
            {
                bool is_present = checkHotKey(kvp.Key);
                if (is_present)
                {
                    if (!kvp.Value.callOnce)
                    {
                        kvp.Value.action();
                        propagateKey = propagateKey && kvp.Value.propagateLastKey;
                        currentCallbacks.Add(kvp.Key);
                    }
                    else if (!currentCallbacks.Contains(kvp.Key))
                    {
                        kvp.Value.action();
                        propagateKey = propagateKey && kvp.Value.propagateLastKey;
                        currentCallbacks.Add(kvp.Key);
                    }
                }
                else
                {
                    currentCallbacks.Remove(kvp.Key);
                }
            }
            return propagateKey;
        }
        public void newKeyDown(int key)
        {
            
            keysPressed.Remove(key);
            foreach (long idHotKey in hotKeys.Keys)
            {
                if (currentCallbacks.Contains(idHotKey))
                {
                    if (!checkHotKey(idHotKey))
                    {
                        currentCallbacks.Remove(idHotKey);
                    }

                }
            }
        }

        protected bool checkHotKey(long idHotKey)
        {
            bool ok = true;
            HotKey currentHotKey=hotKeys[idHotKey];
            foreach (int key in currentHotKey.keys)
                if (!keysPressed.ContainsKey(key)) ok = false;

            return ok;
        }

        private long getMilliseconds()
        {
            return (long) DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }
        private void cleanPressed()
        {
            long minAge = getMilliseconds() - 3000;

            List<int> listRemove=new List<int>();
            foreach (int key in keysPressed.Keys)
            {
                if (keysPressed[key] < minAge) listRemove.Add(key);
            }
            listRemove.ForEach((elem) => keysPressed.Remove(elem));

        }

    }
}
