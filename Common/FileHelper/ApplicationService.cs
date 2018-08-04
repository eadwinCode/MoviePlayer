using Common.FileHelper;
using Common.Interfaces;
using Common.Model;
using Common.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Windows;

namespace Common.FileHelper
{
    public static class ApplicationService
    {
        public const string Settings = "Settings";
        private const string SettingsFileName = "Settings.json";
        private const string PlaylistFileName = "Playlist.pl";
        private const string PlaylistFolderName = "PlaylistFolder";
        private const string LastSeenFolderName = "LastSeen";
        private const string LastSeenFileName = "LastPlayed.sxc";

        private static string AppName = Path.GetFileNameWithoutExtension(AppDomain.
            CurrentDomain.FriendlyName);
        private static string SystemDocumentPath = Environment.GetFolderPath(Environment.
            SpecialFolder.MyDocuments);
        
        private static string MediaFolderPath;
        public static Settings AppSettings = new Settings();
        public static SavedPlaylistCollection AppPlaylist = new SavedPlaylistCollection();
        public static SavedLastSeenCollection SavedLastSeenCollection = new SavedLastSeenCollection();
        static string[] delimeter = { " " };

        public static IDictionary<string,string> formats = new Dictionary<string, string>()
        {
           { ".wmv" ,"wmv"}, {".3gp","3gp" } ,{ ".avi","avi" }, {".divx","divx" }, {".dv","dv" }, {".flv","flv" },{".m4v" ,"m4v"},
            { ".mkv","mkv" }, {".mov","mov" },{ ".mp4","mp4" }, {".mpeg","mpeg" },
            { ".mpeg1","mpeg1" }, {".mpeg2", "mpeg2"},{".mpeg4","mpeg4" },{".mpg","mpg" },{".ogg","ogg" },{".rec","rec" },{".rm","rm" }, {".rmvb","rmvb" },
            { ".vob","vob" },{".webm" ,"webm"}
              };

        public static string[] subtitleformats =
        {
          ".aqt ",".vtt",".cvd",".dks" ,".jss",".sub" ,".ttxt" ,".mpl" ,".txt" ,".pjs" ,".psb"
          ,".rt",".smi" ,".ssf" ,".srt" ,".ssa" ,".svcd",".usf" ,".idx"
        };

        static ApplicationService() {
            if (AppName.Contains(".vshost"))
            {
               AppName= AppName.Replace(".vshost", "");
            }
            MediaFolderPath = SystemDocumentPath + @"\" + AppName;
        }

        public static void LoadFiles()
        {
            LoadTreeViewFile();
            LoadPlaylistFile();
            LoadLastSeenFile();
        }

        public static void LoadLastSeenFile()
        {
            string path = FileExistOrCreate(@"\" + LastSeenFolderName + @"\" + LastSeenFileName);
            if (path != null)
            {
                string jsonfile = File.ReadAllText(path);
                object lastseen = JsonConvert.DeserializeObject<SavedLastSeenCollection>(jsonfile);
                if (lastseen != null)
                {
                    SavedLastSeenCollection = (SavedLastSeenCollection)lastseen;
                }
            }
        }

        public static bool SaveLastSeenFile()
        {
            try
            {
                string path = FileExistOrCreate(@"\" + LastSeenFolderName + @"\" + LastSeenFileName, true);
                string json = JsonConvert.SerializeObject(SavedLastSeenCollection, Formatting.Indented);
                File.WriteAllText(path, json);
                return true;
            }
            catch (Exception)
            {
                return false;
                //throw;
            }
        }

        public static void CreateFolder()
        {
            try
            {
                if (!Directory.Exists(MediaFolderPath))
                {
                    Directory.CreateDirectory(MediaFolderPath);
                    DirectoryInfo di = new DirectoryInfo(MediaFolderPath);
                    di.Attributes |= FileAttributes.Hidden;
                    CreateSubFolder(MediaFolderPath);
                }
                else
                {
                    CreateSubFolder(MediaFolderPath);
                }
            }
            catch (IOException)
            {
               // throw;
            }
        }
        
        static void CreateSubFolder(string path)
        {
            string path1 = path + @"\" + Settings;
            if (!Directory.Exists(path1))
            {
                Directory.CreateDirectory(path1);
                DirectoryInfo di = new DirectoryInfo(path1);
                di.Attributes |= FileAttributes.Hidden;
            }
            path1 = string.Empty;
            path1 = path + @"\" + PlaylistFolderName;
            if (!Directory.Exists(path1))
            {
                Directory.CreateDirectory(path1);
                DirectoryInfo di = new DirectoryInfo(path1);
                di.Attributes |= FileAttributes.Hidden;
            }
            path1 = string.Empty;
            path1 = path + @"\" + LastSeenFolderName;
            if (!Directory.Exists(path1))
            {
                Directory.CreateDirectory(path1);
                DirectoryInfo di = new DirectoryInfo(path1);
                di.Attributes |= FileAttributes.Hidden;
            }
        }

        public static bool LoadTreeViewFile()
        {
            string path = FileExistOrCreate(@"\" + Settings + @"\" + SettingsFileName);
            if (path != null)
            {
                string jsonfile = File.ReadAllText(path);
                object settings = JsonConvert.DeserializeObject<Settings>(jsonfile);
                if (settings != null)
                {
                    AppSettings = (Settings)settings;
                }
                return true;
            }
            return false;
        }

        public static bool LoadPlaylistFile()
        {
            string path = FileExistOrCreate(@"\" + PlaylistFolderName + @"\" + PlaylistFileName);
            if (path != null)
            {
                string jsonfile = File.ReadAllText(path);
                object settings = JsonConvert.DeserializeObject<SavedPlaylistCollection>(jsonfile);
                if (settings != null)
                {
                    AppPlaylist = (SavedPlaylistCollection)settings;
                }
                return true;
            }
            return false;
        }

        public static void SaveFiles()
        {
            SaveTreeViewFiles();
            SavePlaylistFiles();
            SaveLastSeenFile();
        }

        public static bool SaveTreeViewFiles()
        {
            try
            {
                string path = FileExistOrCreate(@"\" + Settings + @"\" + SettingsFileName, true);
                string json = JsonConvert.SerializeObject(AppSettings, Formatting.Indented);
                File.WriteAllText(path, json);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool SavePlaylistFiles()
        {
            if (AppPlaylist.MoviePlayList.Count > 0)
            {
                try
                {
                    string path = FileExistOrCreate(@"\" + PlaylistFolderName + @"\" + PlaylistFileName, true);
                    string json = JsonConvert.SerializeObject(AppPlaylist, Formatting.Indented);
                    File.WriteAllText(path, json);
                    return true;
                }
                catch (Exception)
                {
                    throw;
                }

            }
            return false;
        }

        public static string FileExistOrCreate(string path_to_file, bool returnpath = false)
        {
            string path;
            path = MediaFolderPath + path_to_file;
            if (File.Exists(path))
            {
                return path;
            }
            else
            {
                File.Create(path);
                if (returnpath)
                {
                    return path;
                }
                return null;
            }

        }
        
    }

    

   
}
