using Microsoft.AspNetCore.Http;
using System.Text.Json;
using AirBB.Models.ViewModels;
using AirBB.Models;
using AirBB.Models.Domain;
using AirBB.Models.Utilities;

namespace AirBB.Models.Infrastructure
{
    public class SessionWrapper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string FilterCriteriaKey = "FilterCriteria";
        private const string ReservationsKey = "SessionReservations";

        public SessionWrapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Filter Criteria Management
        public FilterCriteria FilterCriteria
        {
            get
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session == null) return new FilterCriteria();
                
                var criteriaJson = session.GetString(FilterCriteriaKey);
                return criteriaJson != null ? 
                    JsonSerializer.Deserialize<FilterCriteria>(criteriaJson) ?? new FilterCriteria() 
                    : new FilterCriteria();
            }
            set
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session != null)
                {
                    session.SetString(FilterCriteriaKey, JsonSerializer.Serialize(value));
                }
            }
        }

        // Reservations Management (Full objects for session)
        public List<Reservation> Reservations
        {
            get
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session == null) return new List<Reservation>();
                
                var reservationsJson = session.GetString(ReservationsKey);
                return reservationsJson != null ? 
                    JsonSerializer.Deserialize<List<Reservation>>(reservationsJson) ?? new List<Reservation>() 
                    : new List<Reservation>();
            }
            set
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session != null)
                {
                    session.SetString(ReservationsKey, JsonSerializer.Serialize(value));
                }
            }
        }

        // Clear all session data
        public void ClearAll()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.Remove(FilterCriteriaKey);
                session.Remove(ReservationsKey);
            }
        }

        // Clear only filter criteria
        public void ClearFilters()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            session?.Remove(FilterCriteriaKey);
        }

        // Check if session has data
        public bool HasFilterData()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            return session?.GetString(FilterCriteriaKey) != null;
        }

        public bool HasReservationData()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            return session?.GetString(ReservationsKey) != null;
        }
    }
}