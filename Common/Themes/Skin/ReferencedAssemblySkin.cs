using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Common.Themes.Skin
{
    public class ReferencedAssemblySkin : Skin
    {
        private readonly Uri resourceUri;
        public ReferencedAssemblySkin(string name,Uri uri)
            :base(name)
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
