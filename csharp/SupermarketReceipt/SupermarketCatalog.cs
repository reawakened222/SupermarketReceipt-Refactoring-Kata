using System.Collections.Generic;

namespace SupermarketReceipt
{
    public interface SupermarketCatalog
    {
        void AddProduct(Product product, double price);

        void AddBundleOffer(Bundle bundle);

        List<Bundle> GetBundles();

        double GetUnitPrice(Product product);
    }
}