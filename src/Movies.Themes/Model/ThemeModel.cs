using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.Themes.Model
{
    public class ThemeModel : NotificationObject
    {
        public string ThemeName { get; set; }
        public string ThemePath { get; set; }
        bool isactive;
        public bool IsActive
        {
            get { return isactive; }
            set
            {
                isactive = value; RaisePropertyChanged(() => this.IsActive);
            }
        }
    }
}
