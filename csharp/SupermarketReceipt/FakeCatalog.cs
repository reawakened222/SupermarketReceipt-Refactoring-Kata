using System.Collections.Generic;

namespace SupermarketReceipt
{
    public class FakeCatalog : SupermarketCatalog
    {
        private readonly IDictionary<string, double> _prices = new Dictionary<string, double>();
        private readonly IDictionary<string, Product> _products = new Dictionary<string, Product>();
        private readonly List<Bundle> _bundles = new List<Bundle>();
        public void AddProduct(Product product, double price)
        {
            if (product == null) throw new System.ArgumentNullException(nameof(product));
            if (price < 0.0) throw new System.ArgumentOutOfRangeException(nameof(price), "The price may not be negative.");
            _products.Add(product.Name, product);
            _prices.Add(product.Name, price);
        }
        public void AddBundleOffer(Bundle bundle)
        {
            _bundles.Add(bundle);
        }

        public double GetUnitPrice(Product p)
        {
            return _prices[p.Name];
        }

        public List<Bundle> GetBundles()
        {
            return new List<Bundle>(_bundles);
        }
    }
}