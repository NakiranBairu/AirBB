using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace AirBB.Models
{
    public interface ISessionManager
    {
        FilterCriteria GetFilterCriteria();
        void SetFilterCriteria(FilterCriteria criteria);
        List<Reservation> GetReservations();
        void SetReservations(List<Reservation> reservations);
        void AddReservation(Reservation reservation);
        void RemoveReservation(Reservation reservation);
    }

    public class SessionManager : ISessionManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISession _session;
        private const string FILTER_KEY = "FilterCriteria";
        private const string RESERVATIONS_KEY = "Reservations";
        private const string COOKIE_KEY = "Reservations";
        private const int COOKIE_EXPIRY_DAYS = 7;

        public SessionManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _session = httpContextAccessor.HttpContext?.Session 
                ?? throw new InvalidOperationException("Session is not available");
        }

        public FilterCriteria GetFilterCriteria()
        {
            var json = _session.GetString(FILTER_KEY);
            return json == null ? new FilterCriteria() 
                : JsonSerializer.Deserialize<FilterCriteria>(json) ?? new FilterCriteria();
        }

        public void SetFilterCriteria(FilterCriteria criteria)
        {
            var json = JsonSerializer.Serialize(criteria);
            _session.SetString(FILTER_KEY, json);
        }

        public List<Reservation> GetReservations()
        {
            var json = _session.GetString(RESERVATIONS_KEY);
            var sessionReservations = json == null ? new List<Reservation>() 
                : JsonSerializer.Deserialize<List<Reservation>>(json) ?? new List<Reservation>();

            // Sync with cookie if session is empty
            if (!sessionReservations.Any())
            {
                var cookieJson = _httpContextAccessor.HttpContext?.Request.Cookies[COOKIE_KEY];
                if (!string.IsNullOrEmpty(cookieJson))
                {
                    var reservationIds = JsonSerializer.Deserialize<List<int>>(cookieJson);
                    // Here you would typically load the full reservation details from your database
                    // For now, we'll just create basic reservation objects
                    sessionReservations = reservationIds?.Select(id => new Reservation { ResidenceId = id }).ToList() 
                        ?? new List<Reservation>();
                    SetReservations(sessionReservations);
                }
            }

            return sessionReservations;
        }

        public void SetReservations(List<Reservation> reservations)
        {
            var json = JsonSerializer.Serialize(reservations);
            _session.SetString(RESERVATIONS_KEY, json);

            // Update cookie with reservation IDs
            var reservationIds = reservations.Select(r => r.ResidenceId).ToList();
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(COOKIE_EXPIRY_DAYS),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            };
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(
                COOKIE_KEY, 
                JsonSerializer.Serialize(reservationIds),
                options
            );
        }

        public void AddReservation(Reservation reservation)
        {
            var reservations = GetReservations();
            reservations.Add(reservation);
            SetReservations(reservations);
        }

        public void RemoveReservation(Reservation reservation)
        {
            var reservations = GetReservations();
            reservations.RemoveAll(r => r.ResidenceId == reservation.ResidenceId);
            SetReservations(reservations);
        }
    }
}