using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Media;
using VideoComponent.BaseClass;
using System.Text.RegularExpressions;
using Common.Model;

namespace VirtualizingListView.Util
{
    public static class FileExplorerCommonHelper
    {
        public static T GetElement<T>(DependencyObject element)
        {
            while (element != null && !(element is T))
                element = VisualTreeHelper.GetParent(element);

            return (T)Convert.ChangeType(element, typeof(T));
        }

        public static List<FileInfo> GetFilesByExtensions(DirectoryInfo dir, params string[] extensions)
        {
            string[] notsupported = { "etrg", "sample", "rarbg.com" };
            List<FileInfo> Listfiles = new List<FileInfo>();
            IEnumerable<FileInfo> files = dir.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            Listfiles.AddRange(files.Where(f => extensions.Contains(f.Extension.ToLower())));
            //var finalfilesort = Listfiles.Where(f => !notsupported.Contains(Path.GetFileNameWithoutExtension(f.FullName.ToString().ToLower()))).ToList<FileInfo>();
            return Listfiles;
        }

        public static List<FileInfo> RemoveDirectory(List<FileInfo> files, FileInfo fileInfo)
        {
            List<FileInfo> Listfiles = new List<FileInfo>();
            Listfiles.AddRange(files.Where(f => !f.Directory.FullName.Contains(fileInfo.Directory.FullName)));
            return Listfiles;
        }
        public static string FileSizeConverter(double filelength)
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
       
        internal static List<DirectoryInfo> GetParentSubDirectory(DirectoryInfo DirectoryPosition, string[] formats)
        {
            try
            {
                List<DirectoryInfo> ListDir = new List<DirectoryInfo>();
                IEnumerable<DirectoryInfo> files = DirectoryPosition.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
                
               // var checkforwanteddir = files.Where(f => CheckForWantedDir(f, formats)).ToList();
                ListDir.AddRange(files);
                return ListDir;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,ex.TargetSite.Name);
            }
            return null;
        }

        public static bool CheckForWantedDir(DirectoryInfo dir, params string[] extensions)
        {
            string[] notsupported = { "etrg", "sample", "rarbg.com" };
            List<FileInfo> Listfiles = new List<FileInfo>();
            try
            {
                IEnumerable<FileInfo> subdir = dir.GetFiles("*.*", SearchOption.TopDirectoryOnly); 
                
                if (subdir.Count() == 0)
                {
                    subdir = dir.GetFiles("*.*", SearchOption.AllDirectories); 
                }

                var finalfilesort = subdir.Where(x => extensions.Contains(x.Extension.ToLower())).FirstOrDefault();
                //if (subdir.Count<DirectoryInfo>() != 0)
                //{
                //    foreach (var item in subdir)
                //    {
                //        IEnumerable<FileInfo> files = item.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                //        Listfiles.AddRange(files.Where(f => extensions.Contains(f.Extension.ToLower())));
                //        files = null;
                //    }
                //}
                //else
                //{
                //    IEnumerable<FileInfo> files = dir.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                //    Listfiles.AddRange(files.Where(f => extensions.Contains(f.Extension.ToLower())));
                //}


                //var finalfilesort = Listfiles.Where(f => !notsupported.Contains(Path.GetFileNameWithoutExtension(f.FullName.ToString().ToLower()))).ToList<FileInfo>();
                if (finalfilesort != null)
                {
                    return true;
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.TargetSite.Name);
                return false;
            }
           
            return false;
        }

        internal static List<SubtitleFilesModel> MatchSubToMedia(string p, IEnumerable<FileInfo> subpath)
        {
            if (subpath == null) return null;
            var posiblepath = subpath.Where(x => Match(Path.GetFileNameWithoutExtension(x.Name)
                         ,(Path.GetFileNameWithoutExtension(p)))).ToArray();
            if (posiblepath.Length != 0)
            {
                List<SubtitleFilesModel> subfilepath = new List<SubtitleFilesModel>();
                for (int i = 0; i < posiblepath.Length; i++)
                {
                    subfilepath.Add(new SubtitleFilesModel(posiblepath[i].FullName));
                   //subfilepath[i] = posiblepath[i].FullName;
                }
                return subfilepath;
            }

            return new List<SubtitleFilesModel>();
        }

        internal static IEnumerable<FileInfo> GetSubtitleFiles(DirectoryInfo dir)
        {
            List<FileInfo> subfiles = new List<FileInfo>();
           
                IEnumerable<DirectoryInfo> parentchildir = dir.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
                IEnumerable<FileInfo> subfile = dir.GetFiles("*.srt*", SearchOption.TopDirectoryOnly);

                if (subfile != null)
                    subfiles.AddRange(subfile);

            if (parentchildir != null)
            {
                foreach (var item in parentchildir)
                {
                    try
                    {
                        subfile = item.GetFiles("*.srt*", SearchOption.TopDirectoryOnly);
                        if (subfile != null)
                            subfiles.AddRange(subfile);
                    }
                    catch (UnauthorizedAccessException) { }
                }
            }
            
            return subfiles;
        }

        public static bool Match(string srtfile, string file)
        {
            if (file.Contains(srtfile))
            {
                return true;
            }
            int check = 0;
            string[] delimeter = { ".","_"," ", "-", " ", "_", "[", "]", "(", ")", "," };
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
