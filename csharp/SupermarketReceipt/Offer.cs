using System;

namespace SupermarketReceipt
{
    public enum SpecialOfferType
    {
        ThreeForTwo,
        TenPercentDiscount,
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

        public static Offer OfferFactory(SpecialOfferType offerType, Product product, double argument)
        {
            return offerType switch
            {
                SpecialOfferType.ThreeForTwo => new ThreeForTwoOffer(product, argument),
                SpecialOfferType.TenPercentDiscount => new PercentDiscountOffer(product, argument),
                SpecialOfferType.TwoForAmount => new TwoForAmountOffer(product, argument),
                SpecialOfferType.FiveForAmount => new FiveForAmountOffer(product, argument),
                _ => null,
            };
        }

        public abstract Discount ComputeDiscount(double quantity, double unitPrice);
    }
    public class PercentDiscountOffer : Offer
    {
        public PercentDiscountOffer(Product product, double argument) : base(product, argument) { }
        public override Discount ComputeDiscount(double quantity, double unitPrice)
        {
            return new Discount(Product, Argument + "% off", -quantity * unitPrice * Argument / 100.0);
        }
    }
    public class ThreeForTwoOffer : Offer
    {
        public ThreeForTwoOffer(Product product, double argument) : base(product, argument) { }
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
    public class TwoForAmountOffer : Offer
    {
        public TwoForAmountOffer(Product product, double argument) : base(product, argument) { }
        public override Discount ComputeDiscount(double quantity, double unitPrice)
        {
            Discount result = null;
            int quantityAsInt = (int)quantity;
            const int NrInDealPack = 2;
            if (quantityAsInt >= NrInDealPack)
            {
                var total = Argument * (quantityAsInt / NrInDealPack) + quantityAsInt % NrInDealPack * unitPrice;
                var discountN = unitPrice * quantity - total;
                result = new Discount(Product, $"2 for " + Argument, -discountN);
            }
            return result;
        }
    }
    public class FiveForAmountOffer : Offer
    {
        public FiveForAmountOffer(Product product, double argument) : base(product, argument) { }
        public override Discount ComputeDiscount(double quantity, double unitPrice)
        {
            Discount result = null;
            int quantityAsInt = (int)quantity;
            const int NrInDealPack = 5;
            if (quantityAsInt >= NrInDealPack)
            {
                double total = Argument * (quantityAsInt / NrInDealPack) + quantityAsInt % NrInDealPack * unitPrice;
                var discountTotal = unitPrice * quantity - total;
                result = new Discount(this.Product, NrInDealPack + " for " + Argument, -discountTotal);
            }
            return result;
        }
    }
}