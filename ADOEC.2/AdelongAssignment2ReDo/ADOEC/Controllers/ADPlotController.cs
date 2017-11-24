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
    public class ADPlotController : Controller
    {
        private readonly OECContext _context;

        public ADPlotController(OECContext context)
        {
            _context = context;
        }
       
        // GET: ADPlot
        public async Task<IActionResult> Index(string cropId, string name, string sort, string varId, string varName, string searchCriteria, string plotId, string plotName)
        {
            //by default plot listing is sorted by date planted
            var plots = _context.Plot
                        .Include(p => p.Farm)
                        .Include(p => p.Variety)
                        .Include(p => p.Variety.Crop)
                        .Include(p => p.Treatment)
                        .OrderByDescending(p => p.DatePlanted);

            if(searchCriteria != null)
            {
                HttpContext.Session.SetString("searchCriteria", searchCriteria);
            }

            //have index action identify if cropid or varid, and persist to identifier
            if (plotId != null)
            {
                //if found persist it as new filter criteria and limit listing to selected plot
                var plotsForPlotId = _context.Plot
                                    .Where(p => p.PlotId == Convert.ToInt32(plotId))
                                    .Include(p => p.Farm)
                                    .Include(p => p.Variety)
                                    .Include(p => p.Variety.Crop)
                                    .Include(p => p.Treatment)
                                    .OrderByDescending(p => p.DatePlanted);

                if (sort != null)
                {
                    switch (sort)
                    {
                        case "farmName":
                            return View(await plotsForPlotId.OrderBy(p => p.Farm.Name).ToListAsync());
                        case "variety":
                            return View(await plotsForPlotId.OrderBy(p => p.Variety.Name).ToListAsync());
                        case "cec":
                            return View(await plotsForPlotId.OrderBy(p => p.Cec).ToListAsync());
                    }
                }
                else
                {
                    HttpContext.Session.SetString("plotId", plotId);
                    //HttpContext.Session.SetString("plotName", plotName);

                    plotId = HttpContext.Session.GetString("plotId");
                    //plotName = HttpContext.Session.GetString("varName");

                    ViewBag.PlotId = plotId;
                    //if you clicked the "back to plot listing" from the treatment page, keep the name already in viewbag, not the "plot name"
                    if (plotId != null && name != null)
                    {
                        ViewBag.Name = name;
                    }
                    else
                    {
                        ViewBag.Name = plotName;
                    }

                }


                return View(await plotsForPlotId.ToListAsync());
                
            }
            else if (searchCriteria == "v" || varId != null)
            {
                //use varId to restrict the listing to plots with that id
                var plotsForVar = _context.Plot
                            .Where(p => p.Variety.VarietyId == Convert.ToInt32(varId))
                            .Include(p => p.Farm)
                            .Include(p => p.Variety)
                            .Include(p => p.Variety.Crop)
                            .Include(p => p.Treatment)
                            .OrderByDescending(p => p.DatePlanted);

                if (sort != null)
                {
                    switch (sort)
                    {
                        case "farmName":
                            return View(await plotsForVar.OrderBy(p => p.Farm.Name).ToListAsync());
                        case "variety":
                            return View(await plotsForVar.OrderBy(p => p.Variety.Name).ToListAsync());
                        case "cec":
                            return View(await plotsForVar.OrderBy(p => p.Cec).ToListAsync());
                    }
                }
                else
                {
                    HttpContext.Session.SetString("varId", varId);
                    HttpContext.Session.SetString("varName", varName);

                    varId = HttpContext.Session.GetString("varId");
                    varName = HttpContext.Session.GetString("varName");

                    ViewBag.VarId = varId;
                    ViewBag.Name = varName;

                }
                

                return View(await plotsForVar.ToListAsync());
            }
            //look for passed plotId
            else if (searchCriteria == "c" || cropId != null)
            {
                //use cropId to restrict the listing to plots with that id
                var plotsForCrop = _context.Plot
                            .Include(p => p.Farm)
                            .Include(p => p.Variety)
                            .Include(p => p.Variety.Crop)
                            .Include(p => p.Treatment)
                            .OrderByDescending(p => p.DatePlanted)
                            .Where(p => p.Variety.CropId == Convert.ToInt32(cropId));

                if (sort != null)
                {
                    switch (sort)
                    {
                        case "farmName":
                            return View(await plotsForCrop.OrderBy(p => p.Farm.Name).ToListAsync());
                        case "variety":
                            return View(await plotsForCrop.OrderBy(p => p.Variety.Name).ToListAsync());
                        case "cec":
                            return View(await plotsForCrop.OrderBy(p => p.Cec).ToListAsync());
                    }

                }
                else
                {
                    //set session variables
                    HttpContext.Session.SetString("cropId", cropId);
                    HttpContext.Session.SetString("name", name);

                    //get sesion variables
                    cropId = HttpContext.Session.GetString("cropId");
                    name = HttpContext.Session.GetString("name");

                    ViewBag.CropId = cropId;
                    ViewBag.Name = name;
                }

                return View(await plotsForCrop.ToListAsync());
            }
            //when hitting the back button, this keeps the name in the ViewBag
            else if (searchCriteria == null && name != null)
            {
                var plotsForReturn = _context.Plot
                    //.Where(p => p.PlotId == Convert.ToInt32(plotId))
                    .Where (p => p.Variety.Crop.Name == name)
                    .Include(p => p.Farm)
                    .Include(p => p.Variety)
                    .Include(p => p.Variety.Crop)
                    .Include(p => p.Treatment)
                    .OrderByDescending(p => p.DatePlanted);

                ViewBag.Name = name;

                return View(await plotsForReturn.ToListAsync());
            }
            else
            {
                //else return the default plot listing
                return View(await plots.ToListAsync());
            }
 
        }

        // GET: ADPlot/Details/5
        public async Task<IActionResult> Details(int? id, string name)
        {
            ViewBag.Name = name;

            if (id == null)
            {
                return NotFound();
            }

            var plot = await _context.Plot
                .Include(p => p.Farm)
                .Include(p => p.Variety)
                .SingleOrDefaultAsync(m => m.PlotId == id);
            if (plot == null)
            {
                return NotFound();
            }

            return View(plot);
        }

        // GET: ADPlot/Create
        public IActionResult Create(string cropId, string varId, string name)
        {
            ViewBag.Name = name;
            
            if (cropId != null)
            {
                ViewData["FarmId"] = new SelectList(_context.Farm.OrderBy(f => f.Name), "FarmId", "Name");
                ViewData["Id"] = new SelectList(_context.Variety.OrderBy(v => v.Name).Where(v => v.CropId.Equals(int.Parse(cropId))), "CropId", "Name");
                return View();
            }
            else
            {
                ViewData["FarmId"] = new SelectList(_context.Farm.OrderBy(f => f.Name), "FarmId", "Name");
                ViewData["Id"] = new SelectList(_context.Variety.OrderBy(v => v.Name), "VarietyId", "Name", varId);
                return View();
            }

        }

        // POST: ADPlot/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlotId,FarmId,VarietyId,DatePlanted,DateHarvested,PlantingRate,PlantingRateByPounds,RowWidth,PatternRepeats,OrganicMatter,BicarbP,Potassium,Magnesium,Calcium,PHsoil,PHbuffer,Cec,PercentBaseSaturationK,PercentBaseSaturationMg,PercentBaseSaturationCa,PercentBaseSaturationH,Comments")] Plot plot)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plot);
                await _context.SaveChangesAsync();
                //return to plot list with that varId or cropId
                return RedirectToAction(nameof(Index));
            }
            ViewData["FarmId"] = new SelectList(_context.Farm, "FarmId", "ProvinceCode", plot.FarmId);
            ViewData["VarietyId"] = new SelectList(_context.Variety, "VarietyId", "VarietyId", plot.VarietyId);
            return View(plot);
        }

        // GET: ADPlot/Edit/5
        public async Task<IActionResult> Edit(string cropId, string varId, string name)
        {
            ViewBag.Name = name;

            if (cropId != null)
            {
                ViewData["FarmId"] = new SelectList(_context.Farm.OrderBy(f => f.Name), "FarmId", "Name");
                ViewData["Id"] = new SelectList(_context.Variety.OrderBy(v => v.Name).Where(v => v.CropId.Equals(int.Parse(cropId))), "CropId", "Name");
                return View();
            }
            else
            {
                ViewData["FarmId"] = new SelectList(_context.Farm.OrderBy(f => f.Name), "FarmId", "Name");
                ViewData["Id"] = new SelectList(_context.Variety.OrderBy(v => v.Name), "VarietyId", "Name", varId);
                return View();
            }

            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var plot = await _context.Plot.SingleOrDefaultAsync(m => m.PlotId == id);
            //if (plot == null)
            //{
            //    return NotFound();
            //}
            //ViewData["FarmId"] = new SelectList(    _context.Farm, "FarmId", "ProvinceCode", plot.FarmId);
            //ViewData["VarietyId"] = new SelectList(_context.Variety, "VarietyId", "VarietyId", plot.VarietyId);
            //return View(plot);
        }

        // POST: ADPlot/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlotId,FarmId,VarietyId,DatePlanted,DateHarvested,PlantingRate,PlantingRateByPounds,RowWidth,PatternRepeats,OrganicMatter,BicarbP,Potassium,Magnesium,Calcium,PHsoil,PHbuffer,Cec,PercentBaseSaturationK,PercentBaseSaturationMg,PercentBaseSaturationCa,PercentBaseSaturationH,Comments")] Plot plot)
        {
            if (id != plot.PlotId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlotExists(plot.PlotId))
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
            ViewData["FarmId"] = new SelectList(_context.Farm, "FarmId", "ProvinceCode", plot.FarmId);
            ViewData["VarietyId"] = new SelectList(_context.Variety, "VarietyId", "VarietyId", plot.VarietyId);
            return View(plot);
        }

        // GET: ADPlot/Delete/5
        public async Task<IActionResult> Delete(int? id, string name, string cropId, string varId)
        {
            ViewBag.Name = name;

            if (id == null)
            {
                return NotFound();
            }

            var plot = await _context.Plot
                .Include(p => p.Farm)
                .Include(p => p.Variety)
                .SingleOrDefaultAsync(m => m.PlotId == id);
            if (plot == null)
            {
                return NotFound();
            }

            return View(plot);
        }

        // POST: ADPlot/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plot = await _context.Plot.SingleOrDefaultAsync(m => m.PlotId == id);
            _context.Plot.Remove(plot);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlotExists(int id)
        {
            return _context.Plot.Any(e => e.PlotId == id);
        }
    }
}
