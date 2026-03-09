using BinDaddy.Backend.Models;

namespace BinDaddy.Backend.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Check if database is already seeded
            if (context.Categories.Any())
            {
                return;
            }

            // Create categories
            var categories = new Category[]
            {
                new Category { Name = "Residential Bins", Description = "Small bins for residential use" },
                new Category { Name = "Commercial Bins", Description = "Large bins for commercial use" },
                new Category { Name = "Construction Dumpsters", Description = "Heavy-duty dumpsters for construction" },
                new Category { Name = "Recycling Bins", Description = "Specialized recycling containers" },
                new Category { Name = "Specialty Containers", Description = "Custom containers for special purposes" }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();

            // Create products
            var products = new Product[]
            {
                new Product { Name = "20 Yard Residential Bin", Description = "Perfect for home cleanouts", Price = 299.99m, CategoryId = 1, Stock = 50 },
                new Product { Name = "10 Yard Residential Bin", Description = "Ideal for small projects", Price = 199.99m, CategoryId = 1, Stock = 75 },
                new Product { Name = "40 Yard Commercial Dumpster", Description = "Large capacity for businesses", Price = 499.99m, CategoryId = 2, Stock = 30 },
                new Product { Name = "30 Yard Commercial Dumpster", Description = "Medium commercial use", Price = 399.99m, CategoryId = 2, Stock = 40 },
                new Product { Name = "Construction Debris Box", Description = "Heavy-duty construction waste", Price = 349.99m, CategoryId = 3, Stock = 25 },
                new Product { Name = "Roll-Off Dumpster 20ft", Description = "Standard roll-off container", Price = 449.99m, CategoryId = 3, Stock = 20 },
                new Product { Name = "Recycling Bin Set", Description = "3-bin recycling system", Price = 149.99m, CategoryId = 4, Stock = 100 },
                new Product { Name = "Cardboard Recycling Bin", Description = "Large cardboard collection bin", Price = 99.99m, CategoryId = 4, Stock = 60 },
                new Product { Name = "Yard Waste Bin", Description = "For leaves and grass clippings", Price = 129.99m, CategoryId = 5, Stock = 45 },
                new Product { Name = "Hazardous Waste Container", Description = "Safe hazardous material storage", Price = 599.99m, CategoryId = 5, Stock = 15 }
            };
            context.Products.AddRange(products);
            context.SaveChanges();

            // Create users
            var users = new User[]
            {
                new User { Email = "john@example.com", FirstName = "John", LastName = "Doe", Phone = "555-0001", Address = "123 Main St" },
                new User { Email = "jane@example.com", FirstName = "Jane", LastName = "Smith", Phone = "555-0002", Address = "456 Oak Ave" },
                new User { Email = "bob@example.com", FirstName = "Bob", LastName = "Johnson", Phone = "555-0003", Address = "789 Pine Rd" },
                new User { Email = "alice@example.com", FirstName = "Alice", LastName = "Williams", Phone = "555-0004", Address = "321 Elm St" },
                new User { Email = "charlie@example.com", FirstName = "Charlie", LastName = "Brown", Phone = "555-0005", Address = "654 Maple Dr" }
            };
            context.Users.AddRange(users);
            context.SaveChanges();

            // Create carts for users
            foreach (var user in users)
            {
                var cart = new Cart { UserId = user.Id };
                context.Carts.Add(cart);
            }
            context.SaveChanges();

            // Create sample orders
            var orders = new Order[]
            {
                new Order { UserId = users[0].Id, TotalAmount = 299.99m, Status = "Completed", OrderDate = DateTime.UtcNow.AddDays(-10) },
                new Order { UserId = users[1].Id, TotalAmount = 699.98m, Status = "Completed", OrderDate = DateTime.UtcNow.AddDays(-5) },
                new Order { UserId = users[2].Id, TotalAmount = 449.99m, Status = "Pending", OrderDate = DateTime.UtcNow.AddDays(-2) },
                new Order { UserId = users[3].Id, TotalAmount = 1099.97m, Status = "Completed", OrderDate = DateTime.UtcNow.AddDays(-15) },
                new Order { UserId = users[4].Id, TotalAmount = 349.99m, Status = "Processing", OrderDate = DateTime.UtcNow.AddDays(-1) }
            };
            context.Orders.AddRange(orders);
            context.SaveChanges();

            // Create order items
            var orderItems = new OrderItem[]
            {
                new OrderItem { OrderId = orders[0].Id, ProductId = products[0].Id, Quantity = 1, Price = 299.99m },
                new OrderItem { OrderId = orders[1].Id, ProductId = products[1].Id, Quantity = 1, Price = 199.99m },
                new OrderItem { OrderId = orders[1].Id, ProductId = products[4].Id, Quantity = 1, Price = 349.99m },
                new OrderItem { OrderId = orders[2].Id, ProductId = products[2].Id, Quantity = 1, Price = 449.99m },
                new OrderItem { OrderId = orders[3].Id, ProductId = products[3].Id, Quantity = 1, Price = 399.99m },
                new OrderItem { OrderId = orders[3].Id, ProductId = products[5].Id, Quantity = 1, Price = 449.99m },
                new OrderItem { OrderId = orders[3].Id, ProductId = products[6].Id, Quantity = 1, Price = 149.99m },
                new OrderItem { OrderId = orders[4].Id, ProductId = products[8].Id, Quantity = 1, Price = 129.99m }
            };
            context.OrderItems.AddRange(orderItems);
            context.SaveChanges();
        }
    }
}
