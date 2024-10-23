using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Expense_App.Models;

namespace Expense_App.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.transactions.Include(t => t.Category).OrderByDescending(j => j.Date);
            return View(await applicationDbContext.ToListAsync());
        }



        // GET: Transaction/Create
        public IActionResult AddOrEdit(int id = 0)
        {
            populateCategories();
            if (id == 0)
                return View(new Transaction());
            else
                return View(_context.transactions.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if(transaction.TransactionId == 0)
                _context.Add(transaction);
                else
                    _context.Update(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            populateCategories();
            return View(transaction);
        }

        
        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public void populateCategories()
        {
            var categorie = _context.categories.ToList();
            Category DefaultCategory = new Category() { CategoryId = 0, Title="Choose a Category" };
            categorie.Insert(0,DefaultCategory);
            ViewBag.Categories = categorie;
        } 

         }
}
