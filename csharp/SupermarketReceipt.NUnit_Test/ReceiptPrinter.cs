using System.Globalization;
using System.Text;

namespace SupermarketReceipt
{
    public class ReceiptPrinter
    {
        private static readonly CultureInfo Culture = CultureInfo.CreateSpecificCulture("en-GB");

        private readonly int _columns;


        public ReceiptPrinter(int columns)
        {
            _columns = columns;
        }

        public ReceiptPrinter() : this(40)
        {
        }
        public delegate string ReceiptItemPrinter(ReceiptItem item);
        public delegate string DiscountItemPrinter(Discount item);
        public delegate string TotalItemPrinter(Receipt item);
        public delegate string HeaderPrinter();
        public string PrintReceipt(Receipt receipt)
        {
            return this.PrintReceipt(receipt, PrintReceiptItem, PrintDiscount, PrintTotal, () => "", () => "");
        }

        public string PrintReceiptToHTML(Receipt receipt)
        {
            HeaderPrinter receiptHeader = () => $"<tr><th>Item</th><th>Total Price</th><th>Price x Amount</th></tr>";
            HeaderPrinter discountHeader = () => $"<tr><th>Description</th><th>Discount Amount</th></tr>";
            return this.PrintReceipt(receipt, PrintReceiptItemAsHTML, PrintDiscountAsHTML, PrintTotalAsHTML, receiptHeader, discountHeader).ReplaceLineEndings("");
        }
        public string PrintReceipt(Receipt receipt, ReceiptItemPrinter receiptItemPrinter, DiscountItemPrinter discountItemPrinter, TotalItemPrinter totalPrinter, HeaderPrinter PrintReceiptHeader, HeaderPrinter PrintDiscountHeader)
        {
            var result = new StringBuilder();
            string receiptHeader = PrintReceiptHeader();
            result.Append(receiptHeader);
            foreach (var item in receipt.GetItems())
            {
                string receiptItem = receiptItemPrinter(item);
                result.Append(receiptItem);                
            }

            string discountHeader = PrintDiscountHeader();
            result.Append(discountHeader);
            foreach (var discount in receipt.GetDiscounts())
            {
                string discountPresentation = discountItemPrinter(discount);
                result.Append(discountPresentation);
            }

            {
                result.Append("\n");
                result.Append(totalPrinter(receipt));
            }
            return result.ToString();
        }

        private string PrintTotal(Receipt receipt)
        {
            string name = "Total:";
            string value = PrintPrice(receipt.GetTotalPrice());
            return FormatLineWithWhitespace(name, value);
        }

        private string PrintTotalAsHTML(Receipt receipt)
        {
            string name = "Total:";
            string value = PrintPrice(receipt.GetTotalPrice());
            string line = $"<tr><td>{name}</td><td>{value}</td></tr>";
            return line;
        }

        private string PrintDiscount(Discount discount)
        {
            string name = discount.Description + "(" + discount.Product.Name + ")";
            string value = PrintPrice(discount.DiscountAmount);

            return FormatLineWithWhitespace(name, value);
        }

        private string PrintDiscountAsHTML(Discount discount)
        {
            string name = discount.Description + "(" + discount.Product.Name + ")";
            string value = PrintPrice(discount.DiscountAmount);

            string line = $"<tr><td>{name}</td><td>{value}</td></tr>";
            return line;
        }

        private string PrintReceiptItem(ReceiptItem item)
        {
            string totalPrice = PrintPrice(item.TotalPrice);
            string name = item.Product.Name;
            string line = FormatLineWithWhitespace(name, totalPrice);
            if (item.Quantity != 1)
            {
                line += "  " + PrintPrice(item.Price) + " * " + PrintQuantity(item) + "\n";
            }

            return line;
        }
        private string PrintReceiptItemAsHTML(ReceiptItem item)
        {
            string totalPrice = PrintPrice(item.TotalPrice);
            string name = item.Product.Name;

            string line = $"<tr><td>{name}</td><td>{totalPrice}</td>";
            if (item.Quantity != 1)
            {
                line += "<td>" + PrintPrice(item.Price) + " * " + PrintQuantity(item) + "</td>";
            }
            line += "</tr>";
            return line;
        }


        private string FormatLineWithWhitespace(string name, string value)
        {
            var line = new StringBuilder();
            line.Append(name);
            int whitespaceSize = this._columns - name.Length - value.Length;
            for (int i = 0; i < whitespaceSize; i++) {
                line.Append(" ");
            }
            line.Append(value);
            line.Append('\n');
            return line.ToString();
        }

        private string PrintPrice(double price)
        {
            return price.ToString("N2", Culture);
        }

        private static string PrintQuantity(ReceiptItem item)
        {
            return ProductUnit.Each == item.Product.Unit
                ? ((int) item.Quantity).ToString()
                : item.Quantity.ToString("N3", Culture);
        }
        
    }
}