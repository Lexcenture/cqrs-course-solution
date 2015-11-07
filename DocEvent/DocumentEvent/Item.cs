namespace DocumentEvent
{
    public class Item
    {
        private readonly int _id;
        private readonly string _description;
        private readonly int _quantity;
        private readonly decimal _price;

        public Item(int id, string description, int quantity, decimal price)
        {
            _id = id;
            _description = description;
            _quantity = quantity;
            _price = price;
        }

        public int Id
        {
            get { return _id; }
        }

        public string Description
        {
            get { return _description; }
        }

        public int Quantity
        {
            get { return _quantity; }
        }

        public decimal Price
        {
            get { return _price; }
        }

        protected bool Equals(Item other)
        {
            return _id == other._id && string.Equals(_description, other._description) && _quantity == other._quantity && _price == other._price;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Item) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _id;
                hashCode = (hashCode*397) ^ (_description != null ? _description.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ _quantity;
                hashCode = (hashCode*397) ^ _price.GetHashCode();
                return hashCode;
            }
        }
    }
}