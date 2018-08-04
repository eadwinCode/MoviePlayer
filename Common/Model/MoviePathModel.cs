using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Model
{
    public class MovieFolderModel
    {
        public string path;
        private DirectoryInfo directoryinfo;
        public DirectoryInfo DirectoryInfo
        {
            get { if (directoryinfo == null) {
                    directoryinfo = new DirectoryInfo(path);
                }
                return directoryinfo;
            }
        }
        public MovieFolderModel(string fileName) 
        {
            path = fileName;
            //directoryinfo = new DirectoryInfo(fileName);
        }

        public string FullName
        {
            get
            {
                return DirectoryInfo.FullName;
            }
        }

        public string Name
        {
            get { return DirectoryInfo.Name; }
        }

        public string Extension
        {
            get { return DirectoryInfo.Extension; }
        }

        public bool Exists
        {
            get
            {
                return DirectoryInfo.Exists;
            }
        }

        public void Delete()
        {
            DirectoryInfo.Delete();
        }
        public override bool Equals(object obj)
        {
            if (obj != null && (obj is MovieFolderModel))
            {
                return ((MovieFolderModel)obj).FullName == this.FullName;
            }

            return false;
        }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
