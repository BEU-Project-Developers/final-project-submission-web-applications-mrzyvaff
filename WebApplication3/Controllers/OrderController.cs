using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using System;
using System.Collections.Generic; // Added for List<T>
using Microsoft.Extensions.Logging; // Added for logging

namespace WebApplication3.Controllers
{
    public class OrderController : Controller
    {
        // In a real application, IsAdmin would be determined by user roles (e.g., using [Authorize(Roles = "Admin")])
        // For this example, we'll keep it as a placeholder.
        private readonly bool IsAdmin = true;
        private readonly ILogger<OrderController> _logger; // Added for logging

        public OrderController(ILogger<OrderController> logger) // Injected ILogger
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
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
                _logger.LogInformation("Orders Index page accessed.");
                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching orders for Index page.");
                // You might want to show a more user-friendly error page or message here
                TempData["ErrorMessage"] = "An error occurred while loading orders. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    _logger.LogWarning("Details action called with no order ID provided.");
                    return NotFound(); // Or Redirect to an error page / Index
                }
                // In a real application, you would fetch order details from a database here
                _logger.LogInformation($"Details for order ID {id} accessed.");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while trying to get details for order ID {id}.");
                TempData["ErrorMessage"] = "An error occurred while loading order details. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            try
            {
                _logger.LogInformation("Create order page accessed.");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while trying to access Create order page.");
                TempData["ErrorMessage"] = "An error occurred while preparing to create an order. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public IActionResult Create(Order order)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // In a real application, you would save the new order to a database here
                    _logger.LogInformation($"New order created successfully (placeholder). Order ID: {order.Id}");
                    TempData["SuccessMessage"] = "Order created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                _logger.LogWarning("Create order failed due to invalid model state.");
                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during order creation.");
                TempData["ErrorMessage"] = "An error occurred while creating the order. Please try again.";
                return View(order);
            }
        }

        public IActionResult Admin()
        {
            try
            {
                if (!IsAdmin)
                {
                    _logger.LogWarning("Unauthorized access attempt to Admin order page.");
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
                    }
                };

                ViewBag.IsAdmin = IsAdmin;
                _logger.LogInformation("Admin Orders page accessed.");
                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching orders for Admin page.");
                TempData["ErrorMessage"] = "An error occurred while loading admin orders. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            try
            {
                if (!IsAdmin)
                {
                    _logger.LogWarning($"Unauthorized attempt to update status for order ID {id}.");
                    TempData["ErrorMessage"] = "You do not have permission to perform this action.";
                    return RedirectToAction("Index", "Home");
                }

                // In a real application, you would fetch the order from the database and update its status
                _logger.LogInformation($"Order ID {id} status updated to '{status}' (placeholder).");
                TempData["SuccessMessage"] = $"Order {id} status updated to {status}.";
                return RedirectToAction(nameof(Admin));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating status for order ID {id} to '{status}'.");
                TempData["ErrorMessage"] = "An error occurred while updating the order status. Please try again.";
                return RedirectToAction(nameof(Admin));
            }
        }
    }
}