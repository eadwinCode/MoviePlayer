
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using VideoComponent.BaseClass;
using VideoComponent.Events;

namespace VideoComponent
{
    //public class DoubleClick : ICommand
    //{
    //    readonly static IEventAggregator _aggregator;
    //    static DoubleClick()
    //    {
    //        _aggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
    //    }
    //    public bool CanExecute(object parameter)
    //    {
    //        return true;
    //    }

    //    public event EventHandler CanExecuteChanged;

    //    public void Execute(object parameter)
    //    {
    //        var obj = parameter as VideoFolder;
    //        if (obj.Directory.Exists)
    //        {
    //            _aggregator.GetEvent<LoadExecuteCommandEvent>().Publish(obj);
    //        }
    //        else
    //        {
    //            _aggregator.GetEvent<PlayExecuteCommandEvent>().Publish(obj as VideoFolderChild);
    //        }
    //    }
    //}
}
