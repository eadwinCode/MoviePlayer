using Movies.Enums;
using Movies.Models.Model;

namespace Movies.Models.Interfaces
{
    public interface IRadioModel : IPlayable, IMoviesRadio
    {
        string Genre { get; set; }
        string AudioFormat { get; set; }
        int BitRate { get; set; }
        bool IsFavorite { get; set; }
        Channel Channels { get; set; }
        double Frequency { get; set; }
        int SampleRate { get; set; }
        string StationBio { get; set; }
        string StationLocation { get; set; }
       // string StationName { get; set; }
        string StationURL { get; set; }
        string TooltipMessage { get; }
        string Country { get; set; }
        object Clone();
       // void Add();
        //void Remove();
    }

    public interface IRadioFavorite
    {
        
    }
}