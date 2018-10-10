using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.Models.Model
{
    public class DefaultRadioGroup : RadioGroup
    {
        private Guid radioKey;

        public DefaultRadioGroup():base()
        {

        }

        public override bool CanEdit { get { return false; } }

        public override Guid Key
        {
            get { return radioKey; }
            set
            {
                if (radioKey == null || radioKey == Guid.Empty)
                    radioKey = value;
            }
        }
    }
}
