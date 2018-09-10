using Movies.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.MoviesInterfaces
{
    public interface IRadioService
    {
        bool IsRadioOn { get; }
        void ShutdownRadio();
        IPlayFile FileplayerManager { get; }
        void PlayRadio(RadioModel radioModel);
    }
}
