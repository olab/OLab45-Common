using System;

namespace OLab.Data.BusinessObjectsEx.API
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public string Refresh { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
    }
}