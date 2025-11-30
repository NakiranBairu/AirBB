using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace AirBB.Models.Infrastructure
{
    public static class CookieExtensions
    {
        public static void SetCookie<T>(this HttpResponse response, string key, T value, TimeSpan? expireTime = null)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Use true in production
                SameSite = SameSiteMode.Strict,
                Expires = expireTime.HasValue ? DateTime.Now.Add(expireTime.Value) : DateTime.Now.AddDays(7)
            };

            var valueJson = JsonSerializer.Serialize(value);
            response.Cookies.Append(key, valueJson, options);
        }

        public static T? GetCookie<T>(this HttpRequest request, string key)
        {
            var value = request.Cookies[key];
            return string.IsNullOrEmpty(value) ? default : JsonSerializer.Deserialize<T>(value);
        }

        public static bool HasCookie(this HttpRequest request, string key)
        {
            return request.Cookies[key] != null;
        }

        public static void RemoveCookie(this HttpResponse response, string key)
        {
            response.Cookies.Delete(key);
        }
    }
}