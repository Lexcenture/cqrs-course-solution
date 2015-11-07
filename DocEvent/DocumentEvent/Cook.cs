namespace DocumentEvent
{
    public class Cook : IHandle
    {
        private readonly IHandle _nextHandle;

        public Cook(IHandle nextHandle)
        {
            _nextHandle = nextHandle;
        }

        public void Handle(Order order)
        {
            order.AddIngrediants("Checkpeas");
            order.AddIngrediants("Olive oil");
            order.AddIngrediants("Marination Spices");
            order.AddIngrediants("Lamb dices");

            _nextHandle.Handle(order);
        }
    }
}