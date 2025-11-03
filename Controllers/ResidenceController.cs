using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AirBB.Models;
using AirBB.Models.ViewModels;

namespace AirBB.Controllers
{
    public class ResidenceController : Controller
    {
        private readonly AirBBContext _context;
        private readonly ISessionManager _sessionManager;
        
        public ResidenceController(AirBBContext context, ISessionManager sessionManager)
        {
            _context = context;
            _sessionManager = sessionManager;
        }
        
        public IActionResult List(string id = "All")
        {
            return Content($"Public Area - Residence Controller - List Action - ID: {id}");
        }
        
        [HttpGet]
        public IActionResult Detail(int id)
        {
            var residence = _context.Residences
                .Include(r => r.Location)
                .FirstOrDefault(r => r.ResidenceId == id);
            
            if (residence == null)
            {
                return RedirectToAction("Index", "Home");
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
        public IActionResult Reserve(Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                var reservations = _sessionManager.GetReservations();
                reservation.ReservationId = reservations.Count + 1;
                reservation.UserId = 1;
                reservation.Residence = _context.Residences
                    .Include(r => r.Location)
                    .First(r => r.ResidenceId == reservation.ResidenceId);
                
                _sessionManager.AddReservation(reservation);
                
                TempData["ReservationMessage"] = $"Successfully reserved {reservation.Residence.Name} from {reservation.ReservationStartDate:MM/dd/yyyy} to {reservation.ReservationEndDate:MM/dd/yyyy}";
                
                return RedirectToAction("Index", "Home");
            }

            var residence = _context.Residences
                .Include(r => r.Location)
                .First(r => r.ResidenceId == reservation.ResidenceId);
            
            var viewModel = new ResidenceDetailViewModel
            {
                Residence = residence,
                Filter = _sessionManager.GetFilterCriteria(),
                Reservation = reservation
            };
            
            return View("Detail", viewModel);
        }
        
        [HttpGet]
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
        public IActionResult Cancel(int reservationId)
        {
            var reservations = _sessionManager.GetReservations();
            var reservationToRemove = reservations.FirstOrDefault(r => r.ReservationId == reservationId);
            
            if (reservationToRemove != null)
            {
                _sessionManager.RemoveReservation(reservationToRemove);
                TempData["ReservationMessage"] = $"Reservation for {reservationToRemove.Residence?.Name ?? "the residence"} has been cancelled.";
            }
            
            return RedirectToAction("Reservations");
        }
    }
}