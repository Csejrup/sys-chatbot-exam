using EasyNetQ;

namespace SysChatBot.Shared.Messaging;

public class MessageClient(IBus bus) : IMessageClient
{
    public virtual void Send<T>(T message, string topic)
    {
        bus.PubSub.Publish(message, topic);
    }

    public void Listen<T>(Action<T> handler, string topic)
    {
        bus.PubSub.Subscribe(topic, handler);
    }
}
