using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.Marshalling;

namespace Console6._2
{

    public class StoreContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Discount> Discounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Store.db");
        }
    }

    public enum Category
    {
        Electronics,
        HomeAppliances,
        Clothing,
        Sports,
        Food,
        Stationery,
        Cosmetics
    }


    [Table(nameof(StoreContext.Products))]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; } = 0.0;
        public string Manufacturer { get; set; } = string.Empty;
        public string SiteUrl { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public Category Category { get; set; }
        public List<Discount> Discounts { get; set; } = new();
    }

    [Table(nameof(StoreContext.Discounts))]
    public class Discount
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Percentage { get; set; }
        public List<Product> Products { get; set; } = new();
    }


    public class StoreDbHandler
    {
        public StoreContext Context { get; private set; }


        public StoreDbHandler(StoreContext db, bool replace = false)
        {
            Context = db;
            if (replace)
            {
                db.Database.EnsureDeleted();
            }

            db.Database.EnsureCreated();
            SetDummyData();
        }

        private void SetDummyData()
        {
            if (Context.Products.Count() == 0)
            {
                Debug.WriteLine("DB is empty, lets add some data for testing.\r\n");
                Debug.WriteLine("Add data process started...\r\n");

                var discounts = new List<Discount>
            {
                new Discount { Name = "Summer Sale", Percentage = 10 },
                new Discount { Name = "Winter Sale", Percentage = 15 },
                new Discount { Name = "Black Friday", Percentage = 50 },
                new Discount { Name = "Cyber Monday", Percentage = 40 },
                new Discount { Name = "New Year Sale", Percentage = 20 }
            };

                var products = new List<Product>
            {
                // Electronics
                new Product()
                {
                    Name = "Laptop",
                    Price = 1000,
                    Manufacturer = "Dell",
                    Category = Category.Electronics,
                    SiteUrl = "https://dell.com",
                    Country = "USA",
                    Discounts = new List<Discount>() { discounts[0] }
                },

                new Product { Name = "Smartphone", Price = 700, Manufacturer = "Samsung", Category = Category.Electronics, SiteUrl = "https://samsung.com", Country = "South Korea", Discounts = { discounts[0], discounts[1] } },
 
                // Home appliances
                new Product { Name = "Vacuum Cleaner", Price = 150, Manufacturer = "Dyson", Category = Category.HomeAppliances, SiteUrl = "https://dyson.com", Country = "UK", Discounts = { discounts[1] } },
                new Product { Name = "Microwave", Price = 120, Manufacturer = "Panasonic", Category = Category.HomeAppliances, SiteUrl = "https://panasonic.com", Country = "Japan", Discounts = { discounts[0], discounts[1] } },
 
                // Clothing
                new Product { Name = "Jeans", Price = 50, Manufacturer = "Levi's", Category = Category.Clothing, SiteUrl = "https://levis.com", Country = "USA", Discounts = { discounts[4] } },
                new Product { Name = "Jacket", Price = 100, Manufacturer = "North Face", Category = Category.Clothing, SiteUrl = "https://northface.com", Country = "USA", Discounts = { discounts[2] } },
                new Product { Name = "T-Shirt", Price = 30, Manufacturer = "Lagrand", Category = Category.Clothing, SiteUrl = "https://lagrand.com.ua", Country = "Ukraine", Discounts = { discounts[2] } },
                // Sports
                new Product { Name = "Basketball", Price = 30, Manufacturer = "Spalding", Category = Category.Sports, SiteUrl = "https://spalding.com", Country = "USA", Discounts = { discounts[1], discounts[2], discounts[3], discounts[4] } },
 
                // Food
                new Product { Name = "Chocolate", Price = 5, Manufacturer = "Lindt", Category = Category.Food, SiteUrl = "https://lindt.com", Country = "Switzerland", Discounts = { discounts[0], discounts[4] } },
                new Product { Name = "Pasta", Price = 2, Manufacturer = "Barilla", Category = Category.Food, SiteUrl = "https://barilla.com", Country = "Italy", Discounts = { discounts[4] } },
                new Product { Name = "Bread", Price = 1.5, Manufacturer = "Home bread", Category = Category.Food, SiteUrl = "https://ekoproduct.ua", Country = "Ukraine", Discounts = { discounts[3] } },
                // Stationery
                new Product { Name = "Notebook", Price = 3, Manufacturer = "Moleskine", Category = Category.Stationery, SiteUrl = "https://moleskine.com", Country = "Italy", Discounts = { discounts[1] } },
 
                // Cosmetics
                new Product
                {
                    Name = "Shampoo",
                    Price = 15,
                    Manufacturer = "Pantene",
                    Category = Category.Cosmetics,
                    SiteUrl = "https://pantene.com",
                    Country = "USA",
                    Discounts = new()
                }
            };

                Context.Products.AddRange(products);
                Context.SaveChanges();

                Debug.WriteLine("Add data process completed...\r\n");
            }
            else
            {
                Debug.WriteLine("DB already filled by data.\r\n");
            }

            Debug.WriteLine("Show actual DB dummy data.\r\n");

            List<Discount> allDiscounts = GetActualDiscountsList();
            List<Product> allProducts = GetActualProductsList();

            for (int i = 0; i < allProducts.Count; i++)
            {
                Debug.WriteLine($"{i + 1}) Category: {allProducts[i].Category}, Product: {allProducts[i].Name}, Price: {allProducts[i].Price}, " +
                    $"Discounts: {string.Join(", ", allProducts[i].Discounts.Select(pd => pd.Name).ToList())}");
            }
        }

        public List<Product> GetActualProductsList()
        {
            //return Context.Products.Include(p => p.Discounts).ToList();
            return Context.Products.ToList();
        }

        public List<Discount> GetActualDiscountsList()
        {
            //return Context.Discounts.Include(p => p.Products).ToList();
            return Context.Discounts.ToList();
        }
    }



    internal class Program
    {
        static void Main(string[] args)
        {
            using var storeContext = new StoreContext();
            var dbHandler = new StoreDbHandler(storeContext, true);

            dbHandler.Context.Products.Load();
            dbHandler.Context.Discounts.Load();

            List<Product> allProducts = dbHandler.Context.Products.ToList();
            List<Discount> allDiscounts = dbHandler.Context.Discounts.ToList();

            // 1) FromSqlRow(SELECT only)
            // - Query only-> ("Query");

            // 2) ExecuteSqlRow(all other requests).
            // Via:
            // -Query only-> ("Query");
            // -Format + Params-> ($"Query {1} AND {2}", par1, par2)

            // 3) FromSqlInterpolated
            // - FormattableString-> (FormattableString query = $"Bla bla bla {param1} AND Price < {param2}";)



            // I can use {var} instead of {List<>}

            int Price = 60;
            Category categ = Category.Clothing;
           


            List<Product> zavd1 =dbHandler.Context.Products.FromSqlRaw("SELECT * FROM Products WHERE Price < {0} AND Category = {1}", Price, categ).ToList();


            String LIKES = "J%";

            List<Product> zavd2 = dbHandler.Context.Products.FromSqlRaw("SELECT * FROM Products WHERE LOWER(Name) LIKE LOWER({0})", LIKES).ToList();
    

            
            List<Product> zavd3 = dbHandler.Context.Products.FromSqlRaw("SELECT * FROM Products ORDER BY Price DESC LIMIT 3").ToList();


            int ddiscount = 20;

            List<Discount> zavd4 = dbHandler.Context.Discounts.FromSqlRaw("SELECT * FROM Discounts WHERE Percentage >= {0}", ddiscount).ToList();


            var zavd5 = dbHandler.Context.Database.ExecuteSqlRaw("INSERT INTO Products (Name, Price, Manufacturer, SiteUrl, Country, Category) " +
                "VALUES ('Scooter', 2300, 'Xiaomi', 'https', 'China',{0})", (int)Category.Electronics);

            string SeachAboutCountry = "USA";

            var zavd6 = dbHandler.Context.Database.ExecuteSqlRaw("UPDATE Products SET Price = Price + 5 WHERE Country = {0}", SeachAboutCountry);

            dbHandler.Context.SaveChanges();

            dbHandler.Context.ChangeTracker.Clear(); // Only for UPDATE

            int smolePrice = 3;

            var zavd7 = dbHandler.Context.Database.ExecuteSqlRaw("DELETE FROM Products WHERE Price < {0}", smolePrice);














            //SELECT

            List <Product> result1 = dbHandler.Context.Products.FromSqlRaw("SELECT * FROM Products WHERE Price > 100 ").ToList();

            string country = "Italy";
            int price = 90;

            var result2 = dbHandler.Context.Products.FromSqlRaw("SELECT * FROM Products WHERE country  = {0} AND Price < {1} ", country, price).ToList();

            //То же самое что и 2 только более приятный синтаксис
            FormattableString formattableString = ($"SELECT * FROM Products WHERE country  = {country} AND Price < {price} ") ;
            var result3 = dbHandler.Context.Products.FromSqlInterpolated(formattableString).ToList();


            //DELETE
            // Небезопасно
           var result4 = dbHandler.Context.Database.ExecuteSqlRaw("DELETE FROM Products WHERE Id = 5");

            // Insert
            Category category = Category.Electronics;
            string categoryName = category.ToString();

            var result6 = dbHandler.Context.Database.ExecuteSqlRaw("INSERT INTO Products (Name, Price, Manufacturer, SiteUrl, Country, Category) " +
                "VALUES ('Test', 333, 'Manufacturer', 'https', 'Ukraine', {0})", (int)category);

            // Update
            var result7 = dbHandler.Context.Database.ExecuteSqlRaw("UPDATE Products SET Price = 10 WHERE Id = 6");

            dbHandler.Context.SaveChanges(); // Save

            dbHandler.Context.ChangeTracker.Clear(); // Clearing чистим трекер тольк одля undate

            allProducts = dbHandler.Context.Products.ToList();
            allDiscounts = dbHandler.Context.Discounts.ToList();



            Console.ReadLine(); 
        }
    }
}
