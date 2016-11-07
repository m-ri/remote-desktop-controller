using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CommonClasses.SocketMessages;

namespace CommonClasses
{
    public static class ManageClipboard
    {
        private static readonly String TempFolderRoot = System.IO.Path.GetTempPath()+"remoteControl/";//TODO improve according http://stackoverflow.com/questions/944483/how-to-get-temporary-folder-for-current-user


        public static SocketMessages.Message getCurrentClipboardMessage()
        {
            
            SocketMessages.Message m = new SocketMessages.Message(SocketMessages.Message.TypeMessage.ClipboardAttached);

            
            try
            {
                IDictionary<String, Object> mappa;
                mappa = getCurrentClipboard();
                byte[] bytes=CommonClasses.SocketMessages.SerializationMethods.SerializeToByteArray(  mappa);
                m.set_bytes( bytes);
            }
            catch(Exception e)
            {
                throw new Exception("Error during clipboard send");
            }
            return m;
        }

        public static IDictionary<String, Object> getCurrentClipboard()
        {
            
            IDictionary<String, Object> clipboardMap = new Dictionary<string, object>(); 
         
            IDataObject iData = Clipboard.GetDataObject();
            if (iData == null)
            {
                return clipboardMap;
            }
            String[] formats = iData.GetFormats();
            clipboardMap.Clear();
           // Object o;
            foreach (string format in formats)
            {
                if (format == DataFormats.FileDrop){
                    
                    IDictionary<String, Object> filesMap = new Dictionary<string, object>();
                    String[] files =(String[]) iData.GetData(format);
                    foreach (string file in files)
                    {
                        filesMap.Add(Path.GetFileName(file), getDirectoryTree(file,Path.GetFileName(file)));
                    }
                    clipboardMap.Add(DataFormats.FileDrop + "MyStruct", filesMap);
                }
                //else{
                    clipboardMap.Add(format, iData.GetData(format));
                    if (format == "FileName" || format == "FileNameW"|| format == DataFormats.FileDrop)
                    {
                        String[] files = (string[])clipboardMap[format];
                        for (int i = 0; i < files.Length; i++)
                            files[i] = Path.GetFileName(files[i]);
                    }
                //}
                
            }

            return clipboardMap;
        }
        
        public static Dictionary<String,Byte[]> getCurrentClipboardBase()
        {
            Dictionary<String, Byte[]> clipboardMap = new Dictionary<string, byte[]>();
            if(Clipboard.ContainsText()){
                clipboardMap.Add("Text", System.Text.Encoding.Unicode.GetBytes(Clipboard.GetText()));//is the best approache?
            }
            if(Clipboard.ContainsFileDropList()){
                var a=Clipboard.GetFileDropList();
                //TODO finire
                //clipboardMap.Add("FileDropList", Clipboard.get)
            }
            if (Clipboard.ContainsAudio())
            {
                clipboardMap.Add("Audio", SerializationMethods.SerializeToByteArray(Clipboard.GetAudioStream()));
            }
            //if(Clipboard.co)

            return clipboardMap;
            
        }
         
        public static void setCurrentClipboard(SocketMessages.Message m)
        {
            if (m.typeMessage != SocketMessages.Message.TypeMessage.ClipboardAttached) throw new Exception("Error during clipboard receiving");
            try
            {
                IDictionary<String, Object> mappa =(IDictionary<String, Object>) CommonClasses.SocketMessages.SerializationMethods.DeserializeFromByteArray((byte[])m.data);
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
                tempFolder= TempFolderRoot + numFolder + "/";
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

        public static Object getDirectoryTree(String pathFisico,String pathRelativo){
            FileAttributes attr = File.GetAttributes(@pathFisico);
            if((attr & FileAttributes.Directory) == FileAttributes.Directory){//maybe  fastest way http://stackoverflow.com/questions/1395205/better-way-to-check-if-path-is-a-file-or-a-directory
                Dictionary<String,Object> mappaFiles=new Dictionary<string,object>();
                
                String[] files = Directory.GetFiles(pathFisico, "*", SearchOption.TopDirectoryOnly);
                foreach(string file in files){
                    String pathRelFile = /*pathRelativo + "/" + */Path.GetFileName(file);
                    mappaFiles.Add(pathRelFile,getDirectoryTree(file, pathRelFile));
                }
                String[] folders = Directory.GetDirectories(pathFisico, "*", SearchOption.TopDirectoryOnly);
                foreach (string folder in folders)
                {
                    String pathRelFold = /*pathRelativo + "/" + */Path.GetFileName(folder);
                    mappaFiles.Add(pathRelFold, getDirectoryTree(folder, pathRelFold));
                }
                return mappaFiles;
            }else{
                return File.ReadAllBytes(pathFisico);//http://msdn.microsoft.com/en-us/library/ms997518.aspx
            }
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
