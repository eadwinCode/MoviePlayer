using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PresentationExtension.CommonEvent
{
    public class EventAggregatorService
    {
        public static IEventManager IEventManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IEventManager>();
            }
        }

        public static IEventAggregator IEventAggregator
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IEventAggregator>();
            }
        }
    }
}
