using Movies.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Movies.Models.Model
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class MovieFolderModel : IMovieFolderModel
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
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
