using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Net;

/*
 * TO DO
 * 
 * Exception
 * Don't use byteTo64
 * refact
 * 
 */

namespace CommonClasses
{
    //http://www.c-sharpcorner.com/uploadfile/0a7dc8/file-transfer-program-using-C-Sharp-net-windows-application/
    class TransferFile
    {
        struct InfoFile
        {
            public Int32 nameLen;
            public Int64 fileLen;
            public String nameFile;
        };
        public bool sendFile(TcpClient socket, String path)
        {
           /*  byte[] fileBytes ;
             String nameFile = Path.GetFileName(path);

             InfoFile infoFile = new InfoFile();

             infoFile.nameLen = nameFile.Length;
             infoFile.nameFile = nameFile;
            try{
                fileBytes = File.ReadAllBytes(path);


                infoFile.fileLen = fileBytes.Length;
                StreamWriter sw =new StreamWriter(socket.GetStream());
               
                sw.Write(infoFile.nameLen);
                sw.Write(infoFile.fileLen);
                sw.Write(infoFile.nameFile);
                sw.Write(Convert.ToBase64String(fileBytes));
                sw.Flush;
               // sw.Write(fileBytes, 0, fileBytes.Length);
            }catch(Exception e){



                return false;
            }*/
            return true;
        }
        public bool receiveFile(TcpClient socket, String pathFolder)
        {
           /* InfoFile infoFile = new InfoFile();
            int dimBuff=32;
            char[] buff=new char[dimBuff];
            try
            {
                StreamReader sr = new StreamReader(socket.GetStream());
                sr.Read(buff,0,sizeof(Int32));
                infoFile.nameLen = Convert.ToInt32(buff);
                sr.Read(buff, 0, sizeof(Int64));
                infoFile.fileLen = Convert.ToInt32(buff);
                if (infoFile.nameLen >= dimBuff)
                {
                    buff=new char[infoFile.nameLen+2];
                }
                Convert.ToString(sr.Read()
            }
            catch (Exception e)
            {
                return false;
            }*/


            return true;
        }
    }
}
