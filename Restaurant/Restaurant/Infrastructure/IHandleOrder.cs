using Restaurant.Messages;

namespace Restaurant.Infrastructure
{
    public interface IHandle<in T> where T : Message
    {
        void Handle(T message);
    }
}
