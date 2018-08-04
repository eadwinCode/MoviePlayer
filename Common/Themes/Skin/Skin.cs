using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Common.Themes.Skin
{
    public abstract class Skin
    {
        private string name;
        private List<ResourceDictionary> resources = new List<ResourceDictionary>();

        public string Name
        {
            get { return name; }
        }

        public List<ResourceDictionary> Resources
        {
            get { return resources; }
        }

        public Skin(string name)
        {
            this.name = name;
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
        }

        public void Unload()
        {
            foreach (var item in Resources)
            {
                Application.Current.Resources.MergedDictionaries.Remove(item);
            }
            Resources.Clear();
        }

        public abstract void LoadResources();
       
    }
}
