using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;

namespace SupermarketReceipt.NUnit_Test
{
    [TestFixture]
    public class SupermarketTest
    {
        SupermarketCatalog catalog = new FakeCatalog();
        [SetUp]
        public void Setup()
        {
            catalog = new FakeCatalog();
        }
        [TestCase]
        public void EmptyCart()
        {
            ShoppingCart cart = new();

            Teller teller = new(catalog);

            Receipt receipt = teller.ChecksOutArticlesFrom(cart);

            Assert.Multiple(() =>
            {
                Assert.That(receipt.GetItems().Count, Is.EqualTo(0));
                Assert.That(receipt.GetTotalPrice(), Is.EqualTo(0));
            });
        }
        [TestCase]
        public void NullCart()
        {
            Teller teller = new(catalog);

            Receipt receipt = teller.ChecksOutArticlesFrom(null);

            Assert.Multiple(() =>
            {
                Assert.That(receipt.GetItems().Count, Is.EqualTo(0));
                Assert.That(receipt.GetTotalPrice(), Is.EqualTo(0));
            });
        }
        [TestCase]
        public void TenPercentDiscount()
        {
            Product toothbrush = new("toothbrush", ProductUnit.Each);
            catalog.AddProduct(toothbrush, 0.99);
            Product apples = new("apples", ProductUnit.Kilo);
            catalog.AddProduct(apples, 1.99);

            ShoppingCart cart = new();
            cart.AddItemQuantity(apples, 2.5);

            Teller teller = new(catalog);
            teller.AddSpecialOffer(SpecialOfferType.PercentageDiscount, toothbrush, 10.0);

            // ACT
            Receipt receipt = teller.ChecksOutArticlesFrom(cart);

            // ASSERT
            Assert.Multiple(() =>
            {

                Assert.That(receipt.GetTotalPrice(), Is.EqualTo(4.975));
                Assert.That(receipt.GetDiscounts(), Is.EqualTo(new List<Discount>()));
                Assert.That(receipt.GetItems().Count, Is.EqualTo(1));
            });
            ReceiptItem receiptItem = receipt.GetItems()[0];
            Assert.Multiple(() =>
            {
                Assert.That(receiptItem.Product, Is.EqualTo(apples));
                Assert.That(receiptItem.Price, Is.EqualTo(1.99));
                Assert.That(receiptItem.TotalPrice, Is.EqualTo(2.5 * 1.99));
                Assert.That(receiptItem.Quantity, Is.EqualTo(2.5));
            });
        }

        [TestCase]
        public void BuyTwoGetOneFree()
        {
            Product toothBrush = new("toothbrush", ProductUnit.Each);
            const double TOOTHBRUSH_PRICE = 0.99;
            catalog.AddProduct(toothBrush, TOOTHBRUSH_PRICE);
            Teller teller = new(catalog);
            teller.AddSpecialOffer(SpecialOfferType.ThreeForTwo, toothBrush, 0.0);

            ShoppingCart cart = new();
            cart.AddItemQuantity(toothBrush, 3);

            Receipt receipt = teller.ChecksOutArticlesFrom(cart);

            Assert.That(receipt.GetTotalPrice(), Is.EqualTo(2 * TOOTHBRUSH_PRICE));
        }

        [TestCase]
        public void TwentyPercentDiscount()
        {
            Product apple = new("Apple", ProductUnit.Kilo);

            catalog.AddProduct(apple, 1.99);

            Teller teller = new(catalog);
            teller.AddSpecialOffer(SpecialOfferType.PercentageDiscount, apple, 20.0);

            ShoppingCart cart = new();
            cart.AddItemQuantity(apple, 10.0);
            Receipt receipt = teller.ChecksOutArticlesFrom(cart);

            Assert.That(receipt.GetDiscounts(), Has.Count.EqualTo(1));
            Discount discount = receipt.GetDiscounts()[0];

            Assert.That(discount.Product, Is.EqualTo(apple));
            Assert.That(discount.Description, Is.EqualTo("20% off"));
            Assert.That(receipt.GetTotalPrice(), Is.EqualTo((0.8 * 19.90)).Within(0.0001));
        }

        [TestCase]
        public void CheckoutMultipleItems()
        {
            Product toothbrush = new("Toothbrush", ProductUnit.Each);
            catalog.AddProduct(toothbrush, 0.99);
            Product rice = new("Rice", ProductUnit.Each);
            catalog.AddProduct(rice, 2.49);

            ShoppingCart cart = new();
            cart.AddItem(toothbrush);
            cart.AddItemQuantity(rice, 5);

            Teller teller = new(catalog);

            Receipt receipt = teller.ChecksOutArticlesFrom(cart);
            Assert.Multiple(() =>
            {
                Assert.That(receipt.GetDiscounts(), Has.Count.EqualTo(0));
                Assert.That(receipt.GetItems(), Has.Count.EqualTo(2));
            });
            List<ReceiptItem> items = receipt.GetItems();
            Assert.That(items[0].Product, Is.EqualTo(toothbrush));
            Assert.That(items[1].Product, Is.EqualTo(rice));
        }

        [TestCase]
        public void TwoForAmountDiscount()
        {
            Product tomatoBox = new("Box of Tomatoes", ProductUnit.Each);
            catalog.AddProduct(tomatoBox, 0.69);

            ShoppingCart cart = new();
            cart.AddItemQuantity(tomatoBox, 2);

            Teller teller = new(catalog);
            teller.AddSpecialOffer(SpecialOfferType.TwoForAmount, tomatoBox, 0.99);

            Receipt receipt = teller.ChecksOutArticlesFrom(cart);

            Assert.That(receipt.GetTotalPrice(), Is.EqualTo(0.99).Within(0.0001));
        }

        [TestCase]
        public void FiveForAmountDiscount()
        {
            Product toothpaste = new("Toothpaste", ProductUnit.Each);
            catalog.AddProduct(toothpaste, 1.79);

            ShoppingCart cart = new();
            cart.AddItemQuantity(toothpaste, 5);

            Teller teller = new(catalog);
            teller.AddSpecialOffer(SpecialOfferType.FiveForAmount, toothpaste, 7.49);

            Receipt receipt = teller.ChecksOutArticlesFrom(cart);
            Assert.That(receipt.GetTotalPrice(), Is.EqualTo(7.49).Within(0.0001));
        }

        [TestCase]
        public void AddItemMultipleTimes()
        {
            Product apple = new("Apple", ProductUnit.Kilo);
            catalog.AddProduct(apple, 1.99);
            ShoppingCart cart = new();
            cart.AddItemQuantity(apple, 1.0);
            cart.AddItemQuantity(apple, 2.0);

            ShoppingCart cart2 = new();
            cart2.AddItemQuantity(apple, 3.0);

            Teller teller = new(catalog);

            // Hack to check total amount of items
            Assert.That(teller.ChecksOutArticlesFrom(cart).GetTotalPrice(), Is.EqualTo(teller.ChecksOutArticlesFrom(cart2).GetTotalPrice()));
        }

        [TestCase]
        public void ComputeDiscountAmount()
        {
            Product apple = new("Apple", ProductUnit.Kilo);
            const double pricePerKgInEuro = 2.0;
            catalog.AddProduct(apple, pricePerKgInEuro);

            ShoppingCart cart = new();
            const double purchasedAmount = 3.0;
            cart.AddItemQuantity(apple, purchasedAmount);

            const double totalPriceNoDiscount = pricePerKgInEuro * purchasedAmount;
            const double discountPercentage = 10.0;

            Teller teller = new(catalog);
            teller.AddSpecialOffer(SpecialOfferType.PercentageDiscount, apple, discountPercentage);
            Receipt receipt = teller.ChecksOutArticlesFrom(cart);

            Assert.Multiple(() =>
            {
                Assert.That(receipt.GetDiscounts()[0].DiscountAmount, Is.EqualTo(-(1.0 / discountPercentage) * totalPriceNoDiscount).Within(0.0001), "Discount Amount is reduction in price from total amount");
                Assert.That(receipt.GetTotalPrice() + Math.Abs(receipt.GetDiscounts()[0].DiscountAmount), Is.EqualTo(totalPriceNoDiscount).Within(0.0001), "Final price + discount = Full Price without Discount");
            });
        }
    }

    [TestFixture]
    public class Supermarket_NegativeTests
    {
        [TestCase]
        public void NullItemToCatalogue()
        {
            FakeCatalog _catalogue = new();
            var ex = Assert.Throws<ArgumentNullException>(delegate { _catalogue.AddProduct(null, 1.5); });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null. (Parameter 'product')"));
        }

        [TestCase]
        public void NegativePriceToCatalogue()
        {
            FakeCatalog catalog = new();
            var ex = Assert.Throws<ArgumentOutOfRangeException>(delegate { catalog.AddProduct(new("Apple", ProductUnit.Kilo), -42.0); });

            Assert.That(ex.Message, Is.EqualTo("The price may not be negative. (Parameter 'price')"));
        }

        [TestCase]
        public void AddingMultipleOffersForSameItem()
        {
            FakeCatalog catalog = new();
            Product apple = new("Apple", ProductUnit.Kilo);
            catalog.AddProduct(apple, 3.00);
            ShoppingCart theCart = new ShoppingCart();
            theCart.AddItem(apple);
            Teller teller = new(catalog);
            teller.AddSpecialOffer(SpecialOfferType.PercentageDiscount, apple, 20);
            teller.AddSpecialOffer(SpecialOfferType.PercentageDiscount, apple, 40);

            Assert.That(teller.ChecksOutArticlesFrom(theCart).GetDiscounts()[0].DiscountAmount, Is.EqualTo(-1.2).Within(0.0001), "Only latest discount is active");
        }
    }
}