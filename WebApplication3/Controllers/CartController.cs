using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Data;
using WebApplication3.Models;
using Microsoft.Extensions.Logging; // Added for logging

namespace WebApplication3.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CartController> _logger; // Added for logging

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<CartController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger; // Assigned logger
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Attempted to access cart without a logged-in user.");
                    TempData["ErrorMessage"] = "You need to be logged in to view your cart.";
                    return RedirectToAction("Login", "Account");
                }

                var cartItems = await _context.CartItems
                    .Include(c => c.Product)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                var model = new CartViewModel
                {
                    CartItems = cartItems,
                    TotalAmount = cartItems.Sum(c => c.Quantity * c.Product.Price)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart items for user.");
                TempData["ErrorMessage"] = "An error occurred while loading your cart. Please try again.";
                return RedirectToAction("Index", "Home"); // Redirect to home or an error page
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning($"Attempted to add product {productId} to cart without a logged-in user.");
                    TempData["ErrorMessage"] = "You need to be logged in to add items to your cart.";
                    return RedirectToAction("Login", "Account");
                }

                var product = await _context.Products.FindAsync(productId);

                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {productId} not found when adding to cart.");
                    return NotFound();
                }

                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

                if (cartItem == null)
                {
                    cartItem = new CartItem
                    {
                        UserId = userId,
                        ProductId = productId,
                        Quantity = quantity
                    };
                    _context.CartItems.Add(cartItem);
                    _logger.LogInformation($"Added new item (Product ID: {productId}, Quantity: {quantity}) to cart for user {userId}.");
                }
                else
                {
                    cartItem.Quantity += quantity;
                    _logger.LogInformation($"Updated quantity for item (Product ID: {productId}) in cart for user {userId}. New quantity: {cartItem.Quantity}.");
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Item added to cart successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding product {productId} to cart for user.");
                TempData["ErrorMessage"] = "An error occurred while adding the item to your cart. Please try again.";
                return RedirectToAction("Details", "Product", new { id = productId }); // Redirect back to product details or cart
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

                if (cartItem == null)
                {
                    _logger.LogWarning($"Cart item with ID {id} not found or does not belong to user {userId} during quantity update.");
                    return NotFound();
                }

                if (quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                    _logger.LogInformation($"Removed cart item with ID {id} for user {userId} as quantity was set to 0 or less.");
                    TempData["SuccessMessage"] = "Item removed from cart.";
                }
                else
                {
                    cartItem.Quantity = quantity;
                    _logger.LogInformation($"Updated quantity for cart item with ID {id} for user {userId}. New quantity: {quantity}.");
                    TempData["SuccessMessage"] = "Cart quantity updated.";
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating quantity for cart item with ID {id} for user.");
                TempData["ErrorMessage"] = "An error occurred while updating the quantity. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

                if (cartItem == null)
                {
                    _logger.LogWarning($"Cart item with ID {id} not found or does not belong to user {userId} during removal.");
                    return NotFound();
                }

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Removed cart item with ID {id} for user {userId}.");
                TempData["SuccessMessage"] = "Item removed from cart successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing cart item with ID {id} for user.");
                TempData["ErrorMessage"] = "An error occurred while removing the item. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Attempted checkout without a logged-in user.");
                    TempData["ErrorMessage"] = "You need to be logged in to complete your order.";
                    return RedirectToAction("Login", "Account");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogError($"User with ID {userId} not found during checkout.");
                    TempData["ErrorMessage"] = "User not found. Please log in again.";
                    return RedirectToAction("Login", "Account");
                }

                var cartItems = await _context.CartItems
                    .Include(c => c.Product)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    _logger.LogWarning($"User {userId} attempted to checkout with an empty cart.");
                    TempData["WarningMessage"] = "Your cart is empty. Please add items before checking out.";
                    return RedirectToAction(nameof(Index));
                }

                // Create a new order
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = cartItems.Sum(c => c.Quantity * c.Product.Price),
                    Status = "Pending",
                    DeliveryAddress = user.Address ?? "No address provided", // Use user's address or a default
                    Notes = ""
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"New order {order.Id} created for user {userId}.");

                // Create order items
                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product.Price // Use current product price
                    };

                    _context.OrderItems.Add(orderItem);
                }

                // Clear the cart
                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Cart cleared and order items added for order {order.Id}.");

                TempData["SuccessMessage"] = "Your order has been placed successfully!";
                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during checkout process.");
                TempData["ErrorMessage"] = "An error occurred during checkout. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning($"Attempted to view order confirmation {orderId} without a logged-in user.");
                    TempData["ErrorMessage"] = "You need to be logged in to view order details.";
                    return RedirectToAction("Login", "Account");
                }

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

                if (order == null)
                {
                    _logger.LogWarning($"Order with ID {orderId} not found or does not belong to user {userId}.");
                    return NotFound();
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order confirmation for order ID {orderId}.");
                TempData["ErrorMessage"] = "An error occurred while loading your order confirmation. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}