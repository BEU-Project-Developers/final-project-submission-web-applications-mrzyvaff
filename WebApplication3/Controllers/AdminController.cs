using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var pendingOrdersCount = await _context.Orders.CountAsync(o => o.Status == "Pending");
            var completedOrdersCount = await _context.Orders.CountAsync(o => o.Status == "Completed");
            var totalRevenue = await _context.Orders
                .Where(o => o.Status == "Completed")
                .SumAsync(o => o.TotalAmount);
            
            var productCount = await _context.Products.CountAsync();
            var userCount = await _context.Users.CountAsync();

            ViewBag.PendingOrdersCount = pendingOrdersCount;
            ViewBag.CompletedOrdersCount = completedOrdersCount;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.ProductCount = productCount;
            ViewBag.UserCount = userCount;

            return View();
        }

        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(OrderDetails), new { id = id });
        }

        public async Task<IActionResult> Revenue()
        {
            var completedOrders = await _context.Orders
                .Include(o => o.User)
                .Where(o => o.Status == "Completed")
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var totalRevenue = completedOrders.Sum(o => o.TotalAmount);
            
            // Calculate revenue by date (last 30 days)
            var revenueByDate = completedOrders
                .Where(o => o.OrderDate >= DateTime.UtcNow.AddDays(-30))
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(o => o.TotalAmount) })
                .OrderBy(x => x.Date)
                .ToList();

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.RevenueByDate = revenueByDate;
            ViewBag.CompletedOrders = completedOrders.Count;

            return View(completedOrders);
        }
    }
} 