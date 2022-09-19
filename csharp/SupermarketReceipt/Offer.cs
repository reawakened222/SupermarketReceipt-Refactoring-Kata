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

        public Discount ComputeDiscount(double quantity, double unitPrice)
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
}