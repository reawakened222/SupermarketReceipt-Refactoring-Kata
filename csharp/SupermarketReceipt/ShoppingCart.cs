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

            var bundles = catalog.GetBundles();
            int bundlesAmount = bundles.Count();
            for (int i = 0; i < bundlesAmount; ++i)
            {
                var bundle = bundles[i].GetBundleCopy();
                var bundle_fullfilled = bundles[i].GetBundleCopy();
                foreach (var product in _productQuantities.Keys)
                {
                    if (bundle.ContainsKey(product))
                    {
                        if (bundle[product] <= (double)_productQuantities[product])
                        {
                            bundle_fullfilled[product] = -1;
                        }
                    }
                }
                /* All bundle items purchased => Bundle completed  */
                if (bundle_fullfilled.Values.Where(x => x == -1).Count() == bundle_fullfilled.Count)
                {
                    foreach (var item in bundle)
                    {
                        const double tenPercent = 0.1;
                        receipt.AddDiscount(new Discount(item.Key, "Part of bundle", -(tenPercent * (double)catalog.GetUnitPrice(item.Key) * item.Value)));

                    }
                }
            }

            Discount getDiscount(Dictionary<Product, Offer> offers, SupermarketCatalog catalog, Product currentProduct)
            {
                return (offers.ContainsKey(currentProduct)) ? 
                    offers[currentProduct].ComputeDiscount((double)_productQuantities[currentProduct], (double)catalog.GetUnitPrice(currentProduct))
                    : null;
            }
        }
    }
}