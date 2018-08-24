using PresentationExtension.InterFaces;
using System;

namespace PresentationExtension.EventManager
{
    public class SubscriptionToken<T> : SubscriptionTokenBase, ISubscriptionToken<T>
    {
       
        Action<T> subcription;
        public SubscriptionToken(Action<T> Subcription)
        {
            this.subcription = Subcription;
           
        }
       
        public Action<T> SubcriptionAction { get { return subcription; } }
    }
}
