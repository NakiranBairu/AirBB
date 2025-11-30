using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AirBB.Models;
using AirBB.Models.DataLayer.Repositories;
using AirBB.Models.DataLayer;
using AirBB.Models.Domain;

namespace AirBB.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LocationsController : Controller
    {
        private readonly IGenericRepository<Location> _locRepo;

        public LocationsController(IGenericRepository<Location> locRepo)
        {
            _locRepo = locRepo;
        }

        // GET: Admin/Locations
        public async Task<IActionResult> Index()
        {
            var locations = await _locRepo.GetAllAsync();
            return View(locations);
        }

        // GET: Admin/Locations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Locations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocationId,Name")] Location location)
        {
            if (ModelState.IsValid)
            {
                await _locRepo.AddAsync(location);
                TempData["SuccessMessage"] = "Location created successfully!";
                return RedirectToAction(nameof(Index));
            }
            
            if (!ViewData.ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fix the validation errors.";
            }
            
            return View(location);
        }

        // GET: Admin/Locations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _locRepo.GetByIdAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        // POST: Admin/Locations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LocationId,Name")] Location location)
        {
            if (id != location.LocationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _locRepo.UpdateAsync(location);
                    TempData["SuccessMessage"] = "Location updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.LocationId))
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
            
            if (!ViewData.ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fix the validation errors.";
            }
            
            return View(location);
        }

        // GET: Admin/Locations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var options = new QueryOptions<Location>
            {
                Filter = l => l.LocationId == id
            };
            var location = (await _locRepo.GetAllAsync(options)).FirstOrDefault();
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // POST: Admin/Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var location = await _locRepo.GetByIdAsync(id);
            if (location != null)
            {
                await _locRepo.DeleteAsync(location);
                TempData["SuccessMessage"] = "Location deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LocationExists(int id)
        {
            return _locRepo.Query().Any(e => e.LocationId == id);
        }
    }
}