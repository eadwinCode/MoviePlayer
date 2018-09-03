﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using Delimon.Win32.IO;
using System.Threading.Tasks;
using Movies.MoviesInterfaces;
using Movies.Enums;
using Movies.Models.Model;

namespace Movies.MovieServices.Services
{
    public partial class FileExplorerCommonHelper : IFileExplorerCommonHelper
    {
        public T GetElement<T>(DependencyObject element)
        {
            while (element != null && !(element is T))
                element = VisualTreeHelper.GetParent(element);
            return (T)Convert.ChangeType(element, typeof(T));
        }

        private  object padlock = new object();
        private IApplicationService applicationService;

        public FileExplorerCommonHelper(IApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        public  List<FileInfo> GetFilesByExtensions(DirectoryInfo dir, IDictionary<string,string> extensions)
        {
            string[] notsupported = { "etrg", "sample", "rarbg.com" };
            List<FileInfo> Listfiles = new List<FileInfo>();
            IEnumerable<FileInfo> files = dir.GetFiles();// "*.*" SearchOption.TopDirectoryOnly);
            Parallel.ForEach(files, (fileinfo) => SelectWantedFiles( Listfiles, fileinfo,extensions));
            return Listfiles;
        }

        private  void SelectWantedFiles( List<FileInfo> listfiles, FileInfo fileinfo, IDictionary<string, string> extensions)
        {
            if (extensions.ContainsKey(fileinfo.Extension.ToLower()))
            {
                lock (padlock) {
                    listfiles.Add(fileinfo);
                }
            }
                
        }

        public  List<FileInfo> RemoveDirectory(List<FileInfo> files, FileInfo fileInfo)
        {
            List<FileInfo> Listfiles = new List<FileInfo>();
            Listfiles.AddRange(files.Where(f => !f.Directory.FullName.Contains(fileInfo.Directory.FullName)));
            return Listfiles;
        }

        public  string FileSizeConverter(double filelength)
        {
            int GB;
            double MB;
            double KB;
            GB = (int)(filelength / Math.Pow(10, 9));
            MB = (int)(filelength % Math.Pow(10, 9)) / 1000000;
            #region ControlSystem

            if (GB != 0)
            {
                MB = Math.Round(((filelength % Math.Pow(10, 9)) / Math.Pow(10, 9)), 2);
                return (Math.Round((GB + MB) * 0.9530378, 2).ToString() + "GB");
            }
            else if (MB != 0)
            {
                KB = Math.Round(((filelength % Math.Pow(10, 6)) / Math.Pow(10, 6)), 2);
                return (Math.Round((MB + KB) * 0.9530378, 2).ToString() + "MB");
            }
            else
            {
                KB = Math.Round(((filelength % Math.Pow(10, 6)) / Math.Pow(10, 3)), 2);
                return (Math.Round(KB * 0.9530378, 2).ToString() + "KB");
            }
            #endregion
        }

        public List<DirectoryInfo> GetParentSubDirectory(DirectoryInfo DirectoryPosition, IDictionary<string, string> formats)
        {
            List<DirectoryInfo> ListDir = new List<DirectoryInfo>();
            object padlock = new object();
            IEnumerable<DirectoryInfo> files = DirectoryPosition.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
            var parallelloop = Parallel.ForEach(files, (o) => SelectWantedDirectory(o,  ListDir, formats, padlock));
            return ListDir;
        }

        private void SelectWantedDirectory(DirectoryInfo o,  List<DirectoryInfo> listDir, 
            IDictionary<string, string> formats,object lockd)
        {
            if (CheckForWantedDir(o, formats))
            {
                lock (lockd)
                {
                    listDir.Add(o);
                }
            }
        }

        public List<DirectoryInfo> GetParentSubDirectoryWithoutCheck(DirectoryInfo DirectoryPosition,
            IDictionary<string, string> formats)
        {
            List<DirectoryInfo> ListDir = new List<DirectoryInfo>();
            IEnumerable<DirectoryInfo> files = DirectoryPosition.GetDirectories("*.*", SearchOption.TopDirectoryOnly);

            var checkforwanteddir = files.Where(f => CheckForWantedDir(f, formats));
            ListDir.AddRange(checkforwanteddir);
            return ListDir;
        }

        public bool CheckForWantedDir(DirectoryInfo dir, IDictionary<string, string> formats)
        {
            string[] notsupported = { "etrg", "sample", "rarbg.com" };
            List<FileInfo> Listfiles = new List<FileInfo>();
            try
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(dir.FullName);
                
                var subdir = directoryInfo.EnumerateFiles("*.*", System.IO.SearchOption.AllDirectories);
                var finalfilesort = subdir.Where(x => formats.ContainsKey(x.Extension.ToLower())).FirstOrDefault();

                if (finalfilesort != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, ex.TargetSite.Name);
                return false;
            }
            return false;
        }

        public ObservableCollection<string> MatchSubToMedia(string p, IEnumerable<FileInfo> subpath)
        {
            if (subpath == null) return null;
            var posiblepath = subpath.Where(x => Match(Path.GetFileNameWithoutExtension(x.Name)
                         ,(Path.GetFileNameWithoutExtension(p)))).ToArray();
            if (posiblepath.Length != 0)
            {
                ObservableCollection<string> subfilepath = new ObservableCollection<string>();
                for (int i = 0; i < posiblepath.Length; i++)
                {
                    subfilepath.Add((posiblepath[i].FullName));
                   //subfilepath[i] = posiblepath[i].FullName;
                }
                return subfilepath;
            }

            return new ObservableCollection<string>();
        }

        public IEnumerable<FileInfo> GetSubtitleFiles(DirectoryInfo dir)
        {
            List<FileInfo> subfiles = new List<FileInfo>();
            IEnumerable<DirectoryInfo> parentchildir = dir.GetDirectories("*.*", SearchOption.TopDirectoryOnly);

            IEnumerable<FileInfo> subfile = dir.GetFiles()// "*.*", SearchOption.TopDirectoryOnly)
                .Where(s => applicationService.SubtitleFormats.Contains(s.Extension));

                if (subfile != null)
                    subfiles.AddRange(subfile);

            if (parentchildir != null)
            {
                foreach (var item in parentchildir)
                {
                    try
                    {
                        var files = item.GetFiles();
                        subfile = files.Where(s => applicationService.SubtitleFormats.Contains(s.Extension));
                        if (subfile != null)
                            subfiles.AddRange(subfile);
                    }
                    catch (Exception uaex)
                    {
                    }
                }
            }
            
            return subfiles;
        }

        public bool Match(string srtfile, string file)
        {
            if (file.Contains(srtfile))
            {
                return true;
            }
            int check = 0;
            string[] delimeter = { ".","_"," ", "-", " ", "_", "[", "]", "(", ")", ",","+" };
            string[] srttin = srtfile.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);
            string[] filetin = file.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);
            int perfitting = 0;

            if (srtfile.ToLower() == file.ToLower())
            {
                return true;
            }

            if (filetin.Length <= 2)
            {
                if (srttin[0].ToLower() == filetin[0].ToLower())
                {
                    perfitting = 100;
                }
                else
                {
                    check = 1;
                    //srttin = null;
                    //filetin = null;
                }
            }
            else
            {
                check = 1;
            }

            if (check == 1)
            {
                perfitting = 0;
                //string[] delimeter1 = { "-"," ", "_", "[", "]", "(", ")", ",", "x" };
                //srttin = srtfile.Split(delimeter1, StringSplitOptions.RemoveEmptyEntries);
                //filetin = file.Split(delimeter1, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < filetin.Length; i++)
                {
                    for (int j = 0; j < srttin.Length; j++)
                    {
                        if (Regex.Matches( filetin[i].ToLower(),(srttin[j].ToLower())).Count > 0)
                        {
                            ++perfitting;
                            break;
                        }
                    }
                    if (i == srttin.Length)
                    {
                        break;
                    }
                }
            }

            if (perfitting != 0 && check == 1)
            {
                perfitting = (int)(perfitting * 100 / filetin.Length);
            }
            if (perfitting >= 75)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    
}
