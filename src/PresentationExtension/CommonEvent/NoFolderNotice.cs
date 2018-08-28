using Microsoft.Practices.Prism.Events;
using PresentationExtension.EventManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PresentationExtension.CommonEvent
{
    public class NoFolderNoticeEventToken : EventToken<bool> { }
    public class FolderItemChangeEventToken : EventToken<bool> { }
    public class NavigateFolderItemToken : EventToken<object> { }
    public class MessageChangedEventToken : EventToken<object> { }
    public class StatusMessageChangedEventToken : EventToken<bool> { }
    public class NavigateNewPage : EventToken<object> { }
    public class NavigateSearchResult : EventToken<object> { }
    public class PlaylistCollectionChangedEventToken : EventToken<object> { }
    public class IsPlaylistManagerBusy : EventToken<bool> { }

}
