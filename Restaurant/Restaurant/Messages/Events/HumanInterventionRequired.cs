namespace Restaurant.Messages.Events
{
    public class HumanInterventionRequired : Message
    {
        private readonly OrderPlaced _message;
        
        public HumanInterventionRequired(OrderPlaced message)
        {
            _message = message;
        }

        public OrderPlaced Message
        {
            get { return _message; }
        }
    }
}