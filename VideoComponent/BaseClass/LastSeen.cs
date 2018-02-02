using Common.ApplicationCommands;
using Common.Interfaces;
using Common.Model;
using Common.Util;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace VideoComponent.BaseClass
{
   public class LastSeenHelper
    {

       private static ILastSeen Currentobj;

       public static void AddLastSeen(IFolder ifolder,ILastSeen obj)
       {
            if (ifolder.LastSeenCollection == null)
                ifolder.LastSeenCollection = new ObservableCollection<PlayedFiles>();

            ifolder.LastSeenCollection.Add((PlayedFiles)obj);
            (obj as PlayedFiles).SetInCollection();
       }

       //public static ObservableCollection<PlayedFiles> GetLastSeenCollection()
       //{
       //    return CreateHelper.LastSeenCollection;
       //}

       private static bool HasItem(IFolder ifolder,string obj)
       {
            if (ifolder.LastSeenCollection == null)
            {
                return false;
            }
           Currentobj = ifolder.LastSeenCollection.FirstOrDefault(x => x.FileName.Contains(obj));
           return Currentobj != null;
       }

       public static ILastSeen GetProgress(IFolder ifolder,String obj)
       {
           if (HasItem(ifolder,obj))
           {
               return Currentobj;
           }
           return null;
       }

       public static void RemoveLastSeen(IFolder ifolder, ILastSeen obj)
       {
            if (ifolder.LastSeenCollection == null) return;
            PlayedFiles playdfile = obj as PlayedFiles;
            if (ifolder.LastSeenCollection.Contains(playdfile))
            {
                ifolder.LastSeenCollection.Remove((PlayedFiles)obj);
            }
       }

       internal static bool HasSeenItem(IFolder ifolder,ILastSeen lastSeen)
       {
            return ifolder.LastSeenCollection.Contains((PlayedFiles)lastSeen);
       }

        //public static void Update(ILastSeen currentVideoItem)
        //{
        //    RemoveLastSeen(currentVideoItem);
        //    AddLastSeen(currentVideoItem);
        //}
    }

    

    //public class LastSeen : ILastSeen
    //{

    //}

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
