using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using course.Models;
using Microsoft.AspNetCore.Identity;
using course.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace course.Controllers
{
    [Authorize(Roles = "admin, user")]
    public class OrdersController : Controller
    {
        private readonly DeliveryDBContext _context;
        private readonly UserManager<DeliveryUser> _userManager;
        private static int _cartID = 0;

        public OrdersController(DeliveryDBContext context, UserManager<DeliveryUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {

            if (User.IsInRole("user"))
            {
                var user = await _userManager.GetUserAsync(User);
                var id = user.Id;
                var deliveryDBContext = _context.orders.Where(x => x.userID == id);
                return View(await deliveryDBContext.ToListAsync());
            }
            else
            {
                return View(await _context.orders.ToListAsync());
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
           

            var order = await _context.orders
                .FirstOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }
       

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,userID,quantity,price,orderdate,adresscity,adressstreet,adresshome")] Order order)
        {

                var user = await _userManager.GetUserAsync(HttpContext.User);
                var userr = await _userManager.GetUserAsync(User);
                var idd = userr.Id;
                var name = userr.FirstName + " " + userr.LastName;
                order.employer = name;
                order.userID = idd;
                var cart = _context.cart.Include(c => c.ProductsHistory).Where(c => c.userID == idd);
                int count = 0;
                int pricec = 0;
                foreach (var carts in cart)
                {
                    count += 1;
                    pricec += carts.ProductsHistory.price;
                }
                order.price = pricec;
                order.quantity = count;
                order.orderdate = DateTime.Now;
                _context.Add(order);
 
                _context.RemoveRange(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,userID,quantity,price,orderdate,adresscity,adressstreet,adresshome")] Order order)
        {
            if (id != order.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.orders
                .FirstOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.orders.FindAsync(id);
            if (order != null)
            {
                _context.orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.orders.Any(e => e.ID == id);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToHistory(int id)
        {
            var orders = await _context.orders.FindAsync(id);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (orders != null && user != null)
            {
                _context.orders.Remove(orders);
                History history = new History()
                {
                    price = orders.price,
                    quantity = orders.quantity,
                    adressstreet = orders.adressstreet,
                    adresshome = orders.adresshome,
                    employer = orders.employer,
                    adresscity = orders.adresscity,
                    userID = orders.userID,
                    orderdate = orders.orderdate
                };

                _context.Add(history);
                await _context.SaveChangesAsync();

                // Возвращаем пользователя на страницу, с которой он пришёл
                var referer = Request.Headers["Referer"].ToString();
                return Redirect(referer);
            }
            return Json(new { success = false });
        }
    }
}
