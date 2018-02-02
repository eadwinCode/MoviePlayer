using Common.Interfaces;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{

    public class SubtitleFilesModel : NotificationObject, ISubtitleFiles
    {
        public string FileName { get; set; }
        public string Directory { get; set; }
        private bool isselected;
        public bool IsSelected
        {
            get { return isselected; }
            set { isselected = value; RaisePropertyChanged(() => this.IsSelected); }
        }

        public SubtitleFilesModel(string dir)
        {
            this.Directory = dir;
            this.FileName = Path.GetFileNameWithoutExtension(dir);
        }

        public override string ToString()
        {
            return FileName;
        }

        public static void Add(IList<SubtitleFilesModel> collection, string filename)
        {
            SubtitleFilesModel file = new SubtitleFilesModel(filename);
            if (collection.FirstOrDefault(x => x.FileName == file.FileName) == null)
            {
                collection.Add(file);
            }
        }

        public static void Remove(List<SubtitleFilesModel> subPath, SubtitleFilesModel subtitleFiles)
        {
            if (subPath.Contains(subtitleFiles))
            {
                subPath.Remove(subtitleFiles);
            }
        }
    }
}
