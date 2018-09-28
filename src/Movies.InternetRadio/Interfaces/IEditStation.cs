using Movies.Models.Interfaces;

namespace Movies.InternetRadio.Interfaces
{
    internal interface IEditStation
    {
        IMoviesRadio CurrentRadioStation { get; set; }
    }
}