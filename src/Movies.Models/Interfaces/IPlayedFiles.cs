namespace Movies.Models.Interfaces
{
    public interface IPlayedFiles
    {
        bool Exist { get; }
        string FileName { get; set; }
        string Percentage { get; set; }
        double ProgressLastSeen { get; set; }

        void RemoveLastSeen();
        string ToString();
    }
}