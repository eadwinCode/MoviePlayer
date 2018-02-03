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

namespace Common.FileHelper
{
    public static class CreateHelper
    {
        public const string Settings = "Settings";
        private const string SettingsFileName = "Settings.json";
        private const string PlaylistFileName = "Playlist.pl";
        private const string PlaylistFolderName = "PlaylistFolder";

        private static string AppName = Path.GetFileNameWithoutExtension(AppDomain.
            CurrentDomain.FriendlyName);
        private static string SystemDocumentPath = Environment.GetFolderPath(Environment.
            SpecialFolder.MyDocuments);
        
        private static string MediaFolderPath;
        public static Settings AppSettings = new Settings();
        public static SavedPlaylistCollection AppPlaylist = new SavedPlaylistCollection();
        
        static string[] delimeter = { " " };

        static CreateHelper() {
            if (AppName.Contains(".vshost"))
            {
               AppName= AppName.Replace(".vshost", "");
            }
            MediaFolderPath = SystemDocumentPath + @"\" + AppName; }

        public static void LoadFiles()
        {
            LoadTreeViewFile();
            LoadPlaylistFile();
        }

        public static void LoadLastSeenFile(IFolder ifolder)
        {
            string path = FileExistOrCreate(ifolder.Directory.FullName + @"\.movies\LastSeen.json");
            if (path != null)
            {
                string jsonfile = File.ReadAllText(path);
                object lastseen = JsonConvert.DeserializeObject<ObservableCollection<PlayedFiles>>(jsonfile);
                if (lastseen != null)
                {
                    ifolder.LastSeenCollection = (ObservableCollection<PlayedFiles>)lastseen;
                }
            }
        }

        public static bool SaveLastSeenFile(IFolder ifolder)
        {
            if (ifolder.LastSeenCollection == null) { return false; }
            else if (ifolder.LastSeenCollection.Count > 0)
            {
                try
                {
                    string path = FileExistOrCreate(ifolder.Directory.FullName + @"\.movies\LastSeen.json", true);
                    string json = JsonConvert.SerializeObject(ifolder.LastSeenCollection, Formatting.Indented);
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

        public static void Folder()
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
                throw;
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
        }

        public static bool LoadTreeViewFile()
        {
            string path = FileExistOrCreate(SettingsFileName);
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
            string path = FileExistOrCreate(PlaylistFileName);
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
        }

        public static bool SaveTreeViewFiles()
        {
            if (AppSettings.TreeViewItems.Count > 0)
            {
                try
                {
                    string path = FileExistOrCreate(SettingsFileName, true);
                    string json = JsonConvert.SerializeObject(AppSettings, Formatting.Indented);
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

        public static bool SavePlaylistFiles()
        {
            if (AppPlaylist.MoviePlayList.Count > 0)
            {
                try
                {
                    string path = FileExistOrCreate(PlaylistFileName, true);
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

        public static string FileExistOrCreate(string path_to_file,bool returnpath = false)
        {
            string path;
            if (path_to_file == SettingsFileName)
            {
                path = MediaFolderPath + @"\" + Settings + @"\" + SettingsFileName;
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
            else if(path_to_file == PlaylistFileName)
            {
                path = MediaFolderPath + @"\" + PlaylistFolderName + @"\" + PlaylistFileName;
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
            else
            {
                if (File.Exists(path_to_file))
                {
                    return path_to_file;
                }
                else
                {
                    File.Create(path_to_file);
                    if (returnpath)
                    {
                        return path_to_file;
                    }
                }
                return null;
            }
        }

        public static void CreateLastSeenFolder(IFolder ifolder)
        {
            string path = ifolder.Directory.FullName + @"\.movies";
            if (ifolder.Directory.Parent != null)
            {
                if (ifolder.Directory.FullName == ifolder.Directory.Parent.FullName + @"\.movies")
                {
                    return;
                }
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                DirectoryInfo di = new DirectoryInfo(path);
                di.Attributes |= FileAttributes.Hidden;
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(path);
                di.Attributes |= FileAttributes.Hidden;
            }
        }
        
    }

    

   
}
