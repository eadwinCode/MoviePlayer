using PresentationExtension.EventManager;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PresentationExtension.EventManager
{
    public class MovieEventManager : IEventManager
    {
        IDictionary<Type, EventTokenBase> EventsDictionary;
        public MovieEventManager()
        {
            EventsDictionary = new Dictionary<Type, EventTokenBase>();
        }

        public T GetEvent<T>()where T:EventTokenBase,new()
        {
            var eventType = typeof(T);
            if (EventsDictionary.ContainsKey(eventType))
                return EventsDictionary[eventType] as T;
            var eventtype = new T();
            EventsDictionary.Add(typeof(T), eventtype);
            return eventtype;
        }
    }
}
