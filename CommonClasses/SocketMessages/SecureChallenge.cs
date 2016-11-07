using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CommonClasses.SocketMessages
{
    public class SecureChallenge
    {
        private static bool _isInitialized = false;
        private static RandomNumberGenerator rng;
        public static String getString64(int lenBytes)
        {
            if (!_isInitialized) { 
                rng = new RNGCryptoServiceProvider();
                _isInitialized = true;
            }
            byte[] tokenData = new byte[lenBytes];
            rng.GetBytes(tokenData);

            return Convert.ToBase64String(tokenData);
        }

        public static String fusePasswordChallenge(string password, string challenge)
        {
            return password + challenge;
        }
    }
}
