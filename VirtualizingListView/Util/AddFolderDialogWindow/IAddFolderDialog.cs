using System;

namespace VirtualizingListView.Util.AddFolderDialogWindow
{
    public interface IAddFolderDialog
    {
        event EventHandler OnFinished;
        void ShowDialog();
    }
}