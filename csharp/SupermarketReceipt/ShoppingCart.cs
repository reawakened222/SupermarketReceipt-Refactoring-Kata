using System.Collections;
using System.Collections.Generic;

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
            //TODO: https://refactoring.com/catalog/replaceLoopWithPipeline.html
            foreach (var currentProduct in _productQuantities.Keys)
            {
                Discount discount = getDiscount(offers, catalog, currentProduct);

                if (discount != null)
                    receipt.AddDiscount(discount);
            }

            Discount getDiscount(Dictionary<Product, Offer> offers, SupermarketCatalog catalog, Product currentProduct)
            {
                if (!offers.ContainsKey(currentProduct))
                    return null;

                var offer = offers[currentProduct];
                return offer.ComputeDiscount((double)_productQuantities[currentProduct], (double)catalog.GetUnitPrice(currentProduct));
            }
        }
    }
}