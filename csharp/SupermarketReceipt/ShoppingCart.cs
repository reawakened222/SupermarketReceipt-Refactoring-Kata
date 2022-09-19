using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SupermarketReceipt
{
    public class ShoppingCart
    {
        private readonly List<ProductQuantity> _items = new List<ProductQuantity>();
        private readonly Dictionary<Product, double> _productQuantities = new Dictionary<Product, double>();


        public List<ProductQuantity> GetItems()
        {
            return new List<ProductQuantity>(_items);
        }

        public void AddItem(Product product)
        {
            AddItemQuantity(product, 1.0);
        }


        public void AddItemQuantity(Product product, double quantity)
        {
            _items.Add(new ProductQuantity(product, quantity));
            if (_productQuantities.ContainsKey(product))
            {
                var newAmount = _productQuantities[product] + quantity;
                _productQuantities[product] = newAmount;
            }
            else
            {
                _productQuantities.Add(product, quantity);
            }
        }

        public void HandleOffers(Receipt receipt, Dictionary<Product, Offer> offers, SupermarketCatalog catalog)
        {
            IEnumerable<Discount> validDiscounts = _productQuantities.Keys.Select(currentProduct => getDiscount(offers, catalog, currentProduct)).Where(discount => discount != null);
            receipt.AddDiscounts(validDiscounts);


            Discount getDiscount(Dictionary<Product, Offer> offers, SupermarketCatalog catalog, Product currentProduct)
            {
                return (offers.ContainsKey(currentProduct)) ? 
                    offers[currentProduct].ComputeDiscount((double)_productQuantities[currentProduct], (double)catalog.GetUnitPrice(currentProduct))
                    : null;
            }
        }
    }
}