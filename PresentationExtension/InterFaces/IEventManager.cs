using PresentationExtension.EventManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PresentationExtension.InterFaces
{
    public interface IEventManager
    {
        T GetEvent<T>() where T : EventTokenBase, new();
    }
    public interface ISubscriptionToken<T>
    {
        Action<T> SubcriptionAction { get; }
    }
    public interface IEventToken<T>
    {
        bool Contains(SubscriptionTokenBase Event);
        void Publish(T payload);
        SubscriptionTokenBase Subscribe(Action<T> ExcuteAction);
        void UnSubscribe(Action<T> ExcuteAction);
    }
    public interface ISubscriptionTokenBase
    {
        Guid Id { get; }
    }
}
