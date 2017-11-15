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
    public class ADTreatmentController : Controller
    {
        private readonly OECContext _context;

        public ADTreatmentController(OECContext context)
        {
            _context = context;
        }

        // GET: ADTreatment
        public async Task<IActionResult> Index(string plotId, string farmName)
        {
            //if plot Id is in URL/query string, save it to session
            if (plotId != null)
            {
                HttpContext.Session.SetString("plotId", plotId);

                //check if farmName is there or not.
                if (farmName != null)
                {
                    HttpContext.Session.SetString("farmName", farmName);
                }
                else if (farmName == null)
                {
                    if (HttpContext.Session.GetString("farmName") != null)
                    {
                        farmName = HttpContext.Session.GetString("farmName");
                    }
                    else
                    {
                        //fetch name from database
                        var fetchFarmName = _context.Plot
                                    .Where(p => p.Farm.FarmId == Convert.ToInt32(plotId))
                                    .Include(p => p.Farm.Name);
                    }
                }

                //use plot id to filter list of treatments
                var treatment = _context.Treatment
                        .Where(t => t.TreatmentId == Convert.ToInt32(plotId))
                        .Include(t => t.Plot)
                        .OrderBy(t => t.Name);

                ViewBag.FarmName = farmName;
                ViewBag.PlotId = plotId;

                return View(await treatment.ToListAsync());
            }

            //if not plotId passed, see if there is a session variable with it
            else if (plotId == null)
            {
                if (HttpContext.Session.GetString("plotId") != null)
                {
                    //assign it to variable
                    plotId = HttpContext.Session.GetString("plotId");

                    //check if farmName is there or not.
                    if (farmName != null)
                    {
                        HttpContext.Session.SetString("farmName", farmName);
                    }
                    else if (farmName == null)
                    {
                        if (HttpContext.Session.GetString("farmName") != null)
                        {
                            farmName = HttpContext.Session.GetString("farmName");
                        }
                        else
                        {
                            var fetchFarmName = _context.Plot
                                        .Where(p => p.Farm.FarmId == Convert.ToInt32(plotId))
                                        .Include(p => p.Farm.Name);
                        }
                    }

                    //use plot id to filter list of treatments
                    var treatment = _context.Treatment
                                    .Where(t => t.TreatmentId == Convert.ToInt32(plotId))
                                    .Include(t => t.Plot)
                                    .OrderBy(t => t.Name);

                    ViewBag.FarmName = farmName;
                    ViewBag.PlotId = plotId;

                    return View(await treatment.ToListAsync());
                }

                else
                {//if not in session variable either...display a message
                    TempData["message"] = "Please select a plot to see it's treatments.";
                    return RedirectToAction("Index", "ADPlot");
                }
            }
            //do the same thing for farm name
            else if (farmName != null)
            {
                var treatment = _context.Treatment
                .Where(t => t.TreatmentId == Convert.ToInt32(plotId))
                .Include(t => t.Plot)
                .OrderBy(t => t.Name);

                return View(await treatment.ToListAsync());
            }
            else
            {
                var oECContext = _context.Treatment.Include(t => t.Plot);
                return View(await oECContext.ToListAsync());
            }
        }

        // GET: ADTreatment/Details/5
        public async Task<IActionResult> Details(int? id, string farmName)
        {
            ViewBag.FarmName = farmName;

            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatment
                .Include(t => t.Plot)
                .SingleOrDefaultAsync(m => m.TreatmentId == id);
            if (treatment == null)
            {
                return NotFound();
            }

            return View(treatment);
        }

        // GET: ADTreatment/Create
        public IActionResult Create(string farmName)
        {
            ViewBag.FarmName = farmName;

            ViewData["PlotId"] = new SelectList(_context.Plot, "PlotId", "PlotId");
            return View();
        }

        // POST: ADTreatment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TreatmentId,Name,PlotId,Moisture,Yield,Weight")] Treatment treatment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(treatment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PlotId"] = new SelectList(_context.Plot, "PlotId", "PlotId", treatment.PlotId);
            return View(treatment);
        }

        // GET: ADTreatment/Edit/5
        public async Task<IActionResult> Edit(int? id, string farmName)
        {
            ViewBag.FarmName = farmName;

            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatment.SingleOrDefaultAsync(m => m.TreatmentId == id);
            if (treatment == null)
            {
                return NotFound();
            }
            ViewData["PlotId"] = new SelectList(_context.Plot, "PlotId", "PlotId", treatment.PlotId);
            return View(treatment);
        }

        // POST: ADTreatment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TreatmentId,Name,PlotId,Moisture,Yield,Weight")] Treatment treatment)
        {
            if (id != treatment.TreatmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(treatment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreatmentExists(treatment.TreatmentId))
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
            ViewData["PlotId"] = new SelectList(_context.Plot, "PlotId", "PlotId", treatment.PlotId);
            return View(treatment);
        }

        // GET: ADTreatment/Delete/5
        public async Task<IActionResult> Delete(int? id, string farmName)
        {
            ViewBag.FarmName = farmName;

            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatment
                .Include(t => t.Plot)
                .SingleOrDefaultAsync(m => m.TreatmentId == id);
            if (treatment == null)
            {
                return NotFound();
            }

            return View(treatment);
        }

        // POST: ADTreatment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treatment = await _context.Treatment.SingleOrDefaultAsync(m => m.TreatmentId == id);
            _context.Treatment.Remove(treatment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TreatmentExists(int id)
        {
            return _context.Treatment.Any(e => e.TreatmentId == id);
        }
    }
}
