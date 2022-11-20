using System;
using System.Globalization;
using System.Runtime.Serialization;

public abstract class Product
{
    public string Name { get; private set; }

    public string Brand { get; private set; }

    public double Price { get; private set; }

    public string Color { get; private set; }

    public abstract string Size { get; protected set; }

    public Product(string name, string brand, double price, string color)
    {
        Name = name;
        Brand = brand;
        Price = price;
        Color = color;
    }
}
class Shirt : Product
{
    private string _size;

    public override string Size
    {
        get { return _size; }

        protected set
        {
            switch (value)
            {
                case "XS":
                case "S":
                case "M":
                case "L":
                case "XL":
                case "2XL":
                    _size = value;
                    break;
                default:
                    throw new ArgumentException("Invalid size!");
            }
        }
    }

    public Shirt(string name, string brand, double price, string color, string size)
        : base(name, brand, price, color)
    {
        Size = size;
    }
}

class Shoes : Product
{
    private string _size;

    public override string Size
    {
        get { return _size; }

        protected set
        {
            int convertedValue = Int16.Parse(value);

            if (convertedValue >= 39 && convertedValue <= 46)
            {
                _size = value;
            }
            else
            {
                throw new ArgumentException("Invalid size!");
            }
        }
    }

    public Shoes(string name, string brand, double price, string color, string size)
        : base(name, brand, price, color)
    {
        Size = size;
    }
}

class OtherProduct : Product
{
    private string _size;

    public override string Size
    {
        get { return _size; }

        protected set
        {
            int convertedValue = Int16.Parse(value);

            if (convertedValue >= 42 && convertedValue <= 66 && convertedValue % 2 == 0)
            {
                _size = value;
            }
            else
            {
                throw new ArgumentException("Invalid size!");
            }

        }
    }
    public OtherProduct(string name, string brand, double price, string color, string size)
        : base(name, brand, price, color)
    {
        Size = size;
    }
}


class Cart
{
    private List<Product> _products;

    public Cart()
    {
        _products = new List<Product>();
    }

    public List<Product> Products
    {
        get
        {
            if (_products == null)
            {
                return new List<Product>();
            }

            return _products;
        }

        set
        {
            _products = value;
        }
    }

    public void AddToCart(Product product)
    {
        Products.Add(product);
    }
}

/**
 * Purchase class
 */
public static class Purchase
{
    public static int CalculateDiscountPercentage(Product product, string purchasedDatetime, int count)
    {
        int discountPercentage = 0;

        if (IsTuesday(purchasedDatetime))
        {
            if (product.GetType().ToString() == "Shirt")
            {
                discountPercentage = 10;
            }

            if (product.GetType().ToString() == "Shoes")
            {
                discountPercentage = 25;
            }
        }

        if (count >= 3)
        {
            if (discountPercentage < 20)
            {
                discountPercentage = 20;
            }
        }

        return discountPercentage;
    }

    public static bool IsTuesday(string datetime)
    {
        DateTime purchasedDatetime;
        DateTime.TryParseExact(datetime, Cashier.DATETIME_FORMAT, null, DateTimeStyles.None, out purchasedDatetime);

        return purchasedDatetime.DayOfWeek == DayOfWeek.Tuesday;
    }
}

/**
 * Cashier class
 */
class Cashier
{
    public const string DATETIME_FORMAT = "yyyy-MM-dd H:mm:ss";
    private DateTime _purchasedDatetime;
    public Cart CartInstance { get; private set; }

    public string PurchasedDatetime
    {
        get { return _purchasedDatetime.ToString(DATETIME_FORMAT); }
    }

    public Cashier(Cart cart, string? purchasedDatetime = null)
    {
        CartInstance = cart;
        _purchasedDatetime = DateTime.Now;

        if (purchasedDatetime != null)
        {
            DateTime.TryParseExact(purchasedDatetime, DATETIME_FORMAT, null, DateTimeStyles.None, out _purchasedDatetime);
        }
    }

    public void printReceipt()
    {
        int index = 1;
        int discountPercentage = 0;
        double discountAmount = 0;
        double subTotal = 0;
        double discountTotal = 0;

        Console.WriteLine("Date: " + PurchasedDatetime);
        Console.WriteLine("--- Products ---");
        Console.WriteLine();
        
        foreach (Product product in CartInstance.Products)
        {
            Console.WriteLine(product.Name + " - " + product.Brand);
            Console.WriteLine("$" + product.Price);

            discountPercentage = Purchase.CalculateDiscountPercentage(product, PurchasedDatetime, CartInstance.Products.Count);
            discountAmount = Math.Round((product.Price * discountPercentage / 100), 2);
            subTotal += product.Price;
            discountTotal += discountAmount;

            if (discountPercentage > 0)
            {
                Console.WriteLine("#discount: " + discountPercentage + "% -$" + String.Format("{0:0.00}", discountAmount));
            }

            if (index != CartInstance.Products.Count)
            {
                Console.WriteLine();
                index++;
            }
        }

        Console.WriteLine("------------------------------------------------------------");
        Console.WriteLine("SUBTOTAL: $" + subTotal);
        Console.WriteLine("DISCOUNT: " + (discountTotal > 0 ? "-$" : "") + String.Format("{0:0.00}", discountTotal));
        Console.WriteLine("TOTAL: $" + (subTotal - discountTotal));
        Console.WriteLine();
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Example 1");
        Console.WriteLine();

        Product shirt1 = new Shirt("Blue Cotton Shirt", "BrandS", 14.99, "blue", "M");
        Product shirt2 = new Shirt("White Cotton Shirt", "BrandS", 15.99, "white", "M");
        Product trousers1 = new OtherProduct("Black Cotton Trousers", "BrandT", 29.99, "black", "50");
        Product shoes1 = new Shoes("Black Leather Shoes", "BrandS", 59.99, "black", "43");
        Product jacket1 = new OtherProduct("Black Cotton Suit Jacket", "BrandJ", 99.99, "black", "50");

        Cart cart1 = new Cart();
        cart1.AddToCart(shirt1);
        cart1.AddToCart(shirt2);
        cart1.AddToCart(trousers1);
        cart1.AddToCart(shoes1);
        cart1.AddToCart(jacket1);

        Cashier receipt1 = new Cashier(cart1);

        receipt1.printReceipt();


        /* Example 2 - Shirt discount on Tuesday */

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("= = = = = = = = = = = = =");
        Console.WriteLine();
        Console.WriteLine("Example 2");
        Console.WriteLine();

        Product shirt2_1 = new Shirt("Black Silk Shirt", "BrandS", 29.99, "black", "L");
        Product shirt2_2 = new Shirt("White Silk Shirt", "BrandS", 29.99, "white", "L");

        Cart cart2_1 = new Cart();
        cart2_1.AddToCart(shirt2_1);
        cart2_1.AddToCart(shirt2_2);

        Cashier receipt2_1 = new Cashier(cart2_1, "2022-11-15 07:00:00");

        receipt2_1.printReceipt();


        /* Example 3 - Shoes and on count */

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("= = = = = = = = = = = = =");
        Console.WriteLine();
        Console.WriteLine("Example 3");
        Console.WriteLine();

        Product trousers3_1 = new OtherProduct("Red Linen Trousers", "BrandT", 49.99, "red", "56");
        Product shoes3_1 = new Shoes("Red Suede Shoes", "BrandS", 59.99, "red", "44");
        Product shoes3_2 = new Shoes("Black Suede Shoes", "BrandS", 59.99, "black", "44");
        Product jacket3_1 = new OtherProduct("Red Linen Suit Jacket", "BrandJ", 99.99, "red", "56");
        Product shirt3_1 = new Shirt("White Linen Shirt", "BrandS", 29.99, "white", "L");


        Cart cart3_1 = new Cart();
        cart3_1.AddToCart(trousers3_1);
        cart3_1.AddToCart(shoes3_1);
        cart3_1.AddToCart(shoes3_2);
        cart3_1.AddToCart(jacket3_1);
        cart3_1.AddToCart(shirt3_1);

        Cashier receipt3_1 = new Cashier(cart3_1, "2022-11-15 07:00:00");

        receipt3_1.printReceipt();


        /* Example 4 - without discount */

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("= = = = = = = = = = = = =");
        Console.WriteLine();
        Console.WriteLine("Example 4");
        Console.WriteLine();

        Product trousers4_1 = new OtherProduct("Black Cotton Trousers", "BrandT", 29.99, "black", "42");
        Product jacket4_1 = new OtherProduct("Black Cotton Suit Jacket", "BrandJ", 99.99, "black", "50");

        Cart cart4_1 = new Cart();
        cart4_1.AddToCart(trousers4_1);
        cart4_1.AddToCart(jacket4_1);

        Cashier receipt4_1 = new Cashier(cart4_1);

        receipt4_1.printReceipt();
    }
}
