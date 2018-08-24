using Movies.Themes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Movies.Themes.Skin
{
    public abstract class SkinBase
    {
        private string name;
        private List<ResourceDictionary> resources = new List<ResourceDictionary>();
        public ThemeModel themeModel;
        public string Name
        {
            get { return name; }
        }

        public List<ResourceDictionary> Resources
        {
            get { return resources; }
        }

        public SkinBase(string name,ThemeModel themeModel)
        {
            this.name = name;
            this.themeModel = themeModel;
        }

        public void Load()
        {
            if (resources.Count != 0)
                return;

            LoadResources();
            foreach (var item in Resources)
            {
                Application.Current.Resources.MergedDictionaries.Add(item);
            }
            this.themeModel.IsActive = true;
        }

        public void Unload()
        {
            foreach (var item in Resources)
            {
                Application.Current.Resources.MergedDictionaries.Remove(item);
            }
            Resources.Clear();
            themeModel.IsActive = false;
        }

        public abstract void LoadResources();
       
    }
}
