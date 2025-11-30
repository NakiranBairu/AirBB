using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore; 
using AirBB.Models;
using AirBB.Models.DataLayer;
using AirBB.Models.DataLayer.Repositories;
using AirBB.Models.Domain;
using System.Linq.Expressions;

namespace AirBB.Models.Infrastructure
{
    public interface IDataSyncService
    {
        Task SyncSessionFromCookies();
        void SyncCookiesFromSession();
        void EnsureDataConsistency();
        void ClearAllUserData();
    }

    public class DataSyncService : IDataSyncService
    {
        private readonly SessionWrapper _sessionWrapper;
        private readonly CookieWrapper _cookieWrapper;
        private readonly IGenericRepository<Reservation> _reservationRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DataSyncService(SessionWrapper sessionWrapper, CookieWrapper cookieWrapper, 
                             IGenericRepository<Reservation> reservationRepo, IHttpContextAccessor httpContextAccessor)
        {
            _sessionWrapper = sessionWrapper;
            _cookieWrapper = cookieWrapper;
            _reservationRepo = reservationRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        // Sync session from cookies on first load
        public async Task SyncSessionFromCookies()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return;

            // Check if this is the first request (no session data but has cookie data)
            if (!_sessionWrapper.HasReservationData() && _cookieWrapper.HasReservationData())
            {
                var cookieReservationIds = _cookieWrapper.ReservationIds;
                
                if (cookieReservationIds.Any())
                {
                    // Use repository Query to get reservations with includes
                    var reservations = await _reservationRepo.Query()
                        .Include(r => r.Residence) 
                        .Include(r => r.Client)     
                        .Where(r => cookieReservationIds.Contains(r.ReservationId))
                        .ToListAsync();

                    _sessionWrapper.Reservations = reservations;
                }
            }
        }

        // Sync cookies from session 
        public void SyncCookiesFromSession()
        {
            var sessionReservations = _sessionWrapper.Reservations;
            var reservationIds = sessionReservations.Select(r => r.ReservationId).ToList();
            _cookieWrapper.ReservationIds = reservationIds;
        }

        // Ensure data consistency between session and cookies
        public void EnsureDataConsistency()
        {
            var sessionReservationIds = _sessionWrapper.Reservations.Select(r => r.ReservationId).ToList();
            var cookieReservationIds = _cookieWrapper.ReservationIds;

            // Find IDs that are in session but not in cookies
            var missingInCookies = sessionReservationIds.Except(cookieReservationIds).ToList();
            
            // Find IDs that are in cookies but not in session (possibly deleted)
            var missingInSession = cookieReservationIds.Except(sessionReservationIds).ToList();

            // Update cookies if there are discrepancies
            if (missingInCookies.Any() || missingInSession.Any())
            {
                _cookieWrapper.ReservationIds = sessionReservationIds;
            }
        }

        // Clear all user data
        public void ClearAllUserData()
        {
            _sessionWrapper.ClearAll();
            _cookieWrapper.ClearAll();
        }
    }
}