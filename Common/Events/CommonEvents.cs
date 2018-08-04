using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Events
{
    class CommonEvents
    {
    }

    public class LongProcessStartedEvent : CompositePresentationEvent<string> { }
    public class LongProcessCompletedEvent : CompositePresentationEvent<int> { }
}
