namespace VirtualStore.Models
{
    public class FakeDatabase
    {
        public static List<Store> Stores { get; set; } = new List<Store>
        {
            new Store
            {
                Id = 1,
                Name = "Main Store",
                Address = "Al Gammal St - Cairo - Egypt",
                IsMain = true,
                IsInvoiceDirect = true,
                Spaces = new List<Space>
                {
                    new Space
                    {
                        Id = 1,
                        Name = "Default Space",
                        Products = new List<Product>
                        {
                            new Product { Id = 1, Name = "Mobile Samsung", Count = 10 },
                            new Product { Id = 2, Name = "Fan", Count = 15 },
                            new Product { Id = 3, Name = "Washing Machine", Count = 3 },
                        }
                    }
                }
            },
            new Store
            {
                Id = 2,
                Name = "Electric Store",
                Address = "Ain Shams St - Cairo - Egypt",
                IsMain = false,
                IsInvoiceDirect = true,
                Spaces = new List<Space>
                {
                    new Space
                    {
                        Id = 2,
                        Name = "Default Space",
                        Products = new List<Product>
                        {
                            new Product { Id = 4, Name = "TV", Count = 5 },
                            new Product { Id = 5, Name = "Lamp", Count = 25 },
                        }
                    }
                }
            },
            new Store
            {
                Id = 3,
                Name = "Electric Store 2",
                Address = "Shams St - Cairo - Egypt",
                IsMain = false,
                IsInvoiceDirect = true,
                Spaces = new List<Space>
                {
                    new Space
                    {
                        Id = 2,
                        Name = "Default Space",
                        Products = new List<Product>
                        {
                            new Product { Id = 4, Name = "TV", Count = 5 },
                            new Product { Id = 5, Name = "Lamp", Count = 25 },
                        }
                    }
                }
            },
            new Store
            {
                Id = 4,
                Name = "Electric Store 3",
                Address = "Alexandria - Egypt",
                IsMain = false,
                IsInvoiceDirect = true,
                Spaces = new List<Space>
                {
                    new Space
                    {
                        Id = 2,
                        Name = "Default Space",
                        Products = new List<Product>
                        {
                            new Product { Id = 4, Name = "TV", Count = 5 },
                            new Product { Id = 5, Name = "Lamp", Count = 25 },
                        }
                    }
                }
            }
        };
    
}
}
