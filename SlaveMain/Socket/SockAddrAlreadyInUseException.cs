using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlaveMain.Socket
{
    class SockAddrAlreadyInUseException : Exception
    {
        public SockAddrAlreadyInUseException() { }

        public SockAddrAlreadyInUseException(string message) : base(message) { }

        public SockAddrAlreadyInUseException(string message, Exception inner) : base(message, inner) { }
    }
}
