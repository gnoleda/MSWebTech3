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
    public class ADTreatmentFertilizerController : Controller
    {
        private readonly OECContext _context;

        public ADTreatmentFertilizerController(OECContext context)
        {
            _context = context;
        }

        // GET: ADTreatmentFertilizer
        public async Task<IActionResult> Index(string treatmentId, string name)
        {
            var treatmentFert = _context.TreatmentFertilizer.Include(t => t.FertilizerNameNavigation).Include(t => t.Treatment);

            //if the treatmentId is in the url or query string, save it to session
            if (treatmentId !=null)
            {
                HttpContext.Session.SetString("treatmentId", treatmentId);
                HttpContext.Session.SetString("name", name);

                //filter treatmentFertilizer using treatment id to show only fertilizers on that file.
                var treatmentFert1 = _context.TreatmentFertilizer
                                .Where(t => t.TreatmentFertilizerId == Convert.ToInt32(treatmentId))
                                .Include(t => t.FertilizerNameNavigation)
                                .Include(t => t.Treatment)
                                .OrderBy(t => t.FertilizerName);

                ViewBag.Name = name;
                ViewBag.TreatmentId = treatmentId;

                return View(await treatmentFert1.ToListAsync());
            }
            //if no treatment id passed, check for session variable with it
            else if (treatmentId == null)
            {
                if (HttpContext.Session.GetString("treatmentId") != null)
                {
                    treatmentId = HttpContext.Session.GetString("treatmentId");
                    name = HttpContext.Session.GetString("name");

                    //filter treatmentFertilizer using treatment id to show only fertilizers on that file.
                    var treatmentFert2 = _context.TreatmentFertilizer
                                        .Where(t => t.TreatmentFertilizerId == Convert.ToInt32(treatmentId))
                                        .Include(t => t.FertilizerNameNavigation)
                                        .Include(t => t.Treatment)
                                        .OrderBy(t => t.FertilizerName);

                    ViewBag.Name = name;
                    ViewBag.TreatmentId = treatmentId;

                    return View(await treatmentFert2.ToListAsync());
                }
                //if no treatment id, return to treatment controller with message
                else
                {
                    TempData["message"] = "Please select a treatment to see it's fertilizer composition";
                    return RedirectToAction("Index", "ADTreatment");
                }
            }

            return View(await treatmentFert.ToListAsync());
        }

        // GET: ADTreatmentFertilizer/Details/5
        public async Task<IActionResult> Details(int? id, string name)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentFertilizer = await _context.TreatmentFertilizer
                .Include(t => t.FertilizerNameNavigation)
                .Include(t => t.Treatment)
                .SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);
            if (treatmentFertilizer == null)
            {
                return NotFound();
            }

            ViewBag.Name = name;
            //ViewBag.TreatmentId = treatmentId;

            return View(treatmentFertilizer);
        }

        // GET: ADTreatmentFertilizer/Create
        public IActionResult Create(string treatmentId, string name)
        {
            ViewData["FertilizerName"] = new SelectList(_context.Fertilizer, "FertilizerName", "FertilizerName");
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "TreatmentId");

            ViewBag.Name = name;
            //ViewBag.TreatmentId = treatmentId;

            return View();
        }

        // POST: ADTreatmentFertilizer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TreatmentFertilizerId,TreatmentId,FertilizerName,RatePerAcre,RateMetric")] TreatmentFertilizer treatmentFertilizer)
        {
            var checkRateMetric = _context.Fertilizer.Include(a => a.FertilizerName == treatmentFertilizer.FertilizerName && a.Liquid == true);

            if (checkRateMetric != null)
            {
                treatmentFertilizer.RateMetric = "Gal";
            }
            else
            {
                treatmentFertilizer.RateMetric = "Lb";
            }

            if (ModelState.IsValid)
            {
                _context.Add(treatmentFertilizer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["FertilizerName"] = new SelectList(_context.Fertilizer, "FertilizerName", "FertilizerName", treatmentFertilizer.FertilizerName);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "TreatmentId", treatmentFertilizer.TreatmentId);

            return View(treatmentFertilizer);
        }

        // GET: ADTreatmentFertilizer/Edit/5
        public async Task<IActionResult> Edit(int? id, string name)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentFertilizer = await _context.TreatmentFertilizer.SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);
            if (treatmentFertilizer == null)
            {
                return NotFound();
            }
            ViewData["FertilizerName"] = new SelectList(_context.Fertilizer, "FertilizerName", "FertilizerName", treatmentFertilizer.FertilizerName);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "TreatmentId", treatmentFertilizer.TreatmentId);

            ViewBag.Name = name;
            return View(treatmentFertilizer);
        }

        // POST: ADTreatmentFertilizer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TreatmentFertilizerId,TreatmentId,FertilizerName,RatePerAcre,RateMetric")] TreatmentFertilizer treatmentFertilizer)
        {
            if (id != treatmentFertilizer.TreatmentFertilizerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(treatmentFertilizer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreatmentFertilizerExists(treatmentFertilizer.TreatmentFertilizerId))
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
            ViewData["FertilizerName"] = new SelectList(_context.Fertilizer, "FertilizerName", "FertilizerName", treatmentFertilizer.FertilizerName);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "TreatmentId", treatmentFertilizer.TreatmentId);
            return View(treatmentFertilizer);
        }

        // GET: ADTreatmentFertilizer/Delete/5
        public async Task<IActionResult> Delete(int? id, string name)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentFertilizer = await _context.TreatmentFertilizer
                .Include(t => t.FertilizerNameNavigation)
                .Include(t => t.Treatment)
                .SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);
            if (treatmentFertilizer == null)
            {
                return NotFound();
            }

            ViewBag.Name = name;

            return View(treatmentFertilizer);
        }

        // POST: ADTreatmentFertilizer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treatmentFertilizer = await _context.TreatmentFertilizer.SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);
            _context.TreatmentFertilizer.Remove(treatmentFertilizer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TreatmentFertilizerExists(int id)
        {
            return _context.TreatmentFertilizer.Any(e => e.TreatmentFertilizerId == id);
        }
    }
}
