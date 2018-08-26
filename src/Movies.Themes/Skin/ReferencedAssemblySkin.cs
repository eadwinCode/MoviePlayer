using Movies.Themes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Movies.Themes.Skin
{
    public class ReferencedAssemblySkin : SkinBase
    {
        private readonly Uri resourceUri;
        public ReferencedAssemblySkin(string name,Uri uri, ThemeModel themeModel)
            :base(name,themeModel)
        {
            this.resourceUri = uri;
        }
        public override void LoadResources()
        {
            ResourceDictionary resource = (ResourceDictionary)Application.LoadComponent(resourceUri);
            Resources.Add(resource);
        }
    }
}
