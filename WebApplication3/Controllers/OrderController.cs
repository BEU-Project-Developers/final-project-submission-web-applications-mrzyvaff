using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class OrderController : Controller
    {
        private readonly bool IsAdmin = true;

        public IActionResult Index()
        {
            var orders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    OrderDate = DateTime.Now.AddDays(-1),
                    TotalAmount = 45.50m,
                    Status = "Completed",
                    DeliveryAddress = "123 Main St, City",
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { Product = new Product { Name = "Espresso" }, Quantity = 2, UnitPrice = 3.50m },
                        new OrderItem { Product = new Product { Name = "Croissant" }, Quantity = 1, UnitPrice = 2.75m }
                    }
                },
                new Order
                {
                    Id = 2,
                    OrderDate = DateTime.Now.AddDays(-2),
                    TotalAmount = 32.75m,
                    Status = "Processing",
                    DeliveryAddress = "456 Oak Ave, Town",
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { Product = new Product { Name = "Cappuccino" }, Quantity = 1, UnitPrice = 4.25m },
                        new OrderItem { Product = new Product { Name = "Muffin" }, Quantity = 2, UnitPrice = 3.25m }
                    }
                },
                new Order
                {
                    Id = 3,
                    OrderDate = DateTime.Now.AddDays(-3),
                    TotalAmount = 28.90m,
                    Status = "Completed",
                    DeliveryAddress = "789 Pine St, Village",
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { Product = new Product { Name = "Latte" }, Quantity = 2, UnitPrice = 4.50m }
                    }
                }
            };

            ViewBag.IsAdmin = IsAdmin;
            return View(orders);
        }

        public IActionResult Details(int? id)
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Order order)
        {
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Admin()
        {
            if (!IsAdmin)
            {
                return RedirectToAction("Index", "Home");
            }

            var orders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    OrderDate = DateTime.Now.AddDays(-1),
                    TotalAmount = 45.50m,
                    Status = "Completed",
                    DeliveryAddress = "123 Main St, City",
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { Product = new Product { Name = "Espresso" }, Quantity = 2, UnitPrice = 3.50m },
                        new OrderItem { Product = new Product { Name = "Croissant" }, Quantity = 1, UnitPrice = 2.75m }
                    }
                },
                new Order
                {
                    Id = 2,
                    OrderDate = DateTime.Now.AddDays(-2),
                    TotalAmount = 32.75m,
                    Status = "Processing",
                    DeliveryAddress = "456 Oak Ave, Town",
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { Product = new Product { Name = "Cappuccino" }, Quantity = 1, UnitPrice = 4.25m },
                        new OrderItem { Product = new Product { Name = "Muffin" }, Quantity = 2, UnitPrice = 3.25m }
                    }
                },
                new Order
                {
                    Id = 3,
                    OrderDate = DateTime.Now.AddDays(-3),
                    TotalAmount = 28.90m,
                    Status = "Completed",
                    DeliveryAddress = "789 Pine St, Village",
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { Product = new Product { Name = "Latte" }, Quantity = 2, UnitPrice = 4.50m }
                    }
                }
            };

            ViewBag.IsAdmin = IsAdmin;
            return View(orders);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            return RedirectToAction(nameof(Admin));
        }
    }
} 