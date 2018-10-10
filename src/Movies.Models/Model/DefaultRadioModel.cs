using Microsoft.Practices.Prism.ViewModel;
using Movies.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.Models.Model
{
    public class DefaultRadioModel : RadioModel
    {
        private Guid radioKey;
        public DefaultRadioModel(Guid guid)
            :base(guid)
        {
            this.radioKey = guid;
        }
        public override Guid Key
        {
            get { return radioKey; }
            set
            {
                if (radioKey == null || radioKey == Guid.Empty)
                    radioKey = value;
            }
        }

        public override bool CanEdit { get { return false; } }

    }
}
