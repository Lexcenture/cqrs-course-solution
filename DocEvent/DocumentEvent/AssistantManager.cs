using System.Linq;

namespace DocumentEvent
{
    public class AssistantManager : IHandle
    {
        private readonly IHandle _nextHandle;

        public AssistantManager(IHandle nextHandle)
        {
            _nextHandle = nextHandle;
        }

        public void Handle(Order order)
        {
            order.Total = order.GetItems().Sum(x => x.Price*x.Quantity);
            order.Tax = order.Total*0.2m;
            order.SubTotal = order.Total + order.Tax;

            _nextHandle.Handle(order);
        }
    }
}