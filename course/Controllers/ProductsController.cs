using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using course.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using course.Areas.Identity.Data;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace course.Controllers
{
    [Authorize(Roles = "admin, user")]
    public class ProductsController : Controller
    {
        private readonly DeliveryDBContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly UserManager<DeliveryUser> _userManager;

        public ProductsController(DeliveryDBContext context, IWebHostEnvironment appEnvironment, UserManager<DeliveryUser> userManager)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            _userManager = userManager;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.product.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.product
                .FirstOrDefaultAsync(m => m.ID == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }
        [Authorize(Roles = "admin")]
        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("ID,NameP,price,qiantity,image")] Products products, IFormFile upload)
        {
            if (ModelState.IsValid)
            {
                ProductsHistory ph = new ProductsHistory();
                if (upload != null)
                {
                    string path = "/Files/" + upload.FileName;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }
                    products.image = path;
                    ph.image = path;
                }
                ph.price = products.price;
                ph.NameP = products.NameP;
                _context.Add(ph);
                _context.Add(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }
        [Authorize(Roles = "admin")]
        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.product.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,NameP,price,qiantity,image")] Products products, IFormFile? upload)
        {
            if (id != products.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                ProductsHistory ph = new ProductsHistory();
                if (upload != null)
                {
                    string path = "/Files/" + upload.FileName;
                    using (var fileStream = new
                   FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }
                    if (!products.image.IsNullOrEmpty())
                    {
                        System.IO.File.Delete(_appEnvironment.WebRootPath + products.image);
                    }
                    products.image = path;
                    ph.image = products.image;
                }
                try
                { 
                    ph.price = products.price;
                    ph.NameP = products.NameP;
                    _context.Update(ph);
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.ID))
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
            return View(products);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.product
                .FirstOrDefaultAsync(m => m.ID == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id)
        {
            var products = await _context.product.FindAsync(id);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userr = await _userManager.GetUserAsync(User);
            var idd = userr.Id;
            if (products != null && user != null)
            {
                Cart cart = new Cart()
                {
                    userID = idd,
                    ProductsHistoryID = products.ID,
                };

                _context.Add(cart);
                await _context.SaveChangesAsync();

                // Возвращаем пользователя на страницу, с которой он пришёл
                var referer = Request.Headers["Referer"].ToString();
                return Redirect(referer);
            }
            return Json(new { success = false });
        }

            // POST: Products/Delete/5
            [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var products = await _context.product.FindAsync(id);
            if (products != null)
            {
                _context.product.Remove(products);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
            return _context.product.Any(e => e.ID == id);
        }
    }
}
