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
using OfficeOpenXml;

namespace course.Controllers
{
    [Authorize(Roles = "admin, user")]
    public class HistoriesController : Controller
    {
        private readonly UserManager<DeliveryUser> _userManager;
        private readonly DeliveryDBContext _context;
        private readonly IWebHostEnvironment _appEnvironment;


        public HistoriesController(DeliveryDBContext context, UserManager<DeliveryUser> userManager, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
        }
        public FileResult GetReport()
        {
            // Путь к файлу с шаблоном
            string path = "/Reports/reportTemplateHistory.xlsx";
            //Путь к файлу с результатом
            string result = "/Reports/reportHistory.xlsx";
            FileInfo fi = new FileInfo(_appEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(_appEnvironment.WebRootPath + result);
            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "Скотников Е.С.";
                excelPackage.Workbook.Properties.Title = "История заказов";
                excelPackage.Workbook.Properties.Subject = "Заказы";
                excelPackage.Workbook.Properties.Created = DateTime.Now;
                //плучаем лист по имени.
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["History"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<History> History = _context.history.ToList();
                
                foreach (History history in History)
                {
                    worksheet.Cells[startLine, 1].Value = startLine - 2;
                    worksheet.Cells[startLine, 2].Value = history.quantity;
                    worksheet.Cells[startLine, 3].Value = history.orderdate;
                    worksheet.Cells[startLine, 3].Style.Numberformat.Format = "yyyy-mm-dd";
                    worksheet.Cells[startLine, 4].Value = history.price;
                    worksheet.Cells[startLine, 5].Value = history.adresscity;
                    worksheet.Cells[startLine, 6].Value = history.adressstreet;
                    worksheet.Cells[startLine, 7].Value = history.adresshome;

                    startLine++;
                }
                //созраняем в новое место
                excelPackage.SaveAs(fr);
            }
            // Тип файла - content-type
            string file_type =
           "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
            // Имя файла - необязательно
            string file_name = "report.xlsx";
            return File(result, file_type, file_name);
        }

        // GET: Histories
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("user"))
            {
                var user = await _userManager.GetUserAsync(User);
                var id = user.Id;
                var deliveryDBContext = _context.history.Where(x => x.userID == id);
                return View(await deliveryDBContext.ToListAsync());
            }
            else
            {
                return View(await _context.history.ToListAsync());
            }
        }

        // GET: Histories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var history = await _context.history
                .FirstOrDefaultAsync(m => m.ID == id);
            if (history == null)
            {
                return NotFound();
            }

            return View(history);
        }

        // GET: Histories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Histories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,userID,quantity,price,orderdate,adresscity,adressstreet,adresshome")] History history)
        {
            if (ModelState.IsValid)
            {
                _context.Add(history);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(history);
        }

        // GET: Histories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var history = await _context.history.FindAsync(id);
            if (history == null)
            {
                return NotFound();
            }
            return View(history);
        }

        // POST: Histories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,userID,quantity,price,orderdate,adresscity,adressstreet,adresshome")] History history)
        {
            if (id != history.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(history);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistoryExists(history.ID))
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
            return View(history);
        }

        // GET: Histories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var history = await _context.history
                .FirstOrDefaultAsync(m => m.ID == id);
            if (history == null)
            {
                return NotFound();
            }

            return View(history);
        }

        // POST: Histories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var history = await _context.history.FindAsync(id);
            if (history != null)
            {
                _context.history.Remove(history);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HistoryExists(int id)
        {
            return _context.history.Any(e => e.ID == id);
        }
    }
}
