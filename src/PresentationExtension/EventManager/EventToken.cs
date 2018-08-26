using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PresentationExtension.EventManager
{
    public abstract class EventTokenBase
    {
        IDictionary<int, SubscriptionTokenBase> subscriptionactions;
        protected IDictionary<int, SubscriptionTokenBase> SubscriptionActions
        {
            get { return subscriptionactions; }
        }
        public EventTokenBase()
        {
            subscriptionactions = new Dictionary<int, SubscriptionTokenBase>();
        }
    }
    public class EventToken<T> : EventTokenBase, IEventToken<T>
    {
        public bool Contains(SubscriptionTokenBase Event)
        {
            return (SubscriptionActions.Values.Contains(Event));
        }

        public void Publish(T payload)
        {
            foreach (ISubscriptionToken<T> item in SubscriptionActions.Values)
            {
                item.SubcriptionAction.Invoke(payload);
            }
        }

        public SubscriptionTokenBase Subscribe(Action<T> ExcuteAction)
        {
            SubscriptionToken<T> subscriptionToken = new SubscriptionToken<T>(ExcuteAction);
            if(!SubscriptionActions.ContainsKey(ExcuteAction.Method.MetadataToken))
                SubscriptionActions.Add(ExcuteAction.Method.MetadataToken,subscriptionToken);
            return subscriptionToken;
        }

        public void UnSubscribe(Action<T> ExcuteAction)
        {
            if (SubscriptionActions.ContainsKey(ExcuteAction.Method.MetadataToken))
                SubscriptionActions.Remove(ExcuteAction.Method.MetadataToken);
        }
    }
}
