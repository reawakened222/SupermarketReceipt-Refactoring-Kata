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
            foreach (var currentProduct in _productQuantities.Keys)
            {
                var quantity = _productQuantities[currentProduct];
                if (offers.ContainsKey(currentProduct))
                {
                    var offer = offers[currentProduct];
                    var unitPrice = catalog.GetUnitPrice(currentProduct);
                    Discount discount = ComputeDiscount(currentProduct, quantity, offer, unitPrice);

                    if (discount != null)
                        receipt.AddDiscount(discount);
                }
            }

            static Discount ComputeDiscount(Product aProduct, double quantity, Offer offer, double unitPrice)
            {
                switch (offer.OfferType)
                {
                    case SpecialOfferType.ThreeForTwo:
                        int xForAmountDiscount = 3;
                        if ((int)quantity > 2)
                        {
                            var discountAmount = quantity * unitPrice - ((int)quantity / xForAmountDiscount * 2 * unitPrice + (int)quantity % 3 * unitPrice);
                            return new Discount(aProduct, "3 for 2", -discountAmount);
                        }
                        break;

                    case SpecialOfferType.TwoForAmount:

                        xForAmountDiscount = 2;
                        if ((int)quantity >= 2)
                        {
                            var total = offer.Argument * ((int)quantity / xForAmountDiscount) + (int)quantity % 2 * unitPrice;
                            var discountN = unitPrice * quantity - total;
                            return new Discount(aProduct, "2 for " + offer.Argument, -discountN);
                        }

                        break;
                    case SpecialOfferType.FiveForAmount:
                        xForAmountDiscount = 5;
                        if ((int)quantity >= 5)
                        {
                            var discountTotal = unitPrice * quantity - (offer.Argument * ((int)quantity / xForAmountDiscount) + (int)quantity % 5 * unitPrice);
                            return new Discount(aProduct, xForAmountDiscount + " for " + offer.Argument, -discountTotal);
                        }
                        break;
                    case SpecialOfferType.TenPercentDiscount:
                        return new Discount(aProduct, offer.Argument + "% off", -quantity * unitPrice * offer.Argument / 100.0);
                    default:
                        return null;
                }
                return null;
            }
        }
    }
}