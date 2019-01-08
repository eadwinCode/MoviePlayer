using Movies.Models.Model;
using System.Collections.Generic;
using System.IO;

namespace Movies.Models.Interfaces
{
    public interface IMovieFolderModel
    {
        DirectoryInfo DirectoryInfo { get; }
        bool Exists { get; }
        string Extension { get; }
        string FullName { get; }
        string Name { get; }

        void Delete();
        bool Equals(object obj);
        string ToString();
    }

    public interface IFileSystemWatcher
    {
        void UnloadWatch();
    }

    public interface ISearchModel
    {
        ICollection<MediaFolder> Results { get; set; }
        string SearchQuery { get; set; }
    }
}