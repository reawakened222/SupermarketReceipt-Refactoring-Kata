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
                Discount discount = null;
                var quantityAsInt = (int)quantity;
                var xForAmountDiscount = 1;
                if (offer.OfferType == SpecialOfferType.ThreeForTwo)
                {
                    xForAmountDiscount = 3;
                    if (quantityAsInt > 2)
                    {
                        var discountAmount = quantity * unitPrice - (quantityAsInt / xForAmountDiscount * 2 * unitPrice + quantityAsInt % 3 * unitPrice);
                        discount = new Discount(aProduct, "3 for 2", -discountAmount);
                    }
                }
                else if (offer.OfferType == SpecialOfferType.TwoForAmount)
                {
                    xForAmountDiscount = 2;
                    if (quantityAsInt >= 2)
                    {
                        var total = offer.Argument * (quantityAsInt / xForAmountDiscount) + quantityAsInt % 2 * unitPrice;
                        var discountN = unitPrice * quantity - total;
                        discount = new Discount(aProduct, "2 for " + offer.Argument, -discountN);
                    }
                }
                if (offer.OfferType == SpecialOfferType.FiveForAmount)
                {
                    xForAmountDiscount = 5;
                    if (quantityAsInt >= 5)
                    {
                        var discountTotal = unitPrice * quantity - (offer.Argument * (quantityAsInt / xForAmountDiscount) + quantityAsInt % 5 * unitPrice);
                        discount = new Discount(aProduct, xForAmountDiscount + " for " + offer.Argument, -discountTotal);
                    }
                }
                if (offer.OfferType == SpecialOfferType.TenPercentDiscount)
                    discount = new Discount(aProduct, offer.Argument + "% off", -quantity * unitPrice * offer.Argument / 100.0);

                return discount;
            }
        }
    }
}