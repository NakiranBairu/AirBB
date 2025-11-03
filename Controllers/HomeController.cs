using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AirBB.Models;
using AirBB.Models.ViewModels;
using System.Text.Json;

namespace AirBB.Controllers
{
    public class HomeController : Controller
    {
        private readonly AirBBContext _context;
        private readonly ISessionManager _sessionManager;

        public HomeController(AirBBContext context, ISessionManager sessionManager)
        {
            _context = context;
            _sessionManager = sessionManager;
        }

        public async Task<IActionResult> Index()
        {
            var filterCriteria = _sessionManager.GetFilterCriteria();
            var locations = await _context.Locations.ToListAsync();
            var residences = await FilterResidences(filterCriteria);

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
        public IActionResult Filter(FilterCriteria criteria)
        {
            _sessionManager.SetFilterCriteria(criteria);
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var residence = _context.Residences
                .Include(r => r.Location)
                .FirstOrDefault(r => r.ResidenceId == id);

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
        public IActionResult Reserve(int residenceId, DateTime startDate, DateTime endDate)
        {
            var residenceReservations = _context.Reservations
                .Where(r => r.ResidenceId == residenceId)
                .ToList();

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

        public IActionResult Reservations()
        {
            var reservations = _sessionManager.GetReservations();
            var viewModel = new ReservationListViewModel
            {
                Reservations = reservations,
                Filter = _sessionManager.GetFilterCriteria()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CancelReservation(int reservationId)
        {
            var reservations = _sessionManager.GetReservations();
            var reservation = reservations.FirstOrDefault(r => r.ReservationId == reservationId);
            
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
            if (criteria == null)
            {
                return await _context.Residences.Include(r => r.Location).ToListAsync();
            }

            var query = _context.Residences.Include(r => r.Location).Include(r => r.Reservations).AsQueryable();

            if (criteria.LocationId.HasValue && criteria.LocationId > 0)
            {
                query = query.Where(r => r.LocationId == criteria.LocationId.Value);
            }

            if (criteria.GuestNumber.HasValue && criteria.GuestNumber > 0)
            {
                query = query.Where(r => r.GuestNumber >= criteria.GuestNumber.Value);
            }

            if (criteria.CheckInDate.HasValue && criteria.CheckOutDate.HasValue)
            {
                query = query.Where(r => r.Reservations == null || !r.Reservations.Any(res =>
                    res.ReservationStartDate < criteria.CheckOutDate.Value &&
                    res.ReservationEndDate > criteria.CheckInDate.Value));
            }

            return await query.ToListAsync();
        }
    }
}