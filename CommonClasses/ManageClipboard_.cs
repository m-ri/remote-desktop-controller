using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using CommonClasses.SocketMessages;

namespace CommonClasses
{
    public static class ManageClipboard_
    {
        private static String tempFolderRoot = System.IO.Path.GetTempPath()+"remoteControl/";//TODO improve according http://stackoverflow.com/questions/944483/how-to-get-temporary-folder-for-current-user


        public static SocketMessages.Message getCurrentClipboardMessage()
        {
            
            SocketMessages.Message m = new SocketMessages.Message(SocketMessages.Message.TypeMessage.ClipboardAttached);

            try
            {
                IDictionary<String, Byte[]> mappa = getCurrentClipboard();
                m.data = mappa;
            }
            catch(Exception e)
            {
                throw new Exception("Error during clipboard send");
            }
            return m;
        }

        public static IDictionary<String, Byte[]> getCurrentClipboard()
        {

            IDictionary<String, Byte[]> clipboardMap = new Dictionary<String, Byte[]>(); ;
         
            IDataObject iData = Clipboard.GetDataObject();
            String[] formats = iData.GetFormats();
            clipboardMap.Clear();
           // Object o;
            foreach (string format in formats)
            {
                if (format == DataFormats.FileDrop){

                    IDictionary<String, Byte[]> filesMap = new Dictionary<String, Byte[]>();
                    String[] files =(String[]) iData.GetData(format);
                    foreach (string file in files)
                    {
                        Dictionary<String,Byte[]> sub= getDirectoryTree(file,Path.GetFileName(file));
                        filesMap.Add(Path.GetFileName(file), SerializationMethods.ToByteArray(sub, Marshal.SizeOf(sub)));
                    }
                    clipboardMap.Add(DataFormats.FileDrop + "MyStruct", SerializationMethods.ToByteArray(filesMap, Marshal.SizeOf(filesMap)));
                }
                //else{
                try
                {
                    if (Marshal.IsComObject(iData.GetData(format)))
                    {
                        int a;
                    }
                }
                catch (Exception e)
                {

                }

              /*!!!rem      clipboardMap.Add(format, iData.GetData(format));
                    if (format == "FileName" || format == "FileNameW"|| format == DataFormats.FileDrop)
                    {
                        String[] files = (string[])clipboardMap[format];
                        for (int i = 0; i < files.Length; i++)
                            files[i] = Path.GetFileName(files[i]);
                    }
                *///}
                
            }

            return clipboardMap;
        }
        public static void setCurrentClipboard(SocketMessages.Message m)
        {
            if (m.typeMessage != SocketMessages.Message.TypeMessage.ClipboardAttached) throw new Exception("Error during clipboard receiving");
            try
            {
                IDictionary<String, Object> mappa = m.Content<IDictionary<String, Object>>();
                setCurrentClipboard(mappa);

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public static void setCurrentClipboard(IDictionary<String, Object> clipboardMap)
        {
            long numFolder;
            String tempFolder;
            do
            {
                numFolder = (new Random()).Next();
                tempFolder= tempFolderRoot + numFolder + "/";
            } while (Directory.Exists(tempFolder));

            

            Clipboard.Clear();
            DataObject dO = new DataObject();

            foreach (string key in clipboardMap.Keys)
            {
                if (key == "FileName" || key == "FileNameW" || key == DataFormats.FileDrop)
                {
                    String[] files = (string[])clipboardMap[key];
                    for (int i = 0; i < files.Length; i++)
                        files[i] = tempFolder + files[i];

                   
                
                }

                if (key == (DataFormats.FileDrop + "MyStruct"))
                {
                    setDirectoryTree(clipboardMap[key], tempFolder);
                }
                else if (clipboardMap[key] is System.IO.MemoryStream)
                {

                }
                
                else
                {

                    dO.SetData(key, clipboardMap[key]);
                    /*if (key == DataFormats.FileDrop)
                    {
                        dO.SetData("Preferred "+key, clipboardMap[key]);
                    }*/
                }
            }
            Clipboard.SetDataObject(dO, true);
        }

        public static Dictionary<String, Byte[]> getDirectoryTree(String pathFisico, String pathRelativo)
        {
            FileAttributes attr = File.GetAttributes(@pathFisico);
            if((attr & FileAttributes.Directory) == FileAttributes.Directory){//maybe  fastest way http://stackoverflow.com/questions/1395205/better-way-to-check-if-path-is-a-file-or-a-directory
                Dictionary<String, Byte[]> mappaFiles = new Dictionary<String, Byte[]>();
                
                String[] files = Directory.GetFiles(pathFisico, "*", SearchOption.TopDirectoryOnly);
                foreach(string file in files){
                    String pathRelFile = /*pathRelativo + "/" + */Path.GetFileName(file);
                    //mappaFiles.Add(pathRelFile,getDirectoryTree(file, pathRelFile));
                     
                    mappaFiles.Add(pathRelFile,getFile(file) );
                }
                String[] folders = Directory.GetDirectories(pathFisico, "*", SearchOption.TopDirectoryOnly);
                foreach (string folder in folders)
                {
                    String pathRelFold = /*pathRelativo + "/" + */Path.GetFileName(folder);
                     Dictionary<String, Byte[]> subDir=getDirectoryTree(folder, pathRelFold);
                    mappaFiles.Add(pathRelFold,SerializationMethods.ToByteArray(subDir,Marshal.SizeOf(subDir)) );
                }
                return mappaFiles;
            }else{

                throw new Exception("not supported");
                //return File.ReadAllBytes(pathFisico);//http://msdn.microsoft.com/en-us/library/ms997518.aspx
            }
        }

       public static Byte[] getFile(String pathFisico)
       {
           return File.ReadAllBytes(pathFisico);
       }
        public static void setDirectoryTree(Object obj,String path)
        {
            
            if (obj is IDictionary<String,Object>)
            {
                Dictionary<String, Object> mappaFile = (Dictionary<String, Object>)obj;
                Directory.CreateDirectory(path);

                foreach (string file in mappaFile.Keys)
                {
                    setDirectoryTree(mappaFile[file], path + "/" + file);
                }
               
            }
            else if(obj is System.Byte[])
            {

                 File.WriteAllBytes(path,(byte[])obj);
            }
        }


    }
}
