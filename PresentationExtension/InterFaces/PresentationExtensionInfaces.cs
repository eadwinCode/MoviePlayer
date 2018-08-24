using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PresentationExtension.InterFaces
{
    public interface IMainPages
    {
        void SetController(IWindowsCommandButton controller);
        bool HasController { get; }
        NavigationService NavigationService { get; }
        ContentControl Docker { get; }
    }

    public interface IWindowsCommandButton
    {
        void SetActive(bool setactive, bool loadPage);
    }
}
