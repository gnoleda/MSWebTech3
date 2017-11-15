using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using a1OEC.Models;

// controller to display and maintain the Crop table
// D. Turton Sep 2017

namespace a1OEC.Controllers
{
    public class ADCropController : Controller
    {
        private readonly OECContext _context;

        public ADCropController(OECContext context)
        {
            _context = context;
        }

        // display all crops currently on file
        public async Task<IActionResult> Index()
        {
            var cropListing = await _context.Crop
                                .OrderBy(c => c.Name)
                                .ToListAsync();

            return View(cropListing);
        }

        // display the details of the selected crop
        public async Task<IActionResult> Details(int? id, string name)
        {
            ViewBag.CropId = id;
            ViewBag.CropName = name;

            if (id == null)
            {
                return NotFound();
            }

            var crop = await _context.Crop
                .SingleOrDefaultAsync(m => m.CropId == id);
            if (crop == null)
            {
                return NotFound();
            }

            return View(crop);
        }

        // prsent a blank input screen for the user to create a new crop
        public IActionResult Create()
        {
            return View();
        }

        // save the new crop to the database, if it passees edits
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CropId,Name,Image")] Crop crop)
        {
            if (ModelState.IsValid)
            {
                _context.Add(crop);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(crop);
        }

        // load the selected crop record to an input screen for updating
        public async Task<IActionResult> Edit(int? id, string name)
        {
            ViewBag.CropId = id;
            ViewBag.CropName = name;

            if (id == null)
            {
                return NotFound();
            }

            var crop = await _context.Crop.SingleOrDefaultAsync(m => m.CropId == id);
            if (crop == null)
            {
                return NotFound();
            }
            return View(crop);
        }

        // // save the updated crop to the database, if it passees edits
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CropId,Name,Image")] Crop crop)
        {
            if (id != crop.CropId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(crop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CropExists(crop.CropId))
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
            return View(crop);
        }

        // prsent the details of the selected crop for the user to confirm delete
        public async Task<IActionResult> Delete(int? id, string name)
        {
            ViewBag.CropId = id;
            ViewBag.CropName = name;

            if (id == null)
            {
                return NotFound();
            }

            var crop = await _context.Crop
                .SingleOrDefaultAsync(m => m.CropId == id);
            if (crop == null)
            {
                return NotFound();
            }

            return View(crop);
        }

        // delete the selected crop
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var crop = await _context.Crop.SingleOrDefaultAsync(m => m.CropId == id);
            _context.Crop.Remove(crop);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // method to confirm a selected crop exists on the database
        private bool CropExists(int id)
        {
            return _context.Crop.Any(e => e.CropId == id);
        }
    }
}
