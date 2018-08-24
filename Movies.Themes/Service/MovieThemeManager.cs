using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Movies.Themes.Model;
using Movies.Themes.Skin;

namespace Movies.Themes.Service
{
    public class MovieThemeManager
    {
        SkinBase currentskin;
        public SkinBase CurrentSkin
        {
            get { return currentskin; }
        }

        public MovieThemeManager()
        {
            //currentskin = new ReferencedAssemblySkin("Black Skin",
            //    new Uri("/Movies.Themes;component/Themes/Hybrid.xaml", UriKind.Relative));
        }

        private void OnCurrentSkinChange()
        {
            CurrentSkin.Load();
        }

        public void SetNewCurrent(ThemeModel themeModel)
        {
            if (currentskin != null)
                currentskin.Unload();

            currentskin = new ReferencedAssemblySkin(themeModel.ThemeName, new Uri(themeModel.ThemePath, UriKind.Relative),themeModel);
            OnCurrentSkinChange();
        }
    }
}
