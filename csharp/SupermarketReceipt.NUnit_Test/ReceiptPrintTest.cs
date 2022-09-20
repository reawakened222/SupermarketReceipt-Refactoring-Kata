using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;

namespace SupermarketReceipt.NUnit_Test
{
    [TestFixture]
    public class ReceiptPrintTest
    {
        [TestCase]
        public void PrintEmptyReceipt()
        {
            FakeCatalog catalog = new FakeCatalog();
            Teller teller = new Teller(catalog);

            ShoppingCart cart = new ShoppingCart();
            Receipt receipt = teller.ChecksOutArticlesFrom(cart);
            ReceiptPrinter printer = new ReceiptPrinter();
            string res = printer.PrintReceipt(receipt);
            Assert.That(res.Trim(), Is.EqualTo("Total:                              0.00"));

            res = printer.PrintReceiptToHTML(receipt);
            Assert.That(res.Trim(), 
                Is.EqualTo(
                    "<tr><th>Item</th><th>Total Price</th><th>Price x Amount</th></tr>"+
                    "<tr><th>Description</th><th>Discount Amount</th></tr>"+
                    "<tr><td>Total:</td><td>0.00</td></tr>"));
        }
        [TestCase]
        public void PrintSingleItem()
        {
            FakeCatalog catalog = new FakeCatalog();
            Product apple = new Product("Apple", ProductUnit.Kilo);
            catalog.AddProduct(apple, 2.0);
            Teller teller = new Teller(catalog);

            ShoppingCart cart = new ShoppingCart();
            cart.AddItemQuantity(apple, 1.0);
            Receipt receipt = teller.ChecksOutArticlesFrom(cart);
            ReceiptPrinter printer = new ReceiptPrinter();
            string res = printer.PrintReceipt(receipt);
            Assert.That(res.Trim(), Is.EqualTo("Apple                               2.00\n\n" + "Total:                              2.00"));

            res = printer.PrintReceiptToHTML(receipt);
            Assert.That(res.Trim(), Is.EqualTo(
                    "<tr><th>Item</th><th>Total Price</th><th>Price x Amount</th></tr>" + 
                    "<tr><td>Apple</td><td>2.00</td></tr>" +
                    "<tr><th>Description</th><th>Discount Amount</th></tr>"+
                    "<tr><td>Total:</td><td>2.00</td></tr>"));
        }
        [TestCase]
        public void PrintSingleItemWithDiscount()
        {
            FakeCatalog catalog = new FakeCatalog();
            Product apple = new Product("Apple", ProductUnit.Kilo);
            catalog.AddProduct(apple, 2.0);
            Teller teller = new Teller(catalog);
            teller.AddSpecialOffer(SpecialOfferType.PercentageDiscount, apple, 10.0);

            ShoppingCart cart = new ShoppingCart();
            cart.AddItemQuantity(apple, 1.0);
            Receipt receipt = teller.ChecksOutArticlesFrom(cart);
            ReceiptPrinter printer = new ReceiptPrinter();
            string res = printer.PrintReceipt(receipt);
            Assert.That(res.Trim(), Is.EqualTo("Apple                               2.00\n10% off(Apple)                     -0.20\n\n" + "Total:                              1.80"));

            res = printer.PrintReceiptToHTML(receipt);
            Assert.That(res.Trim(), 
                Is.EqualTo(
                    "<tr><th>Item</th><th>Total Price</th><th>Price x Amount</th></tr>" +
                    "<tr><td>Apple</td><td>2.00</td></tr>" +
                    "<tr><th>Description</th><th>Discount Amount</th></tr>" +
                    "<tr><td>10% off(Apple)</td><td>-0.20</td></tr>" +
                    "<tr><td>Total:</td><td>1.80</td></tr>"));
        }
    }
}
