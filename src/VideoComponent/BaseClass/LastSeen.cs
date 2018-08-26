using Common.ApplicationCommands;
using Common.Util;
using Microsoft.Practices.Prism.ViewModel;
using Movies.Models.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace VideoComponent.BaseClass
{

    
    

    public class Parent : IParentData
    {
        private VideoFolder _parentdirectory;
        public VideoFolder GetParentDirectory
        {
            get { return _parentdirectory; }
        }

        public Parent(VideoFolder parentdirectory)
        {
            this._parentdirectory = parentdirectory;
        }
    }

   
}
