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

        public Offer(Product product, double argument)
        {
            Argument = argument;
            _product = product;
        }

        public double Argument { get; }

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
        public PercentDiscountOffer(Product product, double argument) : base(product, argument)
        {
        }
        public override Discount ComputeDiscount(double quantity, double unitPrice)
        {
            return new Discount(Product, Argument + "% off", -quantity * unitPrice * Argument / 100.0);
        }
    }
    public class ThreeForTwoOffer : Offer
    {
        public ThreeForTwoOffer(Product product, double argument) : base(product, argument)
        {
        }
        public override Discount ComputeDiscount(double quantity, double unitPrice)
        {
            Discount result = null;
            if ((int)quantity > 2)
            {
                var discountAmount = quantity * unitPrice - ((int)quantity / 3 * 2 * unitPrice + (int)quantity % 3 * unitPrice);
                result = new Discount(Product, "3 for 2", -discountAmount);
            }
            return result;
        }
    }
    public class TwoForAmountOffer : Offer
    {
        public TwoForAmountOffer(Product product, double argument) : base(product, argument)
        {
        }
        public override Discount ComputeDiscount(double quantity, double unitPrice)
        {
            Discount result = null;
            if ((int)quantity >= 2)
            {
                var total = this.Argument * ((int)quantity / 2) + (int)quantity % 2 * unitPrice;
                var discountN = unitPrice * quantity - total;
                result = new Discount(Product, "2 for " + this.Argument, -discountN);
            }
            return result;
        }
    }
    public class FiveForAmountOffer : Offer
    {
        public FiveForAmountOffer(Product product, double argument) : base(product, argument)
        {
        }
        public override Discount ComputeDiscount(double quantity, double unitPrice)
        {
            Discount result = null;
            if ((int)quantity >= 5)
            {
                var discountTotal = unitPrice * quantity - (this.Argument * ((int)quantity / 5) + (int)quantity % 5 * unitPrice);
                result = new Discount(this.Product, 5 + " for " + this.Argument, -discountTotal);
            }
            return result;
        }
    }
}