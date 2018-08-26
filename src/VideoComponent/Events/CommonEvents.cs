using Microsoft.Practices.Prism.Events;
using Movies.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoComponent.BaseClass;

namespace VideoComponent.Events
{
   
    public class LoadExecuteCommandEvent : CompositePresentationEvent<VideoFolder> { }

    public class CloseFireExplorerEvent : CompositePresentationEvent<object> { }
    public class TreeViewSelection : CompositePresentationEvent<string> { }
    public class PlayExecuteCommandEvent : CompositePresentationEvent<VideoFolderChild> { }

  //  public class RefreshExecuteCommandEvent : CompositePresentationEvent<VideoData> { }

    public class NextCommandEvent : CompositePresentationEvent<String> { }
    public class PreviousCommandEvent : CompositePresentationEvent<String> { }
}
