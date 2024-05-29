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
using Microsoft.AspNetCore.Components;

namespace course.Controllers
{
    [Authorize(Roles = "user")]
    public class CartsController : Controller
    {
        private readonly UserManager<DeliveryUser> _userManager;
        private readonly DeliveryDBContext _context;
        private static int _cartID = 0;


        public CartsController(DeliveryDBContext context, UserManager<DeliveryUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: Carts
        public async Task<IActionResult> Index()
        {
                var user = await _userManager.GetUserAsync(User);
                var id = user.Id;
                var deliveryDBContext = _context.cart.Include(c => c.ProductsHistory).Where(x => x.userID == id);
                return View(await deliveryDBContext.ToListAsync());
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.cart
                .Include(c => c.ProductsHistory)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }
        
        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["ProductsID"] = new SelectList(_context.product, "ID", "NameP");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admins")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,userID,ProductsID,quantity")] Cart cart)
        {
        
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var id = user.Id;
                cart.userID = id;
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductsID"] = new SelectList(_context.product, "ID", "ID", cart.ProductsHistoryID);
            return View(cart);
        }
        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["ProductsID"] = new SelectList(_context.product, "ID", "ID", cart.ProductsHistoryID);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,userID,ProductsID,quantity")] Cart cart)
        {
            if (id != cart.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.ID))
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
            ViewData["ProductsID"] = new SelectList(_context.product, "ID", "ID", cart.ProductsHistoryID);
            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add()
         {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userr = await _userManager.GetUserAsync(User);
            var idd = userr.Id;
            Cart cart = new Cart();
            var deliveryDBContext = _context.cart.Where(x => x.userID == idd);
            return Redirect("/Orders/Create");

        }
        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.cart
                .Include(c => c.ProductsHistory)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.cart.FindAsync(id);
            if (cart != null)
            {
                _context.cart.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.cart.Any(e => e.ID == id);
        }
    }
}
