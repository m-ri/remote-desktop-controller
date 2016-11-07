using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CommonClasses.SocketMessages
{
    public static class MD5PasswordHashing
    {
        public const int MD5ByteLength = 16;
        private static MD5 md5Hash = MD5.Create();

        public static string GetMd5Hash(string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            if (data.Length != MD5ByteLength)
                throw new Exception("Length of MD5 byte stream doesn't match with the standard byte length");

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        public static byte[] GetMd5HashBytes(string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            if (data.Length != MD5ByteLength)
                throw new Exception("Length of MD5 byte stream doesn't match with the standard byte length");

            return data;
        }

        // Verify hashing of values
        //TODO eliminate it
        public static bool VerifyMd5Hash(string local, string other)
        {
            throw new MessageException("remove this code");
            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(local, other))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //is accepted a vector hash with more bytes than length
        public static bool VerifyMd5Hash(string password, byte[] hash)
        {
            byte[] localHash = GetMd5HashBytes(password);
            if (localHash.Length > hash.Length) return false;
            bool hash_ok = true;
            for (int i = 0; i < localHash.Length; i++)
                if (localHash[i] != hash[i]) hash_ok = false;

            return hash_ok;
        }
    }
}
