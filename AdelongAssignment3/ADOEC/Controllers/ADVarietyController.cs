using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using a1OEC.Models;
using Microsoft.AspNetCore.Http;

namespace ADOEC.Controllers
{
    public class ADVarietyController : Controller
    {
        private readonly OECContext _context;

        public ADVarietyController(OECContext context)
        {
            _context = context;
        }

        // GET: ADVariety
        public async Task<IActionResult> Index(string cropId, string name)
        {
            //if no crop id is in url or query string, save to session
            if(cropId != null )
            {
                HttpContext.Session.SetString("cropId", cropId);
            }
            //if crop id is null
            else if(cropId == null)
            {
                //look for it in session
                if(HttpContext.Session.GetString("cropId") != null)
                {
                    cropId = HttpContext.Session.GetString("cropId");
                }
                //if it is empty, go back to croplisting with message
                else
                {
                    TempData["message"] = "Please select a crop";
                    return RedirectToAction("Index", "ADCrop");
                }
            }

            //if no name is in url or query string, save to session
            if (name != null)
            {
                HttpContext.Session.SetString("name", name);

                //persist cropID and name
                ViewBag.CropId = cropId;
                ViewBag.Name = name;
            }
            //if name is null
            else if (name == null)
            {
                //look for it in session
                if (HttpContext.Session.GetString("name") != null)
                {
                    name = HttpContext.Session.GetString("name");

                    //persist cropID and name
                    ViewBag.CropId = cropId;
                    ViewBag.Name = name;
                }
                //if it is empty, fetch record from crop table, then extract and persist its name
                else
                {
                    var name1 =  _context.Crop
                            .Include(v => v.Name)
                            .Where(v => v.CropId.Equals(int.Parse(cropId)));

                    //persist cropID and name
                    ViewBag.CropId = cropId;
                    ViewBag.Name = name1;
                }
            }

            ////persist cropID and name
            //ViewBag.CropId = cropId;
            //ViewBag.Name = name;

            //filter crop listing 
            var cropContext =    _context.Variety
                                .Include(v => v.Crop)
                                .Where( v=> v.CropId.Equals(int.Parse(cropId)))
                                .OrderBy(v => v.Name);

            return View(await cropContext.ToListAsync());
        }

        // GET: ADVariety/Details/5
        public async Task<IActionResult> Details(int? id, string name)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variety = await _context.Variety
                .Include(v => v.Crop)
                .SingleOrDefaultAsync(m => m.VarietyId == id);
            if (variety == null)
            {
                return NotFound();
            }

            ViewBag.VarId = id;
            ViewBag.Name = name;

            return View(variety);
        }

        // GET: ADVariety/Create
        public IActionResult Create()
        {
            ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "CropId");
            string cropId = HttpContext.Session.GetString("cropId");
            string name = HttpContext.Session.GetString("name");

            ViewBag.CropId = cropId;
            ViewBag.Name = name;

            return View();
        }

        // POST: ADVariety/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VarietyId,CropId,Name")] Variety variety)
        {
            if (ModelState.IsValid)
            {
                _context.Add(variety);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "CropId", variety.CropId);
            return View(variety);
        }

        // GET: ADVariety/Edit/5
        public async Task<IActionResult> Edit(int? id, string name)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variety = await _context.Variety.SingleOrDefaultAsync(m => m.VarietyId == id);
            if (variety == null)
            {
                return NotFound();
            }

            ViewBag.VarId = id;
            ViewBag.Name = name;

            ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "CropId", variety.CropId);
            return View(variety);
        }

        // POST: ADVariety/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VarietyId,CropId,Name")] Variety variety)
        {
            if (id != variety.VarietyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(variety);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VarietyExists(variety.VarietyId))
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
            ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "CropId", variety.CropId);
            return View(variety);
        }

        // GET: ADVariety/Delete/5
        public async Task<IActionResult> Delete(int? id, string name)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variety = await _context.Variety
                .Include(v => v.Crop)
                .SingleOrDefaultAsync(m => m.VarietyId == id);
            if (variety == null)
            {
                return NotFound();
            }

            ViewBag.VarId = id;
            ViewBag.Name = name;

            return View(variety);
        }

        // POST: ADVariety/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var variety = await _context.Variety.SingleOrDefaultAsync(m => m.VarietyId == id);
            _context.Variety.Remove(variety);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VarietyExists(int id)
        {
            return _context.Variety.Any(e => e.VarietyId == id);
        }
    }
}
