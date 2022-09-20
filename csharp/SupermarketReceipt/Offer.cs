using System;
using System.Collections.Generic;

namespace SupermarketReceipt
{
    public enum SpecialOfferType
    {
        ThreeForTwo,
        PercentageDiscount,
        TwoForAmount,
        FiveForAmount
    }

    public abstract class Offer
    {
        private Product _product;

        protected Offer(Product product, double argument)
        {
            Argument = argument;
            _product = product;
        }

        protected double Argument { get; }

        protected Product Product { get { return _product; } }

        public static Offer GetOffer(SpecialOfferType offerType, Product product, double argument)
        {
            return offerType switch
            {
                SpecialOfferType.ThreeForTwo => new ThreeForTwoOffer(product),
                SpecialOfferType.PercentageDiscount => new PercentageDiscountOffer(product, argument),
                SpecialOfferType.TwoForAmount => new NItemsForAmountOffer(product, 2, argument),
                SpecialOfferType.FiveForAmount => new NItemsForAmountOffer(product, 5, argument),
                _ => null,
            };
        }

        public abstract Discount ComputeDiscount(double quantity, double unitPrice);
    }
    class PercentageDiscountOffer : Offer
    {
        public PercentageDiscountOffer(Product product, double argument) : base(product, argument) { }
        public override Discount ComputeDiscount(double quantity, double unitPrice)
        {
            return new Discount(Product, Argument + "% off", -quantity * unitPrice * Argument / 100.0);
        }
    }
    class ThreeForTwoOffer : Offer
    {
        public ThreeForTwoOffer(Product product) : base(product, -1) { }
        public override Discount ComputeDiscount(double quantity, double unitPrice)
        {
            Discount result = null;
            int quantityAsInt = (int)quantity;
            if (quantityAsInt > 2)
            {
                var discountAmount = quantity * unitPrice - (quantityAsInt / 3 * 2 * unitPrice + quantityAsInt % 3 * unitPrice);
                result = new Discount(Product, "3 for 2", -discountAmount);
            }
            return result;
        }
    }
    class NItemsForAmountOffer : Offer
    {
        private readonly int _numberOfItems;
        public NItemsForAmountOffer(Product product, int numberOfItems, double argument) : base(product, argument)
        {
            _numberOfItems = numberOfItems;
        }
        public override Discount ComputeDiscount(double quantity, double unitPrice)
        {
            Discount result = null;
            int quantityAsInt = (int)quantity;
            if (quantityAsInt >= _numberOfItems)
            {
                var total = Argument * (quantityAsInt / _numberOfItems) + quantityAsInt % _numberOfItems * unitPrice;
                var discountTotal = unitPrice * quantity - total;
                result = new Discount(Product, $"{_numberOfItems} for {Argument}", -discountTotal);
            }
            return result;
        }
    }

    public class Bundle
    {
        private Dictionary<Product, double> _bundle;

        public Bundle()
        {
            _bundle = new Dictionary<Product, double>();
        }

        public void AddItem(Product product, double quantity)
        {
            _bundle.Add(product, quantity);
        }

        public Dictionary<Product, double> GetBundleCopy()
        {
            return new Dictionary<Product, double>(_bundle);
        }
    }
}