using System.IO;

namespace Movies.MoviesInterfaces
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
}