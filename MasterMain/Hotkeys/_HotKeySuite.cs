using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterMain.Hotkeys
{
    
    public sealed class HotKeySuite : List<HotKey>
    {

        private delegate void KeyChainHandler(KeyEventMoreArgs kex);

        private KeyChainHandler m_keyChain;

        ///<summary>
        /// Adds a HotKey to the collection.
        ///</summary>
        ///<param name="hks"></param>
        public new void Add(HotKey hks)
        {
            m_keyChain += hks.OnKey;
            base.Add(hks);
        }

        ///<summary>
        /// Removes the HotKey from the collection.
        ///</summary>
        ///<param name="hks"></param>
        public new void Remove(HotKey hks)
        {
            m_keyChain -= hks.OnKey;
            base.Remove(hks);
        }

        internal void OnKey(KeyEventMoreArgs e)
        {
            if (m_keyChain != null)
                m_keyChain(e);
        }

    }
}
