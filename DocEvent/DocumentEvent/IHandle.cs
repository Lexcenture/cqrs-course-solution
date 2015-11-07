namespace DocumentEvent
{
    public interface IHandle
    {
        void Handle(Order order);
    }
}