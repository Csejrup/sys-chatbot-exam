namespace SysChatBot.Shared;

public interface IMessageClient
{
    void Send<T>(T message, string messageType);
    void Listen<T>(Action<T> handler, string messageType);
}