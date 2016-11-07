using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasterMain
{
    public class KeyEventMoreArgs : KeyEventArgs
    {
        public KeyEventMoreArgs(Keys keyData)
            : base(keyData) { }

        internal KeyEventMoreArgs(Keys keyData, bool isKeyDown, bool isKeyUp)
            : this(keyData)
        {
            IsKeyDown = isKeyDown;
            IsKeyUp = isKeyUp;
        }


        /// <summary>
        /// True if event signals key down..
        /// </summary>
        public bool IsKeyDown { get; private set; }

        /// <summary>
        /// True if event signals key up.
        /// </summary>
        public bool IsKeyUp { get; private set; }

    }            

}
