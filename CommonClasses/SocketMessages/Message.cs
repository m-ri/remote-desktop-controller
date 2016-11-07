using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace CommonClasses.SocketMessages
{

    /// <summary>
    /// This class is deputed to send/receive array of bytes and object across two endpoints (throughts an established TCP connecitons).
    /// The format of payload is the following:
    /// 
    /// //PDU : TypeMessage[1 Bytes] + data_length[4 Bytes] +  data_value[data_length Bytes]
    /// </summary>
    public class Message 
    {
        
        public enum TypeMessage : byte { 
            Input,//used for inputs or commands
            ClipboardSocketRequest,//used by master in order to create a dedicated socket for clipboard
            ClipboardSocketOk,

            ClipboardRequest,
            ClipboardAttached,
            ClipboardSend,

            /*InputMouse,
            InputKeyboard,*/

            Error, 
            CorrectPassword,
            WrongPassword, 
            AuthenticateRequest,//request for autentication
            AuthentChallenge,//server send back a challenge
            AuthentResponse,//contains HASH(password and challenge)

            CloseConnection,

            PingEcho,
            PingReply

        };
        
       
        private const int MAXDATA = 1024*1024*1000; //1400;
        public const int SIZE_INIZ = 1400;
        private const int MAX_SIZE_SOCKET_CHUNCK = 1400;
        
        private Int32 _dataLen;
        private byte[] _dataVal;

        public Message() { _dataLen = 0; }

        public Message(TypeMessage type)
        {
            typeMessage = type;
            _dataLen = 0;
        }

        public Message(TypeMessage type, byte[] data,int dataLen=-1)
        {
            typeMessage = type;
            if (dataLen < 0 /*|| data_len > data.Length*/) dataLen = data.Length;
            dataLen = Math.Min( dataLen , MAXDATA);
            _dataVal = new byte[dataLen];
            if(dataLen>0)Array.Copy(data, _dataVal, dataLen );
            this._dataLen = dataLen;
        }
      

        public TypeMessage typeMessage { get; set; }

        public void set_bytes(byte[] bytes)
        {
            _dataLen = bytes.Length;
            if (_dataLen > MAXDATA) throw new Exception("Message writing: object too big");
            _dataVal = new byte[_dataLen];
            if (_dataLen > 0) Array.Copy(bytes, _dataVal, _dataLen);
        }

        //avoid to pass byte[] when possible
        public object data
        {
            set
            {
                if (data is string)
                {
                    _dataVal = Encoding.UTF8.GetBytes((string)data);
                    _dataLen = (_dataVal.Length < MAXDATA) ? _dataVal.Length : 0;
                }
                else if (data is byte[])//TODO maybe it doesn't work correctly
                {
                    /*byte[] bytes = (byte[])data;
                    data_len = bytes.Length;
                    data_val = new byte[data_len];
                    if (data_len > 0) Array.Copy(bytes, data_val, data_len);*/
                    set_bytes((byte[])data);
                }
                else
                {
                    _dataVal = SerializationMethods.ToByteArray(value, Marshal.SizeOf(value));
                    if (_dataVal.Length > MAXDATA) throw new Exception("Message serialization: object too big");
                    _dataLen = (_dataVal.Length < MAXDATA) ? _dataVal.Length : 0;
                }
                
                
            }
            get
            {
                /*if (data is string)
                    return Encoding.UTF8.GetString(data_val);
                else*/
                    return _dataVal;
            }
        }
        public String getString()
        {
            return Encoding.UTF8.GetString(_dataVal);
        }
        public void setString(string str)
        {
            _dataVal = Encoding.UTF8.GetBytes((string)str);
            _dataLen = (_dataVal.Length < MAXDATA) ? _dataVal.Length : 0;
        }

        public T Content<T>() { return SerializationMethods.FromByteArray<T>(_dataVal);  }
     

        public byte[] Serialize()
        {
            //byte[] buffer = new byte[MAXDATA+5];
            byte[] buffer = new byte[_dataLen + 5];
            buffer[0] = (byte)this.typeMessage;
            
            Array.ConstrainedCopy(BitConverter.GetBytes(_dataLen), 0, buffer, 1, 4);
            if(_dataLen>0)Array.ConstrainedCopy(_dataVal, 0, buffer, 5, _dataLen);
            return buffer;
        }

        public void WriteToSocket(NetworkStream stream)
        {
            byte[] buffer = this.Serialize();

            try
            {
                stream.Write(buffer, 0, buffer.Length);
            }
            catch (IOException e)
            {
                throw new Exception("Message : " + 
                    Enum.GetName(typeMessage.GetType(),typeMessage) + 
                    " can't be written to socket." + e.ToString());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Message ReadFromSocket(NetworkStream stream)
        {
            byte[] buffer = new byte[SIZE_INIZ];
            byte[] vLength = new byte[4];
            int len = -1;//trigger exception
            TypeMessage type;

            try {
                stream.Read(buffer, 0, 1);
                type = (TypeMessage)buffer[0];

                if (type == TypeMessage.ClipboardAttached || type == TypeMessage.ClipboardRequest)
                {
                    int a;
                    a = 0;
                }



                switch (type)
                {
                    /*case Status.Authenticate:
                        if (readNBytes(stream, 4, vLength, 0) != 0)
                            if (readNBytes(stream, MD5PasswordHashing.MD5ByteLength, buffer, 0) != MD5PasswordHashing.MD5ByteLength)
                                type = Status.Error;
                        len = MD5PasswordHashing.MD5ByteLength;
                        break;*/
                    case TypeMessage.AuthenticateRequest:
                    case TypeMessage.AuthentChallenge:
                    case TypeMessage.AuthentResponse:
                    case TypeMessage.ClipboardAttached:
                    case TypeMessage.ClipboardRequest:
                    case TypeMessage.Input:
                        if (readNBytes(stream, 4, vLength, 0) != 0)
                        {
                            len = BitConverter.ToInt32(vLength, 0);
                            if (len > SIZE_INIZ)
                            {
                                if (len <= MAXDATA) buffer = new byte[len];
                                else throw new Exception("Message: RX packet too big");
                            }
                            if (readNBytes(stream, len, buffer, 0) !=len)
                                type = TypeMessage.Error;
                        }
                        break;
                    case TypeMessage.CorrectPassword:
                    case TypeMessage.WrongPassword:
                    case TypeMessage.Error:
                    case TypeMessage.ClipboardSocketOk:
                    case TypeMessage.ClipboardSocketRequest:
                    case TypeMessage.CloseConnection:
                    case TypeMessage.PingEcho:
                    case TypeMessage.PingReply:
                        readNBytes(stream, 4, vLength, 0);
                        len = BitConverter.ToInt32(vLength, 0);
                        if (len != 0) throw new Exception("Message with unexpected size");
                        break;
                    default:
                        throw new Exception("Message not handled");
                }

                if (len < 0) throw new Exception("Message not handled:negative length");
                if (type == TypeMessage.Error || type == TypeMessage.CorrectPassword || type == TypeMessage.WrongPassword || type == TypeMessage.ClipboardSocketOk || type == TypeMessage.ClipboardSocketRequest) return new Message(type);
                //if (type == Status.Authenticate || type == Status.Input) return new Message(type, buffer, len/*, BitConverter.ToInt32(length, 0)*/);// : new Message(type);
               // if (type==Status.ClipboardAttached) return new Message(type,buffer,len);
                else return new Message(type, buffer, len);
                throw new Exception("case not handled");
            }
            catch (IOException e)
            {
                throw new MessageException("Can't read any message from socket. "+ e.Message);
            }
            
        }

        /// <summary>
        /// Give
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="numBytes">N bytes to be read</param>
        /// <param name="buffer">Destination buffer</param>
        /// <param name="initialOffset">Initial</param>
        /// <returns>Number of bytes effectively ff received from socket</returns>
        private static int readNBytes(NetworkStream stream, int numBytes, byte[] buffer,int initialOffset)
        {
            int countBytesLeft = numBytes;   
            int offset = initialOffset;
            
            while (countBytesLeft > 0)
            {
                int countBytesRead = 0;
                try
                {
                    countBytesRead = stream.Read(buffer, offset, Math.Min(countBytesLeft, MAX_SIZE_SOCKET_CHUNCK));
                }
                catch (Exception e)
                {
                    throw e;//TODO remove
                }
                if (countBytesRead < 0){
                    return -1;
                }
                    
                else if (countBytesRead == 0)
                    break;

                countBytesLeft -= countBytesRead;
                offset += countBytesRead;
            }
            return numBytes - countBytesLeft; 
        }

    }
    
}
