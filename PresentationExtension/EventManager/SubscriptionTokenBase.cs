using PresentationExtension.InterFaces;
using System;

namespace PresentationExtension.EventManager
{
    public abstract class SubscriptionTokenBase 
    {
        Guid id;
        protected Guid Id { get { return id; } }
        public SubscriptionTokenBase()
        {
            id = Guid.NewGuid();
        }
    }
}
