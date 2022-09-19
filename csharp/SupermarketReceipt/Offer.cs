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

    public class Offer
    {
        private Product _product;

        public Offer(SpecialOfferType offerType, Product product, double argument)
        {
            OfferType = offerType;
            Argument = argument;
            _product = product;
        }

        public SpecialOfferType OfferType { get; }
        public double Argument { get; }

        protected Product Product { get { return _product; } }

        public virtual Discount ComputeDiscount(double quantity, double unitPrice)
        {
            switch (this.OfferType)
            {
                case SpecialOfferType.ThreeForTwo:
                    int xForAmountDiscount = 3;
                    if ((int)quantity > 2)
                    {
                        var discountAmount = quantity * unitPrice - ((int)quantity / xForAmountDiscount * 2 * unitPrice + (int)quantity % 3 * unitPrice);
                        return new Discount(this._product, "3 for 2", -discountAmount);
                    }
                    break;

                case SpecialOfferType.TwoForAmount:

                    xForAmountDiscount = 2;
                    if ((int)quantity >= 2)
                    {
                        var total = this.Argument * ((int)quantity / xForAmountDiscount) + (int)quantity % 2 * unitPrice;
                        var discountN = unitPrice * quantity - total;
                        return new Discount(this._product, "2 for " + this.Argument, -discountN);
                    }

                    break;
                case SpecialOfferType.FiveForAmount:
                    xForAmountDiscount = 5;
                    if ((int)quantity >= 5)
                    {
                        var discountTotal = unitPrice * quantity - (this.Argument * ((int)quantity / xForAmountDiscount) + (int)quantity % 5 * unitPrice);
                        return new Discount(this._product, xForAmountDiscount + " for " + this.Argument, -discountTotal);
                    }
                    break;
                case SpecialOfferType.TenPercentDiscount:
                    return new Discount(this._product, this.Argument + "% off", -quantity * unitPrice * this.Argument / 100.0);
                default:
                    return null;
            }
            return null;
        }
    }
    public class PercentDiscountOffer : Offer
    {
        public PercentDiscountOffer(SpecialOfferType offerType, Product product, double argument) : base(offerType, product, argument)
        {
        }
        public override Discount ComputeDiscount(double quantity, double unitPrice)
        {
            return new Discount(Product, Argument + "% off", -quantity * unitPrice * Argument / 100.0);
        }
    }
    public class ThreeForTwoOffer : Offer
    {
        public ThreeForTwoOffer(SpecialOfferType offerType, Product product, double argument) : base(offerType, product, argument)
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
    public class TwoForAmount : Offer
    {
        public TwoForAmount(SpecialOfferType offerType, Product product, double argument) : base(offerType, product, argument)
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
    public class FiveForAmount : Offer
    {
        public FiveForAmount(SpecialOfferType offerType, Product product, double argument) : base(offerType, product, argument)
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