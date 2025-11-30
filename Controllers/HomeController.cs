using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AirBB.Models;
using AirBB.Models.ViewModels;
using AirBB.Models.DataLayer.Repositories;
using AirBB.Models.DataLayer;
using AirBB.Models.Domain;
using AirBB.Models.Infrastructure;
using AirBB.Models.Utilities;
using System.Text.Json;

namespace AirBB.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGenericRepository<Residence> _resRepo;
        private readonly IGenericRepository<Location> _locRepo;
        private readonly IGenericRepository<Reservation> _reservationRepo;
        private readonly ISessionManager _sessionManager;

        public HomeController(IGenericRepository<Residence> resRepo,
            IGenericRepository<Location> locRepo,
            IGenericRepository<Reservation> reservationRepo,
            ISessionManager sessionManager)
        {
            _resRepo = resRepo;
            _locRepo = locRepo;
            _reservationRepo = reservationRepo;
            _sessionManager = sessionManager;
        }

        public async Task<IActionResult> Index()
        {
            var filterCriteria = _sessionManager.GetFilterCriteria();
            var locations = (await _locRepo.GetAllAsync()).ToList();
            var residences = await FilterResidences(filterCriteria);
    Console.WriteLine($"Index - FilterCriteria from session - LocationId: {filterCriteria.LocationId}, GuestNumber: {filterCriteria.GuestNumber}, CheckIn: {filterCriteria.CheckInDate}, CheckOut: {filterCriteria.CheckOutDate}");

            var viewModel = new HomeViewModel
            {
                FilterCriteria = filterCriteria,
                Residences = residences,
                Locations = locations
            };

            if (TempData["ReservationMessage"] != null)
            {
                ViewBag.ReservationMessage = TempData["ReservationMessage"];
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Filter([Bind(Prefix = "FilterCriteria")] FilterCriteria criteria)
        {
            // Defensive: ensure criteria is non-null
            if (criteria == null)
            {
                Console.WriteLine("Filter received - criteria is null");
                criteria = new FilterCriteria();
            }

            // Debug logging
            Console.WriteLine($"Filter received - LocationId: {criteria.LocationId}, GuestNumber: {criteria.GuestNumber}, CheckIn: {criteria.CheckInDate}, CheckOut: {criteria.CheckOutDate}");

            _sessionManager.SetFilterCriteria(criteria);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var options = new QueryOptions<Residence>
            {
                Filter = r => r.ResidenceId == id,
                Includes = new List<System.Linq.Expressions.Expression<Func<Residence, object>>> { r => r.Location! }
            };

            var residence = (await _resRepo.GetAllAsync(options)).FirstOrDefault();

            if (residence == null)
            {
                return NotFound();
            }

            var filterCriteria = _sessionManager.GetFilterCriteria();
            var viewModel = new ResidenceDetailViewModel
            {
                Residence = residence,
                Filter = filterCriteria,
                Reservation = new Reservation
                {
                    ResidenceId = id,
                    ReservationStartDate = filterCriteria.CheckInDate ?? DateTime.Today,
                    ReservationEndDate = filterCriteria.CheckOutDate ?? DateTime.Today.AddDays(1)
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Reserve(int residenceId, DateTime startDate, DateTime endDate)
        {
            var residenceReservations = await _reservationRepo.Query().Where(r => r.ResidenceId == residenceId).ToListAsync();

            var isAvailable = !residenceReservations.Any(r =>
                r.ReservationStartDate < endDate &&
                r.ReservationEndDate > startDate);

            if (!isAvailable)
            {
                TempData["ReservationMessage"] = "Sorry, this residence is not available for the selected dates.";
                return RedirectToAction("Details", new { id = residenceId });
            }

            var reservation = new Reservation
            {
                ResidenceId = residenceId,
                UserId = 1,
                ReservationStartDate = startDate,
                ReservationEndDate = endDate
            };

            _sessionManager.AddReservation(reservation);
            TempData["ReservationMessage"] = "Reservation completed successfully!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Reservations()
        {
            var reservations = _sessionManager.GetReservations();

            // Ensure each reservation has its Residence navigation property populated
            foreach (var res in reservations)
            {
                if (res.Residence == null)
                {
                    var residence = await _resRepo.GetByIdAsync(res.ResidenceId);
                    if (residence != null)
                    {
                        res.Residence = residence;
                    }
                }
            }

            var viewModel = new ReservationListViewModel
            {
                Reservations = reservations,
                Filter = _sessionManager.GetFilterCriteria()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CancelReservation(int reservationId, int residenceId)
        {
            var reservations = _sessionManager.GetReservations();

            // Try to find by reservationId first (if persisted), otherwise fallback to residenceId
            Reservation? reservation = null;
            if (reservationId > 0)
            {
                reservation = reservations.FirstOrDefault(r => r.ReservationId == reservationId);
            }

            if (reservation == null && residenceId > 0)
            {
                reservation = reservations.FirstOrDefault(r => r.ResidenceId == residenceId);
            }

            if (reservation != null)
            {
                _sessionManager.RemoveReservation(reservation);
                TempData["ReservationMessage"] = "Reservation cancelled successfully!";
            }

            return RedirectToAction("Reservations");
        }

        [HttpGet]
        public IActionResult GetReservationCount()
        {
            var reservations = _sessionManager.GetReservations();
            return Json(reservations?.Count ?? 0);
        }

        private async Task<List<Residence>> FilterResidences(FilterCriteria criteria)
        {
            var query = _resRepo.Query().Include(r => r.Location).Include(r => r.Reservations).AsQueryable();

            if (criteria == null)
            {
                return await query.ToListAsync();
            }

            // Filter by Location - only if LocationId is specified and greater than 0
            if (criteria.LocationId.HasValue && criteria.LocationId.Value > 0)
            {
                query = query.Where(r => r.LocationId == criteria.LocationId.Value);
            }

            // Filter by Guest Number - only if GuestNumber is specified and greater than 0
            if (criteria.GuestNumber.HasValue && criteria.GuestNumber.Value > 0)
            {
                query = query.Where(r => r.GuestNumber >= criteria.GuestNumber.Value);
            }

            // Filter by Availability dates - only if both dates are specified
            if (criteria.CheckInDate.HasValue && criteria.CheckOutDate.HasValue)
            {
                var checkIn = criteria.CheckInDate.Value;
                var checkOut = criteria.CheckOutDate.Value;

                query = query.Where(r => r.Reservations == null || !r.Reservations.Any(res =>
                    res.ReservationStartDate < checkOut &&
                    res.ReservationEndDate > checkIn));
            }

            return await query.ToListAsync();
        }
    }
}