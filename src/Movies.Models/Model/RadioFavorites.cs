using Movies.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.Models.Model
{
    public class RadioFavorites : RadioGroup, IRadioFavorite
    {
        public RadioFavorites()
        {
            RadioName = "Favorite Stations";
        }

        public override string ToString()
        {
            return RadioName;
        }
    }
}
