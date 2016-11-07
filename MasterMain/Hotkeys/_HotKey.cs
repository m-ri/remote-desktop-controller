using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasterMain.Hotkeys
{
    public class HotKey
    {
        private readonly Dictionary<Keys, bool> m_hotkeystate;     //Keeps track of the status of the set of Keys

        //These provide the actual status of whether a set is truly activated or not.
        private int m_hotkeydowncount;   //number of hot keys down
      
        private readonly IEnumerable<Keys> m_hotkeys;  //hot-keys provided by the user.
        private bool m_enabled = true;       //enabled by default

        ///<summary>
        ///A delegate representing the signature for the OnHotKeysDownHold event
        ///</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        public delegate void HotKeyHandler( object sender, EventArgs e );

        ///<summary>
        ///Called as the user holds down the keys in the set.  It is NOT triggered the first time the keys are set. <see cref="OnHotKeysDownOnce"/>
        ///</summary>
        public event HotKeyHandler OnHotKeysDownHold;

        /// <summary>
        /// Called whenever the hot key set is no longer active.  This is essentially a KeyPress event, indicating that a full 
        /// key cycle has occurred, only for HotKeys because a single key removed from the set constitutes an incomplete set.
        /// </summary>
        public event HotKeyHandler OnHotKeysUp;

        /// <summary>
        /// Called the first time the down keys are set.  It does not get called throughout the duration the user holds it but only the 
        /// first time it's activated.
        /// </summary>
        public event HotKeyHandler OnHotKeysDownOnce;

        /// <summary>
        /// General invocation handler
        /// </summary>
        /// <param name="hotKeyDelegate"></param>
        private void InvokeHotKeyHandler( HotKeyHandler hotKeyDelegate )
        {
            if ( hotKeyDelegate != null )
                hotKeyDelegate( this, new EventArgs() );
        }

        /// <summary>
        /// Static method which builds a new hot-key from an IEnumerable of Keys and 
        /// binds to the new object its Down(Once/Hold) and Up events
        /// </summary>
        /// <param name="hotKeyDelegate"></param>
        internal static HotKey BindHotKeyToHandlers(IEnumerable<Keys> ks,
                              HotKey.HotKeyHandler onEventDownOnce,
                               HotKey.HotKeyHandler onEventDownHold,
                               HotKey.HotKeyHandler onEventUp)
        {
            HotKey hks = new HotKey(ks);


            hks.OnHotKeysDownOnce += onEventDownOnce;    //The first time the key is down
            hks.OnHotKeysDownHold += onEventDownHold;    //Fired as long as the user holds the hot keys down but is not fired the first time.
            hks.OnHotKeysUp += onEventUp;                //Whenever a key from the set is no longer being held down

            return hks;

        }


        ///<summary>
        /// Creates an instance of the HotKeySet class.  Once created, the keys cannot be changed.
        ///</summary>
        ///<param name="hotkeys">Set of Hot Keys</param>
        public HotKey ( IEnumerable<Keys> hotkeys )
        {
            m_hotkeystate = new Dictionary<Keys, bool>();
            m_hotkeys = hotkeys;
            InitializeKeys();
        }

        /// <summary>
        /// Adds the keys into the dictionary tracking the keys and gets the real-time status of the Keys
        /// from the OS
        /// </summary>
        private void InitializeKeys()
        {
            foreach ( Keys k in HotKeys )
            {
                if (m_hotkeystate.ContainsKey(k))
                   m_hotkeystate.Add(k, false);
            }
        }

        ///<summary>
        /// Gets the set of hot-keys that this class handles.
        ///</summary>
        public IEnumerable<Keys> HotKeys
        {
            get { return m_hotkeys; }
        }

        ///<summary>
        /// Returns whether the set of Keys is activated
        ///</summary>
        public bool HotKeysActivated
        {
            //The number of sets of remapped keys is used to offset the amount originally specified by the user.
            get { return m_hotkeydowncount ==  m_hotkeystate.Count ; }
        }

        ///<summary>
        /// Gets or sets the enabled state of the HotKey set.
        ///</summary>
        public bool Enabled
        {
            get { return m_enabled; }
            set
            {
                if ( value )
                    InitializeKeys(); //must get the actual current state of each key to update

                m_enabled = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kex"></param>
        internal void OnKey( KeyEventMoreArgs kex )
        {

            if ( !Enabled )
                return;

            //Gets the primary key if mapped to a single key or gets the key itself
            Keys k = kex.KeyCode;

            if ( kex.IsKeyDown)
                OnKeyDown( k );
            else 
                OnKeyUp( k );

        }

        private void OnKeyDown( Keys k )
        {
            //If the keys are activated still then keep invoking the event
            if ( HotKeysActivated )
                InvokeHotKeyHandler( OnHotKeysDownHold );          //Call the duration event

            //indicates the key's state is current false but the key is now down
            else if ( m_hotkeystate.ContainsKey( k ) && !m_hotkeystate[ k ] )
            {

                m_hotkeystate[ k ] = true;           //key's state is down
                ++m_hotkeydowncount;                 //increase the number of keys down in this set

                if ( HotKeysActivated )             //because of the increase, check whether the set is activated
                    InvokeHotKeyHandler( OnHotKeysDownOnce ); //Call the initial event

            }

        }

        private void OnKeyUp( Keys k )
        {

            if ( m_hotkeystate.ContainsKey( k ) && m_hotkeystate[ k ] ) //indicates the key's state was down but now it's up
            {

                bool wasActive = HotKeysActivated;

                m_hotkeystate[ k ] = false;          //key's state is up
                --m_hotkeydowncount;                 //this set is no longer ready

                if ( wasActive )
                    InvokeHotKeyHandler( OnHotKeysUp );            //call the KeyUp event because the set is no longer active

            }

        }

    }
}
