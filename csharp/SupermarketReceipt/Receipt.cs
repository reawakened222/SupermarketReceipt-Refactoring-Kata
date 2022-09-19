using System.Collections.Generic;
using System.Linq;

namespace SupermarketReceipt
{
    public class Receipt
    {
        private readonly List<Discount> _discounts = new List<Discount>();
        private readonly List<ReceiptItem> _items = new List<ReceiptItem>();

        public double GetTotalPrice()
        {
            var total = _items.Aggregate(0.0, (x, nextItem) => x + nextItem.TotalPrice) + 
                _discounts.Aggregate(0.0, (x, nextItem) => x + nextItem.DiscountAmount);

            return total;
        }

        public void AddProduct(Product p, double quantity, double price)
        {
            _items.Add(new ReceiptItem(p, quantity, price, quantity * price));
        }

        public List<ReceiptItem> GetItems()
        {
            return new List<ReceiptItem>(_items);
        }

        public void AddDiscount(Discount discount)
        {
            _discounts.Add(discount);
        }

        public void AddDiscounts(IEnumerable<Discount> discounts)
        {
            foreach (var discount in discounts) 
                AddDiscount(discount);
        }

        public List<Discount> GetDiscounts()
        {
            return _discounts;
        }
    }

    public class ReceiptItem
    {
        public ReceiptItem(Product p, double quantity, double price, double totalPrice)
        {
            Product = p;
            Quantity = quantity;
            Price = price;
            TotalPrice = totalPrice;
        }

        public Product Product { get; }
        public double Price { get; }
        public double TotalPrice { get; }
        public double Quantity { get; }
    }
}