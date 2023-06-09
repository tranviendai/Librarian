﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Librarian.Data;
using Librarian.Models;
using System.Net;

namespace Librarian.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get Search Suggestions
        public IActionResult GetSearchSuggestions(string searchTerm, string searchType)
        {
            var suggestions = new List<string>();
            var books = _context.Book.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (searchType == "title")
                {
                    suggestions = books.Where(b => b.title.Contains(searchTerm)).Select(b => b.title).ToList();
                }
                else if (searchType == "author")
                {
                    suggestions = books.Where(b => b.author.Contains(searchTerm)).Select(b => b.author).ToList();
                }
            }
            return Json(suggestions);
        }

        // GET: Books
        public async Task<IActionResult> Index(string searchTerm, string searchType)
        {
            var books = _context.Book.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (searchType == "title")
                {
                    books = books.Where(b => b.title.Contains(searchTerm));
                }
                else if (searchType == "author")
                {
                    books = books.Where(b => b.author.Contains(searchTerm));
                }
            }
            return View(await books.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.bookID == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create(int i = 0)
        {
            Book book = new Book();
            var autoID = _context.Book.OrderByDescending(c => c.bookID).FirstOrDefault();
            if (i != 0)
            {
                book = _context.Book.Where(x => x.bookIndex == i).FirstOrDefault();
            }
            else if (autoID == null)
            {
                book.bookID = "B0001";
            }
            else
            {
                book.bookID = "B000" + (autoID.bookIndex + 1).ToString();
                if (autoID.bookIndex >= 9)
                {
                    book.bookID = "B00" + (autoID.bookIndex + 1).ToString();
                }
                if (autoID.bookIndex >= 99)
                {
                    book.bookID = "B0" + (autoID.bookIndex + 1).ToString();
                }
                else
                {
                    book.bookID = "B" + (autoID.bookIndex + 1).ToString();
                }

            }
           
            ViewBag.categoryID = new SelectList(_context.Category, "categoryID", "nameCategory");
            return View(book);
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("bookID,bookIndex,title,author,image,publisher,publishingYear,summary,count,addDate,categoryID")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("bookID,bookIndex,title,author,image,publisher,publishingYear,summary,count,addDate,categoryID")] Book book)
        {
            if (id != book.bookID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.bookID))
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
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.bookID == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(string id)
        {
            return (_context.Book?.Any(e => e.bookID == id)).GetValueOrDefault();
        }
    }
}
