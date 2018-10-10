using Movies.Models.Model;
using Movies.MoviesInterfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Movies.MovieServices.Services
{
    public class ApplicationService : IApplicationService
    {
        public const string Settings = "Settings";
        private const string SettingsFileName = "Settings.json";
        private const string PlaylistFileName = "Playlist.pl";
        private const string PlaylistFolderName = "PlaylistFolder";
        private const string LastSeenFolderName = "LastSeen";
        private const string LastSeenFileName = "LastPlayed.sxc";
        private const string RadioFileName = "ghdrad.sxc";

        private static string AppName = Path.GetFileNameWithoutExtension(AppDomain.
            CurrentDomain.FriendlyName);
        private static string SystemDocumentPath = Environment.GetFolderPath(Environment.
            SpecialFolder.MyDocuments);
        
        private  string MediaFolderPath;
        public  Settings AppSettings { get; private set; }
       
        public SavedPlaylistCollection AppPlaylist { get; private set; }
        public SavedLastSeenCollection SavedLastSeenCollection { get; private set; }
        public SavedRadioCollection SavedRadioCollection { get; private set; }
        static string[] delimeter = { " " };

        private IDictionary<string,string> formats = new Dictionary<string, string>()
        {
           { ".wmv" ,"wmv"}, {".3gp","3gp" } ,{ ".avi","avi" }, {".divx","divx" }, {".dv","dv" }, {".flv","flv" },{".m4v" ,"m4v"},
            { ".mkv","mkv" }, {".mov","mov" },{ ".mp4","mp4" }, {".mpeg","mpeg" },
            { ".mpeg1","mpeg1" }, {".mpeg2", "mpeg2"},{".mpeg4","mpeg4" },{".mpg","mpg" },{".ogg","ogg" },{".rec","rec" },{".rm","rm" }, {".rmvb","rmvb" },
            { ".vob","vob" },{".webm" ,"webm"}
              };
        public IDictionary<string, string> Formats
        {
            get { return formats; }
        }
        private string[] subtitleformats =
        {
          ".aqt ",".vtt",".cvd",".dks" ,".jss",".sub" ,".ttxt" ,".mpl" ,".txt" ,".pjs" ,".psb"
          ,".rt",".smi" ,".ssf" ,".srt" ,".ssa" ,".svcd",".usf" ,".idx"
        };

        public string[] SubtitleFormats { get { return subtitleformats; } }

        public ApplicationService()
        {
            AppPlaylist = new SavedPlaylistCollection();
            SavedLastSeenCollection = new SavedLastSeenCollection();
            AppSettings = new Settings();
            SavedRadioCollection = new SavedRadioCollection();

            if (AppName.Contains(".vshost"))
            {
                AppName = AppName.Replace(".vshost", "");
            }
            MediaFolderPath = SystemDocumentPath + @"\" + AppName;
        }

        public  void LoadFiles()
        {
            LoadTreeViewFile();
            LoadPlaylistFile();
            LoadLastSeenFile();
            LoadRadioFiles();
        }

        public object LoadRadioFiles(Stream file)
        {
            try
            {
                StreamReader streamReader = new StreamReader(file);
                string jsonfile = streamReader.ReadToEnd();
                object radios = JsonConvert.DeserializeObject<SavedRadioCollection>(jsonfile, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                });
                return radios;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void LoadRadioFiles()
        {
            string path = FileExistOrCreate(@"\" + Settings + @"\" + RadioFileName);
            if (path != null)
            {
                string jsonfile = File.ReadAllText(path);
                object radios = JsonConvert.DeserializeObject<SavedRadioCollection>(jsonfile, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                });
                if (radios != null)
                {
                    SavedRadioCollection = (SavedRadioCollection)radios;
                }
            }
        }

        public void LoadLastSeenFile()
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

        public bool SaveLastSeenFile()
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

        public void CreateFolder()
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
        
        void CreateSubFolder(string path)
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

        public bool LoadTreeViewFile()
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

        public bool LoadPlaylistFile()
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

        public void SaveFiles()
        {
            SaveMovieFolders();
            SavePlaylistFiles();
            SaveLastSeenFile();
            SaveRadioData();
        }

        private bool SaveRadioData()
        {
            try
            {
                var typeBind = new IMovieRadioSeriationBinder("Movies.MovieServices.Services");
                string path = FileExistOrCreate(@"\" + Settings + @"\" + RadioFileName, true);
                string json = JsonConvert.SerializeObject(SavedRadioCollection, Formatting.Indented,new JsonSerializerSettings {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple

                });
                File.WriteAllText(path, json);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool SaveMovieFolders()
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

        public bool SavePlaylistFiles()
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

        public string FileExistOrCreate(string path_to_file, bool returnpath = false)
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
