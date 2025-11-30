using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace AirBB.Models.Infrastructure
{
    public class CookieWrapper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string ReservationIdsKey = "ReservationIds";
        private readonly TimeSpan _defaultExpiry = TimeSpan.FromDays(7); // 7 days as required

        public CookieWrapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Reservations Management (IDs only for security)
        public List<int> ReservationIds
        {
            get
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                if (request == null) return new List<int>();
                
                var reservationIdsJson = request.Cookies[ReservationIdsKey];
                return !string.IsNullOrEmpty(reservationIdsJson) ? 
                    JsonSerializer.Deserialize<List<int>>(reservationIdsJson) ?? new List<int>() 
                    : new List<int>();
            }
            set
            {
                var response = _httpContextAccessor.HttpContext?.Response;
                if (response != null)
                {
                    var options = new CookieOptions
                    {
                        Expires = DateTime.Now.Add(_defaultExpiry),
                        HttpOnly = true,
                        Secure = true, // Use true in production
                        SameSite = SameSiteMode.Strict
                    };

                    var reservationIdsJson = JsonSerializer.Serialize(value ?? new List<int>());
                    response.Cookies.Append(ReservationIdsKey, reservationIdsJson, options);
                }
            }
        }

        // Add a single reservation ID
        public void AddReservationId(int reservationId)
        {
            var currentIds = ReservationIds;
            if (!currentIds.Contains(reservationId))
            {
                currentIds.Add(reservationId);
                ReservationIds = currentIds;
            }
        }

        // Remove a single reservation ID
        public void RemoveReservationId(int reservationId)
        {
            var currentIds = ReservationIds;
            currentIds.Remove(reservationId);
            ReservationIds = currentIds;
        }

        // Clear all cookie data
        public void ClearAll()
        {
            var response = _httpContextAccessor.HttpContext?.Response;
            if (response != null)
            {
                response.Cookies.Delete(ReservationIdsKey);
            }
        }

        // Check if cookie has data
        public bool HasReservationData()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            return request?.Cookies[ReservationIdsKey] != null;
        }

        // Get reservation count
        public int ReservationCount => ReservationIds.Count;
    }
}