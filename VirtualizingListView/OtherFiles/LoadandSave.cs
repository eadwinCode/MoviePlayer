using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Reflection;
using System.IO;
using System.Windows;

namespace VirtualizingListView
{
    public class LoadandSave:Control,ILoadFile
    {
        private string MediaFolderPath;
        string AppName = Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);
        private string SystemDocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public LoadandSave()
        {
            MediaFolderPath = SystemDocumentPath + @"\" + AppName;
            CreateMediaFolders();
            DeserializeData();
        }

        public string MediaPath
        {
            get { return MediaFolderPath; }
        }

        public void CreateMediaFolders()
        {
            try
            {
                if (!Directory.Exists(MediaPath))
                {
                    Directory.CreateDirectory(MediaPath);
                }
            }
            catch (IOException)
            {
                throw;
            }
        }

        public bool SerializeData()
        {
            return false;
           // throw new NotImplementedException();
        }

        public bool DeserializeData()
        {
            return true;
           // throw new NotImplementedException();
        }
    }
}
